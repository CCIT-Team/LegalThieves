namespace Fusion.Addons.KCC
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	public class KCCNetworkContext
	{
		public KCC         KCC;
		public KCCData     Data;
		public KCCSettings Settings;
	}

	// This file contains implementation related to network synchronization and interpolation based on network buffers.
	public unsafe partial class KCC
	{
		// PRIVATE MEMBERS

		private KCCNetworkContext     _networkContext;
		private IKCCNetworkProperty[] _networkProperties;
		private int                   _interpolationTick;
		private int                   _interpolationAttempts;

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

			RenderSource    defaultSource    = Object.RenderSource;
			RenderTimeframe defaultTimeframe = Object.RenderTimeframe;

			Object.RenderSource    = RenderSource.Interpolated;
			Object.RenderTimeframe = GetInterpolationTimeframe();

			bool buffersValid = TryGetSnapshotsBuffers(out NetworkBehaviourBuffer fromBuffer, out NetworkBehaviourBuffer toBuffer, out float alpha);

			Object.RenderSource    = defaultSource;
			Object.RenderTimeframe = defaultTimeframe;

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

		private void InterpolateNetworkData(RenderSource renderSource, RenderTimeframe renderTimeframe, float interpolationAlpha = -1.0f)
		{
			RenderSource    defaultSource    = Object.RenderSource;
			RenderTimeframe defaultTimeframe = Object.RenderTimeframe;

			Object.RenderSource    = renderSource;
			Object.RenderTimeframe = renderTimeframe;

			bool buffersValid = TryGetSnapshotsBuffers(out NetworkBehaviourBuffer fromBuffer, out NetworkBehaviourBuffer toBuffer, out float alpha);

			Object.RenderSource    = defaultSource;
			Object.RenderTimeframe = defaultTimeframe;

			if (buffersValid == false)
				return;
			if (UpdateInterpolationTick(fromBuffer.Tick, toBuffer.Tick) == false)
				return;

			if (interpolationAlpha >= 0.0f && interpolationAlpha <= 1.0f)
			{
				alpha = interpolationAlpha;
			}

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
			RenderSource    defaultSource    = Object.RenderSource;
			RenderTimeframe defaultTimeframe = Object.RenderTimeframe;

			Object.RenderSource    = RenderSource.Interpolated;
			Object.RenderTimeframe = RenderTimeframe.Remote;

			bool buffersValid = TryGetSnapshotsBuffers(out NetworkBehaviourBuffer fromBuffer, out NetworkBehaviourBuffer toBuffer, out float alpha);

			Object.RenderSource    = defaultSource;
			Object.RenderTimeframe = defaultTimeframe;

			if (buffersValid == false)
				return;
			if (UpdateInterpolationTick(fromBuffer.Tick, toBuffer.Tick) == false)
				return;

			KCCNetworkProperties.ReadTransforms(fromBuffer, toBuffer, out Vector3 fromPosition, out Vector3 toPosition, out float fromLookPitch, out float toLookPitch, out float fromLookYaw, out float toLookYaw);

			_fixedData.BasePosition    = fromPosition;
			_fixedData.DesiredPosition = toPosition;
			_fixedData.TargetPosition  = Vector3.Lerp(fromPosition, toPosition, alpha);
			_fixedData.LookPitch       = Mathf.Lerp(fromLookPitch, toLookPitch, alpha);
			_fixedData.LookYaw         = KCCUtility.InterpolateRange(fromLookYaw, toLookYaw, -180.0f, 180.0f, alpha);

			_renderData.BasePosition    = _fixedData.BasePosition;
			_renderData.DesiredPosition = _fixedData.DesiredPosition;
			_renderData.TargetPosition  = _fixedData.TargetPosition;
			_renderData.LookPitch       = _fixedData.LookPitch;
			_renderData.LookYaw         = _fixedData.LookYaw;

			_transform.SetPositionAndRotation(_renderData.TargetPosition, _renderData.TransformRotation);
		}

		private bool UpdateInterpolationTick(int fromTick, int toTick)
		{
			int ticks = toTick - fromTick;
			if (ticks <= 0 && _interpolationTick == fromTick)
			{
				// There's no new data for interpolation.
				_interpolationAttempts = 30;
				return false;
			}

			if (_interpolationAttempts > 0)
			{
				// We have new data for remote snapshot interpolation, however the buffer has invalid tick equal to Runner.Tick.
				// Just ignore this case, the tick should be corrected within several frames.
				if (toTick == Runner.Tick)
				{
					--_interpolationAttempts;
					return false;
				}

				_interpolationAttempts = 0;
			}

			_interpolationTick = fromTick;
			return true;
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
