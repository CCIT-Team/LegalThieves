namespace Fusion.Addons.KCC
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using Fusion;

	// All platform related objecs must respect this execution order to work correctly:
	// 1. Update of all IPlatform         => Calculating new position/rotation values and updating Transform and Rigidbody components.
	// 2. Update of all PlatformProcessor => IPlatform tracking, propagation of their Transform changes since last update to KCC Transform and KCCData.
	// 3. Update of all KCC               => Predicted movement and interpolation.

	/// <summary>
	/// Use this interface to mark a processor as platform.
	/// Make sure the script that moves with the platform object has lower execution order => it must be executed before <c>PlatformProcessor</c>.
	/// </summary>
	public interface IPlatform
	{
		NetworkObject Object { get; }
	}

	/// <summary>
	/// Interface to notify other processors about KCC being transformed.
	/// </summary>
	public interface IPlatformListener
	{
		void OnTransform(KCC kcc, KCCData data, Vector3 positionDelta, Quaternion rotationDelta);
	}

	/// <summary>
	/// This processor tracks overlapping platforms (KCC processors implementing <c>IPlatform</c>) and propagates their position and rotation changes to <c>KCC</c>.
	/// Make sure the script that moves with the <c>IPlatform</c> object has lower execution order => it must be executed before <c>PlatformProcessor</c>.
	/// When <c>PlatformProcessor</c> propagates all platform changes, it notifies <c>IPlatformListener</c> processors with absolute transform deltas.
	/// The <c>PlatformProcessor</c> requires to be simulated on all clients and calls Runner.SetIsSimulated(Object, true);
	/// </summary>
	[DefaultExecutionOrder(-400)]
	[RequireComponent(typeof(NetworkObject))]
    public unsafe class PlatformProcessor : NetworkKCCProcessor, IKCCProcessor, IBeginMove, IEndMove
    {
		// CONSTANTS

		private const int MAX_PLATFORMS = 3;

		// PRIVATE MEMBERS

		[SerializeField][Tooltip("How long it takes to move the KCC from world space to platform space.")]
		private float _platformSpaceTransitionDuration = 0.75f;
		[SerializeField][Tooltip("How long it takes to move the KCC from platform space to world space.")]
		private float _worldSpaceTransitionDuration = 0.5f;

		[Networked]
		private ref ProcessorState _state => ref MakeRef<ProcessorState>();

		private KCC        _kcc;
		private Platform[] _renderPlatforms = new Platform[MAX_PLATFORMS];

		private static List<IPlatform> _cachedPlatforms   = new List<IPlatform>();
		private static List<NetworkId> _cachedNetworkIds1 = new List<NetworkId>();
		private static List<NetworkId> _cachedNetworkIds2 = new List<NetworkId>();

		// PUBLIC METHODS

		/// <summary>
		/// Returns <c>true</c> if there is at least one platorm tracked.
		/// </summary>
		public bool IsActive()
		{
			return _state.IsActive;
		}

		// NetworkBehaviour INTERFACE

		public override void Spawned()
		{
			if (Runner.GameMode != GameMode.Shared)
			{
				// Enable simulation on this object. Proxies will get all Fusion callbacks.
				Runner.SetIsSimulated(Object, true);
			}
		}

		public override void FixedUpdateNetwork()
		{
			if (ReferenceEquals(_kcc, null) == true)
				return;

			if (_kcc.Object.IsInSimulation == true)
			{
				// Update state of platforms, track new, cleanup old.
				UpdatePlatforms(_kcc);

				if (_state.IsActive == true)
				{
					// For predicted KCC, propagate position and rotation deltas of all platforms since last fixed update.
					PropagateMovement(_kcc, _kcc.FixedData, true);

					// Copy fixed state to render state as a base.
					SynchronizeRenderPlatforms();
				}
			}
			else
			{
				// Otherwise snap the KCC to tracked platforms based on interpolated offsets.
				// Notice we modify only position, this is essential to get correct results from KCC physics queries. Rotation keeps unchanged.
				if (TrySetInterpolatedPosition(_kcc, _kcc.FixedData) == true)
				{
					_kcc.SynchronizeTransform(true, false, false);
				}
			}
		}

		public override void Render()
		{
			if (ReferenceEquals(_kcc, null) == true)
				return;

			if (_kcc.IsPredictingInRenderUpdate == true)
			{
				if (_state.IsActive == true)
				{
					// For render-predicted KCC, propagate position and rotation deltas of all platforms since last fixed or render update.
					PropagateMovement(_kcc, _kcc.RenderData, false);
				}
			}
			else
			{
				// Otherwise snap the KCC to tracked platforms based on interpolated offsets.
				// Notice we modify only position, this is essential to get correct results from KCC physics queries. Rotation keeps unchanged.
				if (TrySetInterpolatedPosition(_kcc, _kcc.RenderData) == true)
				{
					_kcc.SynchronizeTransform(true, false, false);
				}
			}
		}

		// NetworkKCCProcessor INTERFACE

		public override float GetPriority(KCC kcc) => float.MinValue;

		public override void OnEnter(KCC kcc, KCCData data)
		{
			_kcc = kcc;
		}

		public override void OnExit(KCC kcc, KCCData data)
		{
			_kcc = null;
		}

		public override void OnInterpolate(KCC kcc, KCCData data)
		{
			// This code path can be executed for:
			// 1. Proxy interpolated in fixed update.
			// 2. Proxy interpolated in render update.
			// 3. Input/State authority interpolated in render update.

			// For KCC proxy, KCCData.TargetPosition equals to snapshot interpolated position at this point.
			// However platforms are predicted everywhere - on all server and clients.
			// If a platform is predicted and KCC proxy interpolated, it results in KCC visual being delayed behind the platform visual.

			// Following code recalculates KCC position by snapping it to predicted platform space, matching position of the platform visual.
			// [KCC position] = [local IPlatform position] + [interpolated IPlatform => KCC offset].
			TrySetInterpolatedPosition(kcc, data);
		}

		// IKCCProcessor INTERFACE

		bool IKCCProcessor.IsActive(KCC kcc) => _state.IsActive;

		// IBeginMove INTERFACE

		float IKCCStage<BeginMove>.GetPriority(KCC kcc) => float.MaxValue;

		void IKCCStage<BeginMove>.Execute(BeginMove stage, KCC kcc, KCCData data)
		{
			// Disable prediction correction and anti-jitter if there is at least one platform tracked.
			// This must be called in both fixed and render update.
			kcc.SuppressFeature(EKCCFeature.PredictionCorrection);
			kcc.SuppressFeature(EKCCFeature.AntiJitter);
		}

		// IEndMove INTERFACE

		float IKCCStage<EndMove>.GetPriority(KCC kcc) => float.MinValue;

		void IKCCStage<EndMove>.Execute(EndMove stage, KCC kcc, KCCData data)
		{
			bool isInFixedUpdate = kcc.IsInFixedUpdate;

			// Update Platform => KCC offset after KCC moves.
			for (int i = 0, count = _state.Platforms.Length; i < count; ++i)
			{
				Platform platform = GetPlatform(i, isInFixedUpdate);
				if (platform.State != EPlatformState.None)
				{
					platform.KCCOffset = Quaternion.Inverse(platform.Rotation) * (data.TargetPosition - platform.Position);
					SetPlatform(i, platform, isInFixedUpdate);
				}
			}
		}

		// PRIVATE METHODS

		private void UpdatePlatforms(KCC kcc)
		{
			// 1. Get all platform objects tracked by KCC.
			kcc.GetProcessors<IPlatform>(_cachedPlatforms);

			// Early exit - performance optimziation.
			if (_cachedPlatforms.Count <= 0 && _state.IsActive == false)
				return;

			_cachedNetworkIds1.Clear(); // Used to store platforms tracked by KCC.
			_cachedNetworkIds2.Clear(); // Used to store platforms tracked by PlatformProcessor.

			foreach (IPlatform platform in _cachedPlatforms)
			{
				_cachedNetworkIds1.Add(platform.Object.Id);
			}

			// 2. Mark all platforms in PlatformProcessor state as inactive if they are not tracked by KCC.
			for (int i = 0, count = _state.Platforms.Length; i < count; ++i)
			{
				Platform platform = _state.Platforms.Get(i);
				if (platform.State == EPlatformState.Active && _cachedNetworkIds1.Contains(platform.Id) == false)
				{
					platform.State = EPlatformState.Inactive;

					_state.Platforms.Set(i, platform);
				}

				if (platform.Id.IsValid == true)
				{
					_cachedNetworkIds2.Add(platform.Id);
				}
			}

			// 3. Register all platforms tracked by KCC that are not tracked by PlatformProcessor.
			foreach (IPlatform trackedPlatform in _cachedPlatforms)
			{
				NetworkObject platformObject = trackedPlatform.Object;
				if (_cachedNetworkIds2.Contains(platformObject.Id) == false)
				{
					// The platform is not yet tracked by PlatformProcessor. Let's try adding it.
					for (int i = 0, count = _state.Platforms.Length; i < count; ++i)
					{
						if (_state.Platforms.Get(i).State == EPlatformState.None)
						{
							_cachedNetworkIds2.Add(platformObject.Id);

							platformObject.transform.GetPositionAndRotation(out Vector3 platformPosition, out Quaternion platformRotation);

							Platform platform = new Platform();
							platform.Id        = platformObject.Id;
							platform.State     = EPlatformState.Active;
							platform.Alpha     = default;
							platform.Position  = platformPosition;
							platform.Rotation  = platformRotation;
							platform.KCCOffset = Quaternion.Inverse(platformRotation) * (kcc.Transform.position - platformPosition);

							_state.Platforms.Set(i, platform);
							break;
						}
					}
				}
			}

			bool isActive = false;

			// 4. Update platforms alpha values.
			// The platform alpha defines how much is the KCC position affected by the platform and is used for smooth transition from from world space to platform space.
			for (int i = 0, count = _state.Platforms.Length; i < count; ++i)
			{
				Platform platform = _state.Platforms.Get(i);
				if (platform.State == EPlatformState.Active)
				{
					isActive = true;

					if (platform.Alpha < 1.0f)
					{
						// The KCC stands within the platform, increasing alpha to 1.0f.
						platform.Alpha = _platformSpaceTransitionDuration > 0.001f ? Mathf.Min(platform.Alpha + Runner.DeltaTime / _platformSpaceTransitionDuration, 1.0f) : 1.0f;
						_state.Platforms.Set(i, platform);
					}
				}
				else if (platform.State == EPlatformState.Inactive)
				{
					// The KCC left the the platform, decreasing alpha to 0.0f.
					platform.Alpha -= _worldSpaceTransitionDuration > 0.001f ? (Runner.DeltaTime / _worldSpaceTransitionDuration) : 1.0f;

					if (platform.Alpha <= 0.0f)
					{
						// Once the alpha is 0.0f, we can remove the platform entirely.
						platform = default;
					}
					else
					{
						isActive = true;
					}

					_state.Platforms.Set(i, platform);
				}
			}

			_state.IsActive = isActive;
		}

		private void PropagateMovement(KCC kcc, KCCData data, bool isInFixedUpdate)
		{
			bool       synchronize  = false;
			Vector3    basePosition = data.TargetPosition;
			Quaternion baseRotation = data.TransformRotation;

			// 1. Iterate over all tracked platforms, calculate their position and rotation deltas and propagate them to the KCC.
			for (int i = 0, count = _state.Platforms.Length; i < count; ++i)
			{
				Platform platform = GetPlatform(i, isInFixedUpdate);
				if (platform.State != EPlatformState.Active || platform.Id.IsValid == false)
					continue;

				NetworkObject platformObject = Runner.FindObject(platform.Id);
				if (platformObject == null || platformObject.TryGetComponent(out IPlatform synchronizePlatform) == false)
					continue;

				platformObject.transform.GetPositionAndRotation(out Vector3 currentPlatformPosition, out Quaternion currentPlatformRotation);

				// Calculate platform position and rotation delta since last update.
				Vector3    platformPositionDelta = currentPlatformPosition - platform.Position;
				Quaternion platformRotationDelta = Quaternion.Inverse(platform.Rotation) * currentPlatformRotation;

				if (platform.State == EPlatformState.Inactive)
				{
					// With decreasing alpha we are also lowering the impact of platform transform changes.
					platformRotationDelta = Quaternion.Slerp(Quaternion.identity, platformRotationDelta, platform.Alpha);
				}

				// The platform rotated, we have to rotate stored KCC position offset.
				Vector3 recalculatedKCCOffset = platformRotationDelta * platform.KCCOffset;

				// Calculate delta between old and new KCC position offset. This needs to be added to KCC to stay on a platform spot.
				Vector3 kccOffsetDelta = recalculatedKCCOffset - platform.KCCOffset;

				// Final KCC position delta is calculated as sum of platform delta and KCC offset delta.
				// Notice the KCC offset is in platform local space so it needs to be rotated.
				Vector3 kccPositionDelta = platformPositionDelta + currentPlatformRotation * kccOffsetDelta;

				if (platform.State == EPlatformState.Inactive)
				{
					// With decreasing alpha we are also lowering the impact of platform transform changes.
					kccPositionDelta = Vector3.Lerp(Vector3.zero, kccPositionDelta, platform.Alpha);
				}

				// Propagate calculated position delta to the KCC.
				data.BasePosition    += kccPositionDelta;
				data.DesiredPosition += kccPositionDelta;
				data.TargetPosition  += kccPositionDelta;

				// Propagate rotation delta to the KCC.
				data.AddLookRotation(0.0f, platformRotationDelta.eulerAngles.y);

				// Update platform properties with new values.
				platform.Position  = currentPlatformPosition;
				platform.Rotation  = currentPlatformRotation;
				platform.KCCOffset = recalculatedKCCOffset;

				// Update PlatformProcessor state.
				SetPlatform(i, platform, isInFixedUpdate);

				// Set flag to synchronize Transform and Ridigbody components.
				synchronize = true;
			}

			// 2. Deltas from all platforms are propagated, now we have to recalculate Platform => KCC offsets.
			for (int i = 0, count = _state.Platforms.Length; i < count; ++i)
			{
				Platform platform = GetPlatform(i, isInFixedUpdate);
				if (platform.State != EPlatformState.None)
				{
					// Offset needs to be calculated for both Active and Inactive platforms.
					platform.KCCOffset = Quaternion.Inverse(platform.Rotation) * (data.TargetPosition - platform.Position);

					// Update PlatformProcessor state.
					SetPlatform(i, platform, isInFixedUpdate);
				}
			}

			if (synchronize == true)
			{
				// There is at least one platform tracked, Transform and Rigidbody should be refreshed before any KCC begins predicted move.
				kcc.SynchronizeTransform(true, true, false);

				Vector3    positionDelta = data.TargetPosition - basePosition;
				Quaternion rotationDelta = Quaternion.Inverse(baseRotation) * data.TransformRotation;

				// Notify all listeners.
				foreach (IPlatformListener listener in kcc.GetProcessors<IPlatformListener>(true))
				{
					try
					{
						listener.OnTransform(kcc, data, positionDelta, rotationDelta);
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
					}
				}
			}
		}

		private bool TrySetInterpolatedPosition(KCC kcc, KCCData data)
		{
			// At this point all platforms (IPlatform) should have updated their transforms.
			// This method calculates interpolated position of the KCC by taking local platform positions + interpolated Position => KCC offsets.
			// Calculations below result in smooth transition between world and multiple platform spaces.

			RenderSource    defaultSource    = Object.RenderSource;
			RenderTimeframe defaultTimeframe = Object.RenderTimeframe;

			// Timeframe of PlatformProcessor is locked to timeframe of KCC, both use same prediction/interpolation strategy.
			Object.RenderSource    = RenderSource.Interpolated;
			Object.RenderTimeframe = kcc.GetInterpolationTimeframe();

			bool buffersValid = TryGetSnapshotsBuffers(out NetworkBehaviourBuffer fromBuffer, out NetworkBehaviourBuffer toBuffer, out float alpha);

			Object.RenderSource    = defaultSource;
			Object.RenderTimeframe = defaultTimeframe;

			if (buffersValid == false)
				return false;

			bool isInSimulation = kcc.Object.IsInSimulation;

			Vector3 averagePosition = default;
			float   averageAlpha    = default;

			ProcessorState fromState = fromBuffer.ReinterpretState<ProcessorState>();
			ProcessorState toState   = toBuffer.ReinterpretState<ProcessorState>();

			for (int i = 0; i < toState.Platforms.Length; ++i)
			{
				Platform fromPlatform = fromState.Platforms.Get(i);
				Platform toPlatform   = toState.Platforms.Get(i);

				if (fromPlatform.State == EPlatformState.None)
				{
					if (toPlatform.State == EPlatformState.None)
						continue;

					// Only To is valid => the KCC just jumped on the platform.

					// Render interpolated KCC with predicted fixed simulation - for perfect snapping we want to interpolate only if the state is Active (KCC stands within the platform trigger).
					// Otherwise the KCC could penetrate geometry while keeping Inactive state during platform-space => world-space transition, which is undesired.
					if (isInSimulation == true && toPlatform.State != EPlatformState.Active)
						continue;

					NetworkObject toPlatformObject = Runner.FindObject(toPlatform.Id);
					if (toPlatformObject == null)
						continue;

					// In following calculations we're interpolating between [world-space interpolated KCC position] and [platform-space interpolated KCC position].

					toPlatformObject.transform.GetPositionAndRotation(out Vector3 platformPosition, out Quaternion platformRotation);

					Vector3 kccPosition = data.TargetPosition;
					Vector3 toPosition  = platformPosition + platformRotation * toPlatform.KCCOffset;

					if (kcc.GetInterpolatedNetworkBufferPosition(out Vector3 interpolatedKCCPosition) == true)
					{
						kccPosition = interpolatedKCCPosition;
					}

					averagePosition += Vector3.Lerp(kccPosition, toPosition, alpha) * toPlatform.Alpha;
					averageAlpha    += toPlatform.Alpha;
				}
				else if (toPlatform.State == EPlatformState.None)
				{
					if (fromPlatform.State == EPlatformState.None)
						continue;

					// Only From is valid => the KCC just left the platform.

					// Render interpolated KCC with predicted fixed simulation - for perfect snapping we want to interpolate only if the state is Active (KCC stands within the platform trigger).
					// Otherwise the KCC could penetrate geometry while keeping Inactive state during platform-space => world-space transition, which is undesired.
					if (isInSimulation == true && fromPlatform.State != EPlatformState.Active)
						continue;

					NetworkObject fromPlatformObject = Runner.FindObject(fromPlatform.Id);
					if (fromPlatformObject == null)
						continue;

					// In following calculations we're interpolating between [world-space interpolated KCC position] and [platform-space interpolated KCC position].

					fromPlatformObject.transform.GetPositionAndRotation(out Vector3 platformPosition, out Quaternion platformRotation);

					Vector3 fromPosition = platformPosition + platformRotation * fromPlatform.KCCOffset;
					Vector3 kccPosition  = data.TargetPosition;

					if (kcc.GetInterpolatedNetworkBufferPosition(out Vector3 interpolatedKCCPosition) == true)
					{
						kccPosition = interpolatedKCCPosition;
					}

					averagePosition += Vector3.Lerp(fromPosition, kccPosition, alpha) * fromPlatform.Alpha;
					averageAlpha    += fromPlatform.Alpha;
				}
				else
				{
					if (toPlatform.Id != fromPlatform.Id)
						continue;

					// From and To are same platform objects.

					// Render interpolated KCC with predicted fixed simulation - for perfect snapping we want to interpolate only if the state is Active (KCC stands within the platform trigger).
					// Otherwise the KCC could penetrate geometry while keeping Inactive state during platform-space => world-space transition, which is undesired.
					if (isInSimulation == true && (fromPlatform.State != EPlatformState.Active || toPlatform.State != EPlatformState.Active))
						continue;

					NetworkObject platformObject = Runner.FindObject(toPlatform.Id);
					if (platformObject == null)
						continue;

					// In following calculations we're interpolating between two platform-space interpolated KCC positions.

					float   platformAlpha          = Mathf.Lerp(fromPlatform.Alpha, toPlatform.Alpha, alpha);
					Vector3 platformRelativeOffset = Vector3.Lerp(fromPlatform.KCCOffset, toPlatform.KCCOffset, alpha);

					platformObject.transform.GetPositionAndRotation(out Vector3 platformPosition, out Quaternion platformRotation);

					averagePosition += (platformPosition + platformRotation * platformRelativeOffset) * platformAlpha;
					averageAlpha    += platformAlpha;
				}
			}

			if (averageAlpha < 0.001f)
				return false;

			// Final position equals to weighted average of snap-interpolated KCC positions.
			data.TargetPosition = averagePosition / averageAlpha;
			return true;
		}

		// HELPER METHODS

		private Platform GetPlatform(int index, bool isInFixedUpdate)
		{
			return isInFixedUpdate == true ? _state.Platforms.Get(index) : _renderPlatforms[index];
		}

		private void SetPlatform(int index, Platform platform, bool isInFixedUpdate)
		{
			if (isInFixedUpdate == true)
			{
				_state.Platforms.Set(index, platform);
			}

			_renderPlatforms[index] = platform;
		}

		private void SynchronizeRenderPlatforms()
		{
			for (int i = 0, count = _state.Platforms.Length; i < count; ++i)
			{
				_renderPlatforms[i] = _state.Platforms.Get(i);
			}
		}

		// DATA STRUCTURES

		public enum EPlatformState
		{
			None     = 0,
			Active   = 1,
			Inactive = 2,
		}

		public struct Platform : INetworkStruct
		{
			public NetworkId      Id;
			public EPlatformState State;
			public float          Alpha;
			public Vector3        Position;
			public Quaternion     Rotation;
			public Vector3        KCCOffset;
		}

		public struct ProcessorState : INetworkStruct
		{
			public int Flags;
			[Networked][Capacity(MAX_PLATFORMS)]
			public NetworkArray<Platform> Platforms => default;

			public bool IsActive { get => (Flags & 1) == 1; set { if (value) { Flags |= 1; } else { Flags &= ~1; } } }
		}
	}
}
