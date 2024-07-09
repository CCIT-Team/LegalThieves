namespace Fusion.Addons.KCC
{
	using System;
	using System.Collections.Generic;
	using Unity.Profiling;
	using UnityEngine;

	using ReadOnlyProcessors = System.Collections.ObjectModel.ReadOnlyCollection<IKCCProcessor>;

	/// <summary>
	/// Fusion kinematic character controller component.
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public sealed partial class KCC : NetworkTRSP, IAfterSpawned, IAfterClientPredictionReset, IBeforeTick, IAfterTick
	{
		// CONSTANTS

		public const int    CACHE_SIZE            = 64;
		public const int    HISTORY_SIZE          = 60;
		public const string TRACING_SCRIPT_DEFINE = "KCC_TRACE";

		// PRIVATE MEMBERS

		[SerializeField]
		private KCCSettings         _settings = new KCCSettings();

		private Transform           _transform;
		private Rigidbody           _rigidbody;
		private bool                _isSpawned;
		private bool                _isInFixedUpdate;
		private bool                _hasManualUpdate;
		private KCCDebug            _debug                 = new KCCDebug();
		private KCCCollider         _collider              = new KCCCollider();
		private KCCData             _fixedData             = new KCCData();
		private KCCData             _renderData            = new KCCData();
		private KCCData[]           _historyData           = new KCCData[HISTORY_SIZE];
		private KCCSettings         _defaultSettings       = new KCCSettings();
		private KCCOverlapInfo      _extendedOverlapInfo   = new KCCOverlapInfo(CACHE_SIZE);
		private KCCOverlapInfo      _trackOverlapInfo      = new KCCOverlapInfo(CACHE_SIZE);
		private List<Collider>      _childColliders        = new List<Collider>();
		private RaycastHit[]        _raycastHits           = new RaycastHit[CACHE_SIZE];
		private Collider[]          _hitColliders          = new Collider[CACHE_SIZE];
		private Collider[]          _addColliders          = new Collider[CACHE_SIZE];
		private Collider[]          _removeColliders       = new Collider[CACHE_SIZE];
		private KCCCollision[]      _removeCollisions      = new KCCCollision[CACHE_SIZE];
		private KCCResolver         _resolver              = new KCCResolver(CACHE_SIZE);
		private EKCCFeatures        _activeFeatures        = EKCCFeatures.None;
		private List<KCCStageInfo>  _activeStages          = new List<KCCStageInfo>();
		private IKCCProcessor[]     _cachedProcessors      = new IKCCProcessor[CACHE_SIZE * 2];
		private int                 _cachedProcessorCount  = 0;
		private IKCCProcessor[]     _stageProcessors       = new IKCCProcessor[CACHE_SIZE * 2];
		private int                 _stageProcessorCount   = 0;
		private BeginMove           _beginMove             = new BeginMove();
		private PrepareData         _prepareData           = new PrepareData();
		private AfterMoveStep       _afterMoveStep         = new AfterMoveStep();
		private EndMove             _endMove               = new EndMove();
		private List<IKCCProcessor> _localProcessors       = new List<IKCCProcessor>();
		private ReadOnlyProcessors  _localROProcessors;
		private int                 _lastPredictedFixedTick;
		private int                 _lastPredictedRenderFrame;
		private int                 _lastPredictedLookRotationFrame;
		private float               _lastRenderTime;
		private Vector3             _lastRenderPosition;
		private Vector3             _lastAntiJitterPosition;
		private Collider            _lastNonNetworkedCollider;
		private Vector3             _predictionError;

		private event Action<KCC>   _onSpawn;

		private static ProfilerMarker _fixedUpdateMarker   = new ProfilerMarker("KCC.FixedUpdate");
		private static ProfilerMarker _renderUpdateMarker  = new ProfilerMarker("KCC.RenderUpdate");
		private static ProfilerMarker _restoreStateMarker  = new ProfilerMarker("KCC.RestoreState");
		private static ProfilerMarker _afterTickMarker     = new ProfilerMarker("KCC.AfterTick");
		private static ProfilerMarker _simulatedMoveMarker = new ProfilerMarker("Simulated Move");
		private static ProfilerMarker _interpolationMarker = new ProfilerMarker("Interpolation");

		// PUBLIC METHODS

		/// <summary>
		/// Immediately synchronize Transform and Rigidbody based on current state.
		/// </summary>
		public void SynchronizeTransform(bool synchronizePosition, bool synchronizeRotation, bool allowAntiJitter = true)
		{
			if (IsInFixedUpdate == true)
			{
				allowAntiJitter = false;
			}

			SynchronizeTransform(Data, synchronizePosition, synchronizeRotation, allowAntiJitter);
		}

		/// <summary>
		/// Refresh child colliders list, used for collision filtering.
		/// Child colliders are ignored completely, triggers are treated as valid collision.
		/// </summary>
		public void RefreshChildColliders()
		{
			_childColliders.Clear();

			GetComponentsInChildren(true, _childColliders);

			int currentIndex = 0;
			int lastIndex    = _childColliders.Count - 1;

			while (currentIndex <= lastIndex)
			{
				Collider childCollider = _childColliders[currentIndex];
				if (childCollider.isTrigger == true || childCollider == _collider.Collider)
				{
					_childColliders[currentIndex] = _childColliders[lastIndex];
					_childColliders.RemoveAt(lastIndex);

					--lastIndex;
				}
				else
				{
					++currentIndex;
				}
			}
		}

		/// <summary>
		/// Returns fixed data for specific tick in history. Default history size is 60 ticks.
		/// </summary>
		public KCCData GetHistoryData(int tick)
		{
			if (tick < 0)
				return null;

			KCCData data = _historyData[tick % HISTORY_SIZE];
			if (data != null && data.Tick == tick)
				return data;

			return null;
		}

		/// <summary>
		/// Controls whether update methods are driven by default Fusion methods or called manually using <c>ManualFixedUpdate()</c> and <c>ManualRenderUpdate()</c>.
		/// </summary>
		public void SetManualUpdate(bool hasManualUpdate)
		{
			_hasManualUpdate = hasManualUpdate;
		}

		/// <summary>
		/// Invokes callback when the KCC is spawned.
		/// If the KCC is already spawned, the callback is invoked immediately.
		/// </summary>
		public void InvokeOnSpawn(Action<KCC> callback)
		{
			if (_isSpawned == true)
			{
				callback(this);
			}
			else
			{
				_onSpawn -= callback;
				_onSpawn += callback;
			}
		}

		/// <summary>
		/// Manual fixed update execution, <c>SetManualUpdate(true)</c> must be called prior usage.
		/// </summary>
		public void ManualFixedUpdate()
		{
			if (_isSpawned == false)
				return;
			if (_hasManualUpdate == false)
				throw new InvalidOperationException($"[{name}] Manual update is not set!");

			_fixedUpdateMarker.Begin();
			OnFixedUpdateInternal();
			_fixedUpdateMarker.End();
		}

		/// <summary>
		/// Manual render update execution, <c>SetManualUpdate(true)</c> must be called prior usage.
		/// </summary>
		public void ManualRenderUpdate()
		{
			if (_isSpawned == false)
				return;
			if (_hasManualUpdate == false)
				throw new InvalidOperationException($"[{name}] Manual update is not set!");

			_renderUpdateMarker.Begin();
			OnRenderUpdateInternal();
			_renderUpdateMarker.End();
		}

		/// <summary>
		/// Explicit interpolation on demand. Implicit interpolation in render update is not skipped!
		/// </summary>
		/// <param name="alpha">Custom interpolation alpha. Valid range is 0.0 - 1.0, otherwise default value from <c>TryGetSnapshotsBuffers()</c> is used.</param>
		public void Interpolate(float alpha = -1.0f)
		{
			if (_isSpawned == false)
				return;

			RenderSource    renderSource    = RenderSource.Interpolated;
			RenderTimeframe renderTimeframe = GetInterpolationTimeframe();

			Interpolate(renderSource, renderTimeframe, alpha);
		}

		/// <summary>
		/// Explicit interpolation on demand. Implicit interpolation in render update is not skipped!
		/// </summary>
		/// <param name="renderSource">Custom render source.</param>
		/// <param name="renderTimeframe">Custom render timeframe.</param>
		/// <param name="alpha">Custom interpolation alpha. Valid range is 0.0 - 1.0, otherwise default value from <c>TryGetSnapshotsBuffers()</c> is used.</param>
		public void Interpolate(RenderSource renderSource, RenderTimeframe renderTimeframe, float alpha = -1.0f)
		{
			if (_isSpawned == false)
				return;

			InterpolateNetworkData(renderSource, renderTimeframe, alpha);

			if (_renderData.IsActive == true)
			{
				CacheProcessors(_renderData);
				InvokeOnInterpolate(_renderData);
				SynchronizeTransform(_renderData, true, true, false);
			}
		}

		// MonoBehaviour INTERFACE

		private void Awake()
		{
			_transform = transform;

			_rigidbody = GetComponent<Rigidbody>();
			_rigidbody.isKinematic = true;

			_localROProcessors = new ReadOnlyProcessors(_localProcessors);

			RefreshCollider();
		}

		private void OnDestroy()
		{
			SetDefaults(true);
		}

		private void OnDrawGizmosSelected()
		{
			if (_settings == null)
				return;

			float radius = Mathf.Max(0.01f, _settings.Radius);
			float height = Mathf.Max(radius * 2.0f, _settings.Height);

			Vector3 basePosition = transform.position;

			Color gizmosColor = Gizmos.color;

			Vector3 baseLow     = basePosition + Vector3.up * radius;
			Vector3 baseHigh    = basePosition + Vector3.up * (height - radius);
			Vector3 offsetFront = Vector3.forward * radius;
			Vector3 offsetBack  = Vector3.back    * radius;
			Vector3 offsetLeft  = Vector3.left    * radius;
			Vector3 offsetRight = Vector3.right   * radius;

			Gizmos.color = Color.green;

			Gizmos.DrawWireSphere(baseLow, radius);
			Gizmos.DrawWireSphere(baseHigh, radius);

			Gizmos.DrawLine(baseLow + offsetFront, baseHigh + offsetFront);
			Gizmos.DrawLine(baseLow + offsetBack,  baseHigh + offsetBack);
			Gizmos.DrawLine(baseLow + offsetLeft,  baseHigh + offsetLeft);
			Gizmos.DrawLine(baseLow + offsetRight, baseHigh + offsetRight);

			if (_settings.Extent > 0.0f)
			{
				float extendedRadius = radius + _settings.Extent;

				Gizmos.color = Color.yellow;

				Gizmos.DrawWireSphere(baseLow, extendedRadius);
				Gizmos.DrawWireSphere(baseHigh, extendedRadius);
			}

			Gizmos.color = gizmosColor;
		}

		// NetworkBehaviour INTERFACE

		public override int? DynamicWordCount => GetNetworkDataWordCount();

		public override void Spawned()
		{
			Trace(nameof(Spawned));

			if (_isSpawned == true)
				throw new InvalidOperationException($"[{name}] KCC is already spawned!");

			_defaultSettings.CopyFromOther(_settings);

			SetDefaults(false);

			_isSpawned       = true;
			_isInFixedUpdate = true;

			KCCUtility.GetClampedLookRotationAngles(_transform.rotation, out float lookPitch, out float lookYaw);

			_fixedData = new KCCData();
			_fixedData.Frame           = Time.frameCount;
			_fixedData.Tick            = Runner.Tick.Raw;
			_fixedData.Time            = Runner.SimulationTime;
			_fixedData.DeltaTime       = Runner.DeltaTime;
			_fixedData.UpdateDeltaTime = _fixedData.DeltaTime;
			_fixedData.Gravity         = Physics.gravity;
			_fixedData.MaxGroundAngle  = 60.0f;
			_fixedData.MaxWallAngle    = 5.0f;
			_fixedData.MaxHangAngle    = 30.0f;
			_fixedData.BasePosition    = _transform.position;
			_fixedData.DesiredPosition = _transform.position;
			_fixedData.TargetPosition  = _transform.position;
			_fixedData.LookPitch       = lookPitch;
			_fixedData.LookYaw         = lookYaw;

			_lastPredictedFixedTick         = _fixedData.Tick;
			_lastPredictedRenderFrame       = _fixedData.Frame;
			_lastPredictedLookRotationFrame = _fixedData.Frame;

			if (Object.HasStateAuthority == false)
			{
				ReadNetworkData();
				SynchronizeTransform(_fixedData, true, true, false);
			}

			_renderData = new KCCData();
			_renderData.CopyFromOther(_fixedData);

			_lastRenderPosition     = _renderData.TargetPosition;
			_lastAntiJitterPosition = _renderData.TargetPosition;

			RefreshCollider();
			RefreshChildColliders();

			UnityEngine.Object[] processors = _settings.Processors;
			if (processors != null)
			{
				for (int i = 0, count = processors.Length; i < count; ++i)
				{
					UnityEngine.Object processorObject = processors[i];
					if (processorObject == null)
					{
						KCCUtility.Log(this, this, default, EKCCLogType.Warning, $"Missing processor object - {nameof(KCCSettings)}.{nameof(KCCSettings.Processors)} at index {i}");
						continue;
					}

					if (KCCUtility.ResolveProcessor(processorObject, out IKCCProcessor processor, out GameObject gameObject, out Component component, out ScriptableObject scriptableObject) == false)
					{
						KCCUtility.Log(this, processorObject, default, EKCCLogType.Error, $"Failed to resolve {nameof(IKCCProcessor)} in {processorObject.name} ({processorObject.GetType().FullName}) - {nameof(KCCSettings)}.{nameof(KCCSettings.Processors)} at index {i}");
						continue;
					}

					AddLocalProcessor(processor);
				}
			}

			if (_onSpawn != null)
			{
				try { _onSpawn(this); } catch (Exception exception) { UnityEngine.Debug.LogException(exception); }

				_onSpawn = null;
			}
		}

		public override void Despawned(NetworkRunner runner, bool hasState)
		{
			if (_isSpawned == false)
				return;

			Trace(nameof(Despawned));

			ForceRemoveAllCollisions(_fixedData);
			ForceRemoveAllModifiers(_fixedData);

			while (_localProcessors.Count > 0)
			{
				RemoveLocalProcessor(_localProcessors[_localProcessors.Count - 1]);
			}

			SetDefaults(true);
		}

		public override void FixedUpdateNetwork()
		{
			if (_hasManualUpdate == true)
				return;

			_fixedUpdateMarker.Begin();
			OnFixedUpdateInternal();
			_fixedUpdateMarker.End();
		}

		public override void Render()
		{
			if (_hasManualUpdate == true)
				return;

			_renderUpdateMarker.Begin();
			OnRenderUpdateInternal();
			_renderUpdateMarker.End();
		}

		// IAfterSpawned INTERFACE

		void IAfterSpawned.AfterSpawned()
		{
			_renderData.CopyFromOther(_fixedData);

			if (Object.IsInSimulation == true)
			{
				WriteNetworkData();
			}

			_isInFixedUpdate = false;
		}

		// IAfterClientPredictionReset INTERFACE

		void IAfterClientPredictionReset.AfterClientPredictionReset()
		{
			int latestServerTick = Runner.LatestServerTick;

			Trace(nameof(IAfterClientPredictionReset.AfterClientPredictionReset), $"Tick:{latestServerTick}");

			_restoreStateMarker.Begin();

			KCCData historyData = _historyData[latestServerTick % HISTORY_SIZE];
			if (historyData != null && historyData.Tick == latestServerTick)
			{
				_fixedData.CopyFromOther(historyData);
				_fixedData.Frame = Time.frameCount;
			}

			ReadNetworkData();

			if (historyData != default)
			{
				RestoreHistoryData(historyData);
			}

			RefreshCollider();

			if (_fixedData.IsActive == true)
			{
				SynchronizeTransform(_fixedData, true, true, false);
			}

			_lastPredictedFixedTick = latestServerTick;

			_restoreStateMarker.End();
		}

		// IBeforeTick INTERFACE

		void IBeforeTick.BeforeTick()
		{
			_isInFixedUpdate = true;

			_fixedData.Frame           = Time.frameCount;
			_fixedData.Tick            = Runner.Tick.Raw;
			_fixedData.Alpha           = 0.0f;
			_fixedData.Time            = Runner.SimulationTime;
			_fixedData.DeltaTime       = Runner.DeltaTime;
			_fixedData.UpdateDeltaTime = _fixedData.DeltaTime;

			Trace(nameof(IBeforeTick.BeforeTick), "[Fixed Update Initialization]", $"Time:{_fixedData.Time:F6}", $"DeltaTime:{_fixedData.DeltaTime:F6}", $"Alpha:{_fixedData.Alpha:F4}", $"HasInputAuthority:{Object.HasInputAuthority}", $"HasStateAuthority:{Object.HasStateAuthority}");
		}

		// IAfterTick INTERFACE

		void IAfterTick.AfterTick()
		{
			Trace(nameof(IAfterTick.AfterTick));

			_afterTickMarker.Begin();

			PublishFixedData(true, true);
			WriteNetworkData();

			_isInFixedUpdate = false;

			_afterTickMarker.End();
		}

		// PRIVATE METHODS

		private void OnFixedUpdateInternal()
		{
			if (IsInFixedUpdate == false)
				throw new InvalidOperationException($"[{name}] KCC fixed update called from render update! This is not allowed.");

			Trace(nameof(OnFixedUpdateInternal));

			_debug.BeforePredictedFixedMove(this);

			RefreshCollider();

			_simulatedMoveMarker.Begin();

			MovePredicted(_fixedData);

			if (_fixedData.IsActive == true)
			{
				SynchronizeTransform(_fixedData, true, true, false);
			}

			PublishFixedData(false, true);

			_lastPredictedFixedTick = _fixedData.Tick;

			_simulatedMoveMarker.End();

			_debug.AfterPredictedFixedMove(this);
		}

		private void OnRenderUpdateInternal()
		{
			if (IsInFixedUpdate == true)
				throw new InvalidOperationException($"[{name}] KCC render update called from fixed update! This is not allowed.");

			Trace(nameof(OnRenderUpdateInternal));

			int   frame     = Time.frameCount;
			float deltaTime = Runner.DeltaTime;

			_renderData.Frame = frame;

			if (Object.IsInSimulation == true)
			{
				_simulatedMoveMarker.Begin();

				_renderData.Tick  = Runner.Tick;
				_renderData.Alpha = Runner.LocalAlpha;

				float previousTime = _renderData.Time;

				_renderData.Time = Runner.SimulationTime + _renderData.Alpha * deltaTime;

				if (IsInterpolatingInRenderUpdate == true)
				{
					_renderData.Tick -= 1;
					_renderData.Time -= deltaTime;

					if (_renderData.Frame == _fixedData.Frame)
					{
						previousTime -= deltaTime;
					}
				}

				_renderData.DeltaTime       = _renderData.Time - previousTime;
				_renderData.UpdateDeltaTime = _renderData.DeltaTime;

				UpdatePredictionError();
#if UNITY_EDITOR
				if (_debug.ShowPath == true)
				{
					if (_renderData.Frame == _fixedData.Frame)
					{
						UnityEngine.Debug.DrawLine(_fixedData.TargetPosition, _renderData.TargetPosition, KCCDebug.FixedToRenderPathColor, _debug.DisplayTime);
					}
					else
					{
						UnityEngine.Debug.DrawLine(_lastRenderPosition, _renderData.TargetPosition, KCCDebug.PredictionCorrectionColor, _debug.DisplayTime);
					}
				}
#endif
				if (IsPredictingInRenderUpdate == true)
				{
					MovePredicted(_renderData);

					_lastPredictedRenderFrame       = frame;
					_lastPredictedLookRotationFrame = frame;
				}
				else
				{
					MoveInterpolated(_renderData);

					if (_settings.ForcePredictedLookRotation == true)
					{
						_lastPredictedLookRotationFrame = frame;
					}
				}

				if (_renderData.IsActive == true)
				{
					SynchronizeTransform(_renderData, true, true, true);
				}

				_simulatedMoveMarker.End();
			}
			else
			{
				_interpolationMarker.Begin();

				if (_settings.ProxyInterpolationMode == EKCCInterpolationMode.Transform)
				{
					InterpolateNetworkTransform();
				}
				else
				{
					_fixedData.Frame           = frame;
					_fixedData.Tick            = Object.LastReceiveTick;
					_fixedData.Alpha           = 1.0f;
					_fixedData.Time            = _fixedData.Tick * deltaTime;
					_fixedData.DeltaTime       = deltaTime;
					_fixedData.UpdateDeltaTime = deltaTime;

					ReadNetworkData();

					InterpolateNetworkData(RenderSource.Interpolated, RenderTimeframe.Remote);

					if (_renderData.IsActive == true)
					{
						RefreshCollider();
						CacheProcessors(_renderData);
						InvokeOnInterpolate(_renderData);
						SynchronizeTransform(_renderData, true, true, false);
					}
				}

				_interpolationMarker.End();
			}

			_lastRenderPosition = _renderData.TargetPosition;
			_lastRenderTime     = _renderData.Time;

			_debug.AfterRenderUpdate(this);
		}

		private void UpdatePredictionError()
		{
			if (_activeFeatures.Has(EKCCFeature.PredictionCorrection) == true && _renderData.Frame == _fixedData.Frame)
			{
				KCCData current = GetHistoryData(_renderData.Tick);
				if (current != null && _lastRenderTime <= current.Time)
				{
					for (int i = 0; i < 5; ++i)
					{
						KCCData previous = GetHistoryData(current.Tick - 1);
						if (previous == null)
							break;

						if (_lastRenderTime >= previous.Time)
						{
							if (current.HasTeleported == true || previous.HasTeleported == true)
							{
								_predictionError = default;
								return;
							}

							float deltaTime = current.Time - previous.Time;
							if (deltaTime <= 0.000001f)
							{
								_predictionError = default;
								return;
							}

							float lastRenderAlpha = (_lastRenderTime - previous.Time) / deltaTime;
							Vector3 expectedRenderPosition = Vector3.Lerp(previous.TargetPosition, current.TargetPosition, lastRenderAlpha);
#if UNITY_EDITOR
							if (_debug.ShowPath == true)
							{
								UnityEngine.Debug.DrawLine(expectedRenderPosition, _lastRenderPosition, KCCDebug.PredictionErrorColor, _debug.DisplayTime);
							}
#endif
							_predictionError = _lastRenderPosition - expectedRenderPosition;
							if (_predictionError.sqrMagnitude >= _settings.TeleportThreshold * _settings.TeleportThreshold)
							{
								_predictionError = default;
								return;
							}

							_predictionError = Vector3.Lerp(_predictionError, Vector3.zero, _settings.PredictionCorrectionSpeed * Time.deltaTime);

							_renderData.BasePosition    += _predictionError;
							_renderData.DesiredPosition += _predictionError;
							_renderData.TargetPosition  += _predictionError;

							return;
						}

						current = previous;
					}
				}
			}

			if (_predictionError.IsAlmostZero(0.000001f) == true)
			{
				_predictionError = default;
			}
			else
			{
				_renderData.BasePosition    -= _predictionError;
				_renderData.DesiredPosition -= _predictionError;
				_renderData.TargetPosition  -= _predictionError;

				_predictionError = Vector3.Lerp(_predictionError, Vector3.zero, _settings.PredictionCorrectionSpeed * Time.deltaTime);

				_renderData.BasePosition    += _predictionError;
				_renderData.DesiredPosition += _predictionError;
				_renderData.TargetPosition  += _predictionError;
			}
		}

		private void MovePredicted(KCCData data)
		{
			_activeFeatures = _settings.Features;

			float   baseTime            = data.Time;
			float   baseDeltaTime       = data.DeltaTime;
			Vector3 basePosition        = data.TargetPosition;
			Vector3 desiredPosition     = data.TargetPosition;
			bool    wasGrounded         = data.IsGrounded;
			bool    wasSteppingUp       = data.IsSteppingUp;
			bool    wasSnappingToGround = data.IsSnappingToGround;

			data.DeltaTime       = baseDeltaTime;
			data.BasePosition    = basePosition;
			data.DesiredPosition = desiredPosition;

			if (data.IsActive == false)
			{
				data.ClearTransientProperties();
				ForceRemoveAllCollisions(data);
				ForceRemoveAllHits(data);
				return;
			}

			CacheProcessors(data);
			SetBaseProperties(data);

			ExecuteStageInternal<IBeginMove, BeginMove>(_beginMove, data);

			if (data.IsActive == false)
			{
				data.ClearTransientProperties();
				ForceRemoveAllCollisions(data);
				ForceRemoveAllHits(data);

				ExecuteStageInternal<IEndMove, EndMove>(_endMove, data);

				return;
			}

			baseDeltaTime = data.DeltaTime;
			basePosition  = data.BasePosition;

			if (baseDeltaTime < KCCSettings.ExtrapolationDeltaTimeThreshold)
			{
				Vector3 extrapolationVelocity = data.DesiredVelocity;
				if (data.RealVelocity.sqrMagnitude <= extrapolationVelocity.sqrMagnitude)
				{
					extrapolationVelocity = data.RealVelocity;
				}

				desiredPosition = basePosition + extrapolationVelocity * baseDeltaTime;

				data.BasePosition    = basePosition;
				data.DesiredPosition = desiredPosition;
				data.TargetPosition  = desiredPosition;

				ExecuteStageInternal<IEndMove, EndMove>(_endMove, data);

				InvokeOnStay(data);

				return;
			}

			ExecuteStageInternal<IPrepareData, PrepareData>(_prepareData, data);

			ForceRemoveAllHits(data);

			float   pendingDeltaTime     = Mathf.Clamp01(baseDeltaTime);
			Vector3 pendingDeltaPosition = data.DesiredVelocity * pendingDeltaTime + data.ExternalDelta;

			desiredPosition = data.BasePosition + pendingDeltaPosition;

			data.DesiredPosition = desiredPosition;
			data.TargetPosition  = data.BasePosition;
			data.ExternalDelta   = default;

			bool    hasFinished           = false;
			float   radiusMultiplier      = Mathf.Clamp(_settings.CCDRadiusMultiplier, 0.25f, 0.75f);
			float   maxDeltaMagnitude     = _settings.Radius * (radiusMultiplier + 0.1f);
			float   optimalDeltaMagnitude = _settings.Radius * radiusMultiplier;
			Vector3 nonTeleportedPosition = data.TargetPosition;

			while (hasFinished == false && data.HasTeleported == false)
			{
				data.BasePosition = data.TargetPosition;

				float   consumeDeltaTime     = pendingDeltaTime;
				Vector3 consumeDeltaPosition = pendingDeltaPosition;

				if (_activeFeatures.Has(EKCCFeature.CCD) == true)
				{
					float consumeDeltaPositionMagnitude = consumeDeltaPosition.magnitude;
					if (consumeDeltaPositionMagnitude > maxDeltaMagnitude)
					{
						float deltaRatio = optimalDeltaMagnitude / consumeDeltaPositionMagnitude;

						consumeDeltaTime     *= deltaRatio;
						consumeDeltaPosition *= deltaRatio;
					}
					else
					{
						hasFinished = true;
					}
				}
				else
				{
					hasFinished = true;
				}

				pendingDeltaTime     -= consumeDeltaTime;
				pendingDeltaPosition -= consumeDeltaPosition;

				if (pendingDeltaTime <= 0.0f)
				{
					pendingDeltaTime = 0.0f;
				}

				data.Time                = baseTime - pendingDeltaTime;
				data.DeltaTime           = consumeDeltaTime;
				data.DesiredPosition     = data.BasePosition + consumeDeltaPosition;
				data.TargetPosition      = data.DesiredPosition;
				data.WasGrounded         = data.IsGrounded;
				data.WasSteppingUp       = data.IsSteppingUp;
				data.WasSnappingToGround = data.IsSnappingToGround;

				ProcessMoveStep(data);

				if (data.HasTeleported == false)
				{
					nonTeleportedPosition = data.TargetPosition;
				}

				UpdateCollisions(data, _trackOverlapInfo);

				if (data.HasTeleported == true)
				{
					UpdateHits(data, null, EKCCHitsOverlapQuery.New);
					UpdateCollisions(data, _trackOverlapInfo);
				}

				if (hasFinished == true && data.ExternalDelta.IsZero() == false)
				{
					pendingDeltaPosition += data.ExternalDelta;
					data.ExternalDelta = default;
					hasFinished = false;
				}
			}

			data.Time                = baseTime;
			data.DeltaTime           = baseDeltaTime;
			data.BasePosition        = basePosition;
			data.DesiredPosition     = desiredPosition;
			data.WasGrounded         = wasGrounded;
			data.WasSteppingUp       = wasSteppingUp;
			data.WasSnappingToGround = wasSnappingToGround;
			data.RealVelocity        = (nonTeleportedPosition - data.BasePosition) / data.DeltaTime;
			data.RealSpeed           = data.RealVelocity.magnitude;

			Vector3 targetPosition = data.TargetPosition;

			ExecuteStageInternal<IEndMove, EndMove>(_endMove, data);

			if (data.TargetPosition.IsEqual(targetPosition) == false)
			{
				UpdateHits(data, null, EKCCHitsOverlapQuery.New);
				UpdateCollisions(data, _trackOverlapInfo);
			}

			targetPosition = data.TargetPosition;

			InvokeOnStay(data);

			if (data.TargetPosition.IsEqual(targetPosition) == false)
			{
				UpdateHits(data, null, EKCCHitsOverlapQuery.New);
				UpdateCollisions(data, _trackOverlapInfo);
			}
		}

		private void MoveInterpolated(KCCData data)
		{
			if (data.IsActive == false)
				return;

			KCCData currentFixedData = _fixedData;
			if (currentFixedData.HasTeleported == false)
			{
				KCCData previousFixedData = GetHistoryData(currentFixedData.Tick - 1);
				if (previousFixedData != null)
				{
					float alpha = data.Alpha;

					data.BasePosition    = Vector3.Lerp(previousFixedData.BasePosition, currentFixedData.BasePosition, alpha) + _predictionError;
					data.DesiredPosition = Vector3.Lerp(previousFixedData.DesiredPosition, currentFixedData.DesiredPosition, alpha) + _predictionError;
					data.TargetPosition  = Vector3.Lerp(previousFixedData.TargetPosition, currentFixedData.TargetPosition, alpha) + _predictionError;
					data.RealVelocity    = Vector3.Lerp(previousFixedData.RealVelocity, currentFixedData.RealVelocity, alpha);
					data.RealSpeed       = Mathf.Lerp(previousFixedData.RealSpeed, currentFixedData.RealSpeed, alpha);

					if (_settings.ForcePredictedLookRotation == false)
					{
						data.LookPitch = Mathf.Lerp(previousFixedData.LookPitch, currentFixedData.LookPitch, alpha);
						data.LookYaw   = KCCUtility.InterpolateRange(previousFixedData.LookYaw, currentFixedData.LookYaw, -180.0f, 180.0f, alpha);
					}
				}
			}

			CacheProcessors(data);
			InvokeOnInterpolate(data);
		}

		private void SetBaseProperties(KCCData data)
		{
			data.HasTeleported       = default;
			data.MaxPenetrationSteps = _settings.MaxPenetrationSteps;

			if (data.Frame == _fixedData.Frame)
			{
				data.JumpFrames = default;
			}
		}

		private void ProcessMoveStep(KCCData data)
		{
			data.IsGrounded         = default;
			data.IsSteppingUp       = default;
			data.IsSnappingToGround = default;
			data.GroundNormal       = default;
			data.GroundTangent      = default;
			data.GroundPosition     = default;
			data.GroundDistance     = default;
			data.GroundAngle        = default;

			ForceRemoveAllHits(data);

			bool hasJumped = data.JumpFrames > 0;

			if (_settings.CollisionLayerMask != 0 && _collider.IsSpawned == true)
			{
				float                baseOverlapQueryExtent = _settings.Radius;
				EKCCHitsOverlapQuery baseHitsOverlapQuery   = EKCCHitsOverlapQuery.Default;

				if (_settings.ForceSingleOverlapQuery == true)
				{
					baseOverlapQueryExtent = _settings.Extent;
					baseHitsOverlapQuery   = EKCCHitsOverlapQuery.Reuse;
				}

				CapsuleOverlap(_extendedOverlapInfo, data, data.TargetPosition, _settings.Radius, _settings.Height, baseOverlapQueryExtent, _settings.CollisionLayerMask, QueryTriggerInteraction.Collide);

				data.TargetPosition = ResolvePenetration(_extendedOverlapInfo, data, data.BasePosition, data.TargetPosition, hasJumped == false, data.MaxPenetrationSteps, 3, true);

				UpdateHits(data, _extendedOverlapInfo, baseHitsOverlapQuery);
			}

			if (hasJumped == true)
			{
				data.IsGrounded = false;
			}

			_afterMoveStep.OverlapInfo.CopyFromOther(_extendedOverlapInfo);

			ExecuteStageInternal<IAfterMoveStep, AfterMoveStep>(_afterMoveStep, data);
		}

		private void SynchronizeTransform(KCCData data, bool synchronizePosition, bool synchronizeRotation, bool allowAntiJitter)
		{
			if (synchronizePosition == true)
			{
				Vector3 targetPosition = data.TargetPosition;

				_rigidbody.position = targetPosition;

				if (allowAntiJitter == true && _activeFeatures.Has(EKCCFeature.AntiJitter) == true && _settings.AntiJitterDistance.IsZero() == false)
				{
					Vector3 targetDelta = targetPosition - _lastAntiJitterPosition;
					if (targetDelta.sqrMagnitude < _settings.TeleportThreshold * _settings.TeleportThreshold)
					{
						targetPosition = _lastAntiJitterPosition;

						float distanceY = Mathf.Abs(targetDelta.y);
						if (distanceY > 0.000001f && distanceY > _settings.AntiJitterDistance.y)
						{
							targetPosition.y += targetDelta.y * Mathf.Clamp01((distanceY - _settings.AntiJitterDistance.y) / distanceY);
						}

						Vector3 targetDeltaXZ = targetDelta.OnlyXZ();

						float distanceXZ = Vector3.Magnitude(targetDeltaXZ);
						if (distanceXZ > 0.000001f && distanceXZ > _settings.AntiJitterDistance.x)
						{
							targetPosition += targetDeltaXZ * Mathf.Clamp01((distanceXZ - _settings.AntiJitterDistance.x) / distanceXZ);
						}
					}

					_lastAntiJitterPosition = targetPosition;
				}

				if (synchronizeRotation == true)
				{
					_transform.SetPositionAndRotation(targetPosition, data.TransformRotation);
				}
				else
				{
					_transform.position = targetPosition;
				}
			}
			else
			{
				if (synchronizeRotation == true)
				{
					_transform.rotation = data.TransformRotation;
				}
			}
		}

		private void RefreshCollider()
		{
			if (_settings.Shape == EKCCShape.None)
			{
				_collider.Destroy();
				return;
			}

			_settings.Radius = Mathf.Max(0.01f, _settings.Radius);
			_settings.Height = Mathf.Max(_settings.Radius * 2.0f, _settings.Height);
			_settings.Extent = Mathf.Max(0.0f, _settings.Extent);

			_collider.Update(this);
		}

		private void SetDefaults(bool cleanup)
		{
			_debug.SetDefaults();

			_fixedData.Clear();
			_renderData.Clear();
			_historyData.Clear();
			_afterMoveStep.OverlapInfo.Reset(true);
			_extendedOverlapInfo.Reset(true);
			_trackOverlapInfo.Reset(true);
			_childColliders.Clear();
			_raycastHits.Clear();
			_hitColliders.Clear();
			_addColliders.Clear();
			_removeColliders.Clear();
			_removeCollisions.Clear();
			_activeStages.Clear();
			_cachedProcessors.Clear();
			_stageProcessors.Clear();
			_localProcessors.Clear();

			_cachedProcessorCount = default;
			_stageProcessorCount  = default;

			_rigidbody.isKinematic = true;

			_activeFeatures                 = default;
			_lastPredictedFixedTick         = default;
			_lastPredictedRenderFrame       = default;
			_lastPredictedLookRotationFrame = default;
			_lastRenderTime                 = default;
			_lastRenderPosition             = default;
			_lastAntiJitterPosition         = default;
			_lastNonNetworkedCollider       = default;
			_predictionError                = default;

			if (cleanup == true)
			{
				_isSpawned       = default;
				_isInFixedUpdate = default;
				_hasManualUpdate = default;

				_settings.CopyFromOther(_defaultSettings);

				_collider.Destroy();

				_onSpawn = default;

				ResolveCollision      = default;
				OnCollisionEnter      = default;
				OnCollisionExit       = default;
				GetExternalProcessors = default;
			}
		}

		private void PublishFixedData(bool render, bool history)
		{
			if (render == true)
			{
				_renderData.CopyFromOther(_fixedData);
			}

			if (history == true)
			{
				KCCData historyData = _historyData[_fixedData.Tick % HISTORY_SIZE];
				if (historyData == null)
				{
					historyData = new KCCData();
					_historyData[_fixedData.Tick % HISTORY_SIZE] = historyData;
				}

				historyData.CopyFromOther(_fixedData);
			}
		}
	}
}
