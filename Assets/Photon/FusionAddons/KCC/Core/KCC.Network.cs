namespace Fusion.Addons.KCC
{
	using System.Collections.Generic;
	using UnityEngine;

	public class KCCNetworkContext
	{
		public KCC         KCC;
		public KCCData     Data;
		public KCCSettings Settings;

		public int LastInterpolationJumpCounter     = -1;
		public int LastInterpolationTeleportCounter = -1;
	}

	// This file contains implementation related to network synchronization and interpolation based on network buffers.
	public unsafe partial class KCC
	{
		// PRIVATE MEMBERS

		private KCCNetworkContext     _networkContext;
		private IKCCNetworkProperty[] _networkProperties;

		// PUBLIC METHODS

		/// <summary>
		/// Returns position stored in network buffer.
		/// </summary>
		public Vector3 GetNetworkBufferPosition()
		{
			fixed (int* ptr = &ReinterpretState<int>())
			{
				return ((NetworkTRSPData*)ptr)->Position + KCCNetworkUtility.ReadVector3(ptr + NetworkTRSPData.WORDS);
			}
		}

		/// <summary>
		/// Returns interpolated position based on data stored in network buffers.
		/// </summary>
		public bool GetInterpolatedNetworkBufferPosition(out Vector3 interpolatedPosition)
		{
			interpolatedPosition = default;

			bool buffersValid = TryGetSnapshotsBuffers(out NetworkBehaviourBuffer fromBuffer, out NetworkBehaviourBuffer toBuffer, out float alpha);
			if (buffersValid == false)
				return false;

			KCCNetworkProperties.ReadPositions(fromBuffer, toBuffer, out Vector3 fromPosition, out Vector3 toPosition);

			interpolatedPosition = Vector3.Lerp(fromPosition, toPosition, alpha);

			return true;
		}

		// PRIVATE METHODS

		private int GetNetworkDataWordCount()
		{
			InitializeNetworkProperties();

			int wordCount = 0;

			for (int i = 0, count = _networkProperties.Length; i < count; ++i)
			{
				IKCCNetworkProperty property = _networkProperties[i];
				wordCount += property.WordCount;
			}

			return wordCount;
		}

		private void ReadNetworkData()
		{
			_networkContext.Data = _fixedData;

			fixed (int* statePtr = &ReinterpretState<int>())
			{
				int* ptr = statePtr;

				for (int i = 0, count = _networkProperties.Length; i < count; ++i)
				{
					IKCCNetworkProperty property = _networkProperties[i];
					property.Read(ptr);
					ptr += property.WordCount;
				}
			}
		}

		private void WriteNetworkData()
		{
			_networkContext.Data = _fixedData;

			fixed (int* statePtr = &ReinterpretState<int>())
			{
				int* ptr = statePtr;

				for (int i = 0, count = _networkProperties.Length; i < count; ++i)
				{
					IKCCNetworkProperty property = _networkProperties[i];
					property.Write(ptr);
					ptr += property.WordCount;
				}
			}
		}

		private void InterpolateNetworkData()
		{
			bool buffersValid = TryGetSnapshotsBuffers(out NetworkBehaviourBuffer fromBuffer, out NetworkBehaviourBuffer toBuffer, out float alpha);
			if (buffersValid == false)
				return;

			float deltaTime  = Runner.DeltaTime;
			float renderTick = fromBuffer.Tick + alpha * (toBuffer.Tick - fromBuffer.Tick);

			_renderData.CopyFromOther(_fixedData);

			_renderData.Frame           = Time.frameCount;
			_renderData.Tick            = Mathf.RoundToInt(renderTick);
			_renderData.Alpha           = alpha;
			_renderData.DeltaTime       = deltaTime;
			_renderData.UpdateDeltaTime = deltaTime;
			_renderData.Time            = renderTick * deltaTime;

			_networkContext.Data = _renderData;

			KCCInterpolationInfo interpolationInfo = new KCCInterpolationInfo();
			interpolationInfo.FromBuffer = fromBuffer;
			interpolationInfo.ToBuffer   = toBuffer;
			interpolationInfo.Alpha      = alpha;

			for (int i = 0, count = _networkProperties.Length; i < count; ++i)
			{
				IKCCNetworkProperty property = _networkProperties[i];
				property.Interpolate(interpolationInfo);
				interpolationInfo.Offset += property.WordCount;
			}

			// User interpolation and post-processing.
			InterpolateUserNetworkData(_renderData, interpolationInfo);
		}

		private void InterpolateNetworkTransform()
		{
			bool buffersValid = TryGetSnapshotsBuffers(out NetworkBehaviourBuffer fromBuffer, out NetworkBehaviourBuffer toBuffer, out float alpha);
			if (buffersValid == false)
				return;

			KCCNetworkProperties.ReadTransforms(fromBuffer, toBuffer, out Vector3 fromPosition, out Vector3 toPosition, out float fromLookPitch, out float toLookPitch, out float fromLookYaw, out float toLookYaw);

			Vector3 targetPosition = Vector3.Lerp(fromPosition, toPosition, alpha);
			float   lookPitch      = Mathf.Lerp(fromLookPitch, toLookPitch, alpha);
			float   lookYaw        = KCCUtility.InterpolateRange(fromLookYaw, toLookYaw, -180.0f, 180.0f, alpha);
			Vector3 realVelocity   = default;
			float   realSpeed      = default;

			int ticks = toBuffer.Tick - fromBuffer.Tick;
			if (ticks > 0)
			{
				realVelocity = (toPosition - fromPosition) / (Runner.DeltaTime * ticks);
				realSpeed    = realVelocity.magnitude;
			}

			_renderData.BasePosition    = fromPosition;
			_renderData.DesiredPosition = toPosition;
			_renderData.TargetPosition  = targetPosition;
			_renderData.LookPitch       = lookPitch;
			_renderData.LookYaw         = lookYaw;
			_renderData.RealVelocity    = realVelocity;
			_renderData.RealSpeed       = realSpeed;

			_fixedData.BasePosition    = _renderData.BasePosition;
			_fixedData.DesiredPosition = _renderData.DesiredPosition;
			_fixedData.TargetPosition  = _renderData.TargetPosition;
			_fixedData.LookPitch       = _renderData.LookPitch;
			_fixedData.LookYaw         = _renderData.LookYaw;
			_fixedData.RealVelocity    = _renderData.RealVelocity;
			_fixedData.RealSpeed       = _renderData.RealSpeed;

			_transform.SetPositionAndRotation(_renderData.TargetPosition, _renderData.TransformRotation);
		}

		private void RestoreHistoryData(KCCData historyData)
		{
			// Some values can be synchronized from user code.
			// We have to ensure these properties are in correct state with other properties.

			if (_fixedData.IsGrounded == true)
			{
				// Reset IsGrounded and WasGrounded to history state, otherwise using GroundNormal and other ground related properties leads to undefined behavior and NaN propagation.
				// This has effect only if IsGrounded and WasGrounded is synchronized over network.
				_fixedData.IsGrounded  = historyData.IsGrounded;
				_fixedData.WasGrounded = historyData.WasGrounded;
			}

			// User history data restoration.

			RestoreUserHistoryData(historyData);
		}

		private void InitializeNetworkProperties()
		{
			if (_networkContext != null)
				return;

			_networkContext = new KCCNetworkContext();
			_networkContext.KCC      = this;
			_networkContext.Settings = _settings;

			List<IKCCNetworkProperty> properties = new List<IKCCNetworkProperty>(32);
			properties.Add(new KCCNetworkProperties(_networkContext));

			InitializeUserNetworkProperties(_networkContext, properties);

			_networkProperties = properties.ToArray();
		}

		// PARTIAL METHODS

		partial void InitializeUserNetworkProperties(KCCNetworkContext networkContext, List<IKCCNetworkProperty> networkProperties);
		partial void InterpolateUserNetworkData(KCCData data, KCCInterpolationInfo interpolationInfo);
		partial void RestoreUserHistoryData(KCCData historyData);
	}
}
