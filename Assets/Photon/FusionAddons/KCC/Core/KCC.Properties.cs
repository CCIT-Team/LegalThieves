namespace Fusion.Addons.KCC
{
	using System.Collections.ObjectModel;
	using UnityEngine;

	#pragma warning disable 0109

	// This file contains public properties.
	public partial class KCC
	{
		// PUBLIC MEMBERS

		/// <summary>
		/// <c>True</c> if the <c>KCC</c> is already initialized - Spawned() has been called.
		/// </summary>
		public bool IsSpawned => _isSpawned;

		/// <summary>
		/// Controls execution of the KCC.
		/// </summary>
		public bool IsActive => Data.IsActive;

		/// <summary>
		/// Returns <c>FixedData</c> if in fixed update, otherwise <c>RenderData</c>.
		/// </summary>
		public new KCCData Data => IsInFixedUpdate == true ? _fixedData : _renderData;

		/// <summary>
		/// Returns <c>KCCData</c> instance used for calculations in fixed update.
		/// </summary>
		public KCCData FixedData => _fixedData;

		/// <summary>
		/// Returns <c>KCCData</c> instance used for calculations in render update.
		/// </summary>
		public KCCData RenderData => _renderData;

		/// <summary>
		/// Basic <c>KCC</c> settings. These settings are reset to default when <c>Initialize()</c> or <c>Deinitialize()</c> is called.
		/// </summary>
		public KCCSettings Settings => _settings;

		/// <summary>
		/// Used for debugging - logs, drawings, statistics.
		/// </summary>
		public KCCDebug Debug => _debug;

		/// <summary>
		/// Reference to cached <c>Transform</c> component.
		/// </summary>
		public Transform Transform => _transform;

		/// <summary>
		/// Reference to <c>KCC</c> collider. Can be null if <c>Settings.Shape</c> is set to <c>EKCCShape.None</c>.
		/// </summary>
		public CapsuleCollider Collider => _collider.Collider;

		/// <summary>
		/// Reference to attached <c>Rigidbody</c> component.
		/// </summary>
		public Rigidbody Rigidbody => _rigidbody;

		/// <summary>
		/// Features the <c>KCC</c> is executing during update.
		/// </summary>
		public EKCCFeatures ActiveFeatures => _activeFeatures;

		/// <summary>
		/// Controls whether update methods are driven by default Unity/Fusion methods or called manually using <c>ManualFixedUpdate()</c> and <c>ManualRenderUpdate()</c>.
		/// </summary>
		public bool HasManualUpdate => _hasManualUpdate;

		/// <summary>
		/// <c>True</c> if the <c>KCC</c> is in fixed update. This can be used to skip logic in render.
		/// </summary>
		public bool IsInFixedUpdate => _isInFixedUpdate == true || (_isSpawned == true && Runner.Stage != default);

		/// <summary>
		/// <c>True</c> if the current fixed update is forward.
		/// </summary>
		public bool IsInForwardUpdate => _isSpawned == true && Runner.Stage != default && Runner.IsForward == true;

		/// <summary>
		/// <c>True</c> if the current fixed update is resimulation.
		/// </summary>
		public bool IsInResimulationUpdate => _isSpawned == true && Runner.Stage != default && Runner.IsResimulation == true;

		/// <summary>
		/// <c>True</c> if the movement prediction is enabled in fixed update.
		/// </summary>
		[System.Obsolete("Use Object.IsInSimulation.")]
		public bool IsPredictingInFixedUpdate => Object.IsInSimulation;

		/// <summary>
		/// <c>True</c> if the movement interpolation is enabled in fixed update.
		/// </summary>
		[System.Obsolete("Interpolation in fixed update has been removed.")]
		public bool IsInterpolatingInFixedUpdate => false;

		/// <summary>
		/// <c>True</c> if the movement prediction is enabled in render update.
		/// </summary>
		public bool IsPredictingInRenderUpdate
		{
			get
			{
				if (Object.HasInputAuthority == true)
					return _settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_PredictRender;
				if (Object.HasStateAuthority == true)
					return _settings.StateAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_PredictRender;

				return false;
			}
		}

		/// <summary>
		/// <c>True</c> if the movement interpolation is enabled in render update.
		/// </summary>
		public bool IsInterpolatingInRenderUpdate
		{
			get
			{
				if (Object.HasInputAuthority == true)
					return _settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_InterpolateRender;
				if (Object.HasStateAuthority == true)
					return _settings.StateAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_InterpolateRender;

				return true;
			}
		}

		/// <summary>
		/// Tick number of the last fixed update in which KCC was predicted.
		/// </summary>
		public int LastPredictedFixedTick => _lastPredictedFixedTick;

		/// <summary>
		/// Frame number of the last render update in which KCC was predicted.
		/// </summary>
		public float LastPredictedRenderFrame => _lastPredictedRenderFrame;

		/// <summary>
		/// Frame number of the last render update in which KCC look rotation was predicted.
		/// The look rotation can be render predicted even if the KCC is render interpolated using <c>KCCSettings.ForcePredictedLookRotation</c>.
		/// </summary>
		public float LastPredictedLookRotationFrame => _lastPredictedLookRotationFrame;

		/// <summary>
		/// Render position difference on input authority compared to state authority.
		/// </summary>
		public Vector3 PredictionError => _predictionError;

		/// <summary>
		/// Locally executed processors. This list is cleared in <c>Initialize()</c> and initialized with <c>KCCSettings.Processors</c>.
		/// The list is read-only and can be explicitly modified by <c>AddLocalProcessor()</c> and <c>RemoveLocalProcessor()</c>.
		/// </summary>
		public ReadOnlyCollection<IKCCProcessor> LocalProcessors => _localROProcessors;
	}
}
