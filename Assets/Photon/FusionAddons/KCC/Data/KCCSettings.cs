namespace Fusion.Addons.KCC
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Base settings for <c>KCC</c>, can be modified at runtime.
	/// </summary>
	[Serializable]
	public sealed partial class KCCSettings
	{
		// CONSTANTS

		public static readonly int   MaxNestedStages                 = 32;
		public static readonly float ExtrapolationDeltaTimeThreshold = 0.00005f;

		// PUBLIC MEMBERS

		[Header("Networked Settings")]

		[Tooltip("Defines KCC physics behavior. This property is networked.\n" +
		"• None - Skips internal physics query, collider is despawned.\n" +
		"• Capsule - Full physics processing, Capsule collider spawned.")]
		public EKCCShape Shape = EKCCShape.Capsule;

		[Tooltip("Sets collider isTrigger. This property is networked.")]
		public bool IsTrigger = false;

		[Tooltip("Sets collider radius. This property is networked.")]
		public float Radius = 0.35f;

		[Tooltip("Sets collider height. This property is networked.")]
		public float Height = 1.8f;

		[Tooltip("Defines additional radius extent for ground detection and processors tracking. Recommended range is 10-20% of radius. This property is networked.\n" +
		"• Low value decreases stability and has potential performance impact when executing additional checks.\n" +
		"• High value increases stability at the cost of increased sustained performance impact.")]
		public float Extent = 0.035f;

		[Tooltip("Sets layer of collider game object. This property is networked.")]
		[KCCLayer]
		public int ColliderLayer = 0;

		[Tooltip("Layer mask the KCC collides with. This property is networked.")]
		public LayerMask CollisionLayerMask = 1;

		[Tooltip("Default KCC features. This property is networked.")]
		public EKCCFeatures Features = EKCCFeatures.All;

		[Tooltip("Defines update behavior for KCC with input authority. This has priority over State Authority Behavior. This property is networked.\n" +
		"• Predict Fixed | Interpolate Render - Full processing/prediction in fixed update, interpolation between last two predicted fixed update states in render update.\n" +
		"• Predict Fixed | Predict Render - Full processing/prediction in fixed update, full processing/prediction in render update.")]
		public EKCCAuthorityBehavior InputAuthorityBehavior = EKCCAuthorityBehavior.PredictFixed_InterpolateRender;

		[Tooltip("Defines update behavior for KCC with state authority. This property is networked.\n" +
		"• Predict Fixed | Interpolate Render - Full processing/prediction in fixed update, interpolation between last two predicted fixed update states in render update.\n" +
		"• Predict Fixed | Predict Render - Full processing/prediction in fixed update, full processing/prediction in render update.")]
		public EKCCAuthorityBehavior StateAuthorityBehavior = EKCCAuthorityBehavior.PredictFixed_InterpolateRender;

		[Tooltip("Defines interpolation behavior. Proxies predicted in fixed update are always fully interpolated. This property is networked.\n" +
		"• Full - Interpolates all networked properties in KCCSettings and KCCData, synchronizes Transform and Rigidbody components everytime interpolation is triggered.\n" +
		"• Transform - Interpolates only position and rotation and synchronizes Transform component. This mode is fastest, but most KCCSettings and KCCData properties won't be synchronized. Use with caution!")]
		public EKCCInterpolationMode ProxyInterpolationMode = EKCCInterpolationMode.Full;

		[Tooltip("Forces render prediction of look rotation for input authority. Can be used for extra look responsiveness with other properties being interpolated. This property is networked.")]
		public bool ForcePredictedLookRotation = false;

		[Tooltip("Allows input authority to call Teleport RPC. Use with caution. This property is networked.")]
		public bool AllowClientTeleports = false;

		[Header("Local Settings")]

		[KCCProcessorReference][Tooltip("Default processors, propagated to KCC.LocalProcessors upon initialization. This property is not networked.")]
		public UnityEngine.Object[] Processors;

		[Tooltip("Penetration in single move step is corrected in multiple steps which results in higher overall depenetration quality. This property is not networked.")]
		[Range(1, 16)]
		public int MaxPenetrationSteps = 8;

		[Tooltip("Controls maximum distance the KCC moves in a single CCD step. Valid range is 10% - 90% of the radius. Use lower values if the character passes through geometry.\n" +
		"This setting is valid only when EKCCFeature.CCD is enabled. CCD Max Step Distance = Radius * CCD Radius Multiplier. This property is not networked.")]
		[Range(0.1f, 0.9f)]
		public float CCDRadiusMultiplier = 0.75f;

		[Tooltip("Defines render position distance tolerance to smooth out jitter. Higher values may introduce noticeable delay when switching move direction. This property is not networked.\n" +
		"• X = Horizontal axis.\n" +
		"• Y = Vertical axis.")]
		public Vector2 AntiJitterDistance = new Vector2(0.025f, 0.01f);

		[Tooltip("How fast prediction error interpolates towards zero. This property is not networked.")]
		public float PredictionCorrectionSpeed = 30.0f;

		[Tooltip("Maximum interactions synchronized over network. Includes Collisions, Modifiers and Ignores from KCCData. This property is not networked.")]
		public int NetworkedInteractions = 8;

		[Tooltip("Reduces network traffic by synchronizing compressed position at the cost of precision. Useful for non-player characters. This property is not networked.")]
		public bool CompressNetworkPosition = false;

		[Tooltip("Perform single overlap query during move. Hits are tracked on position before depenetration. This is a performance optimization for non-player characters at the cost of possible errors in movement. This property is not networked.")]
		public bool ForceSingleOverlapQuery = false;

		[Tooltip("Enable to always check collisions against non-convex mesh colliders to prevent ghost collisions and incorrect penetration vectors. This property is not networked.")]
		public bool SuppressConvexMeshColliders = false;


		// PUBLIC METHODS

		public void CopyFromOther(KCCSettings other)
		{
			Shape                       = other.Shape;
			IsTrigger                   = other.IsTrigger;
			Radius                      = other.Radius;
			Height                      = other.Height;
			Extent                      = other.Extent;
			ColliderLayer               = other.ColliderLayer;
			CollisionLayerMask          = other.CollisionLayerMask;
			Features                    = other.Features;
			InputAuthorityBehavior      = other.InputAuthorityBehavior;
			StateAuthorityBehavior      = other.StateAuthorityBehavior;
			ProxyInterpolationMode      = other.ProxyInterpolationMode;
			ForcePredictedLookRotation  = other.ForcePredictedLookRotation;
			AllowClientTeleports        = other.AllowClientTeleports;

			if (other.Processors != null && other.Processors.Length > 0)
			{
				Processors = new UnityEngine.Object[other.Processors.Length];
				Array.Copy(other.Processors, Processors, Processors.Length);
			}
			else
			{
				Processors = null;
			}

			MaxPenetrationSteps         = other.MaxPenetrationSteps;
			CCDRadiusMultiplier         = other.CCDRadiusMultiplier;
			AntiJitterDistance          = other.AntiJitterDistance;
			PredictionCorrectionSpeed   = other.PredictionCorrectionSpeed;
			NetworkedInteractions       = other.NetworkedInteractions;
			CompressNetworkPosition     = other.CompressNetworkPosition;
			ForceSingleOverlapQuery     = other.ForceSingleOverlapQuery;
			SuppressConvexMeshColliders = other.SuppressConvexMeshColliders;

			CopyUserSettingsFromOther(other);
		}

		// PARTIAL METHODS

		partial void CopyUserSettingsFromOther(KCCSettings other);
	}
}
