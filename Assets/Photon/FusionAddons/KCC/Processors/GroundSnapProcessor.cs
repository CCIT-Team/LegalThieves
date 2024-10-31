namespace Fusion.Addons.KCC
{
	using UnityEngine;

	/// <summary>
	/// This processor snaps character down after losing grounded state.
	/// </summary>
	public class GroundSnapProcessor : KCCProcessor, IAfterMoveStep
	{
		// CONSTANTS

		public static readonly int DefaultPriority = -2000;

		// PRIVATE MEMBERS

		[SerializeField][Tooltip("Maximum ground check distance for snapping.")]
		private float _snapDistance = 0.25f;
		[SerializeField][Tooltip("Ground snapping speed per second.")]
		private float _snapSpeed = 4.0f;
		[SerializeField][Tooltip("Force extra update of collision hits if the snapping is active and moves the KCC.")]
		private bool  _forceUpdateHits = false;

		private KCCData        _overlapData = new KCCData();
		private KCCOverlapInfo _overlapInfo = new KCCOverlapInfo();

		// KCCProcessor INTERFACE

		public override float GetPriority(KCC kcc) => DefaultPriority;

		// IAfterMoveStep INTERFACE

		public virtual void Execute(AfterMoveStep stage, KCC kcc, KCCData data)
		{
			if (_snapDistance <= 0.0f)
				return;

			// Ground snapping activates only if ground is lost and there's no jump or step-up active.
			if (data.IsGrounded == true || data.WasGrounded == false || data.JumpFrames > 0 || data.IsSteppingUp == true || data.WasSteppingUp == true)
				return;

			// Ignore ground snapping if there is a force pushing the character upwards.
			if (data.DynamicVelocity.y > 0.0f)
				return;

			float maxPenetrationDistance  = _snapDistance;
			float maxStepPenetrationDelta = kcc.Settings.Radius * 0.25f;
			int   penetrationSteps        = Mathf.CeilToInt(maxPenetrationDistance / maxStepPenetrationDelta);
			float penetrationDelta        = maxPenetrationDistance / penetrationSteps;
			float overlapRadius           = kcc.Settings.Radius * 1.5f;

			// Make a bigger overlap to correctly resolve penetrations along the way down.
			kcc.CapsuleOverlap(_overlapInfo, data.TargetPosition - new Vector3(0.0f, _snapDistance, 0.0f), overlapRadius, kcc.Settings.Height + _snapDistance, QueryTriggerInteraction.Ignore);

			if (_overlapInfo.ColliderHitCount == 0)
				return;

			_overlapData.CopyFromOther(data);

			// Checking collisions with full snap distance could lead to incorrect collision type (ground/slope/wall) detection.
			// So we split the downward movenent into more steps and move by 1/4 of radius at max in single step.
			for (int i = 0; i < penetrationSteps; ++i)
			{
				_overlapData.TargetPosition.y -= penetrationDelta;

				// Resolve penetration on new candidate position.
				kcc.ResolvePenetration(_overlapInfo, _overlapData, 1, false, false);

				if (_overlapData.IsGrounded == true)
				{
					// We found the ground, now move the KCC towards the grounded position.

					float maxSnapDelta = _snapSpeed * data.UpdateDeltaTime;

					if (data.WasSnappingToGround == false)
					{
						// First max snap delta is reduced by half to smooth out the snapping.
						maxSnapDelta *= 0.5f;
					}

					Vector3 targetGroundedPosition = _overlapData.TargetPosition;
					Vector3 targetSnappedPosition  = targetGroundedPosition;

					Vector3 snapPositionOffset = targetSnappedPosition - data.TargetPosition;
					if (snapPositionOffset.sqrMagnitude > maxSnapDelta * maxSnapDelta)
					{
						targetSnappedPosition = data.TargetPosition + snapPositionOffset.normalized * maxSnapDelta;
					}

					data.TargetPosition     = targetSnappedPosition;
					data.IsGrounded         = _overlapData.IsGrounded;
					data.GroundNormal       = _overlapData.GroundNormal;
					data.GroundTangent      = _overlapData.GroundTangent;
					data.GroundPosition     = _overlapData.GroundPosition;
					data.GroundDistance     = Mathf.Max(0.0f, Vector3.Distance(targetSnappedPosition, targetGroundedPosition) - kcc.Settings.Radius);
					data.GroundAngle        = _overlapData.GroundAngle;
					data.IsSnappingToGround = true;

					kcc.Debug.DrawGroundSnapping(data.TargetPosition, targetGroundedPosition, targetSnappedPosition, kcc.IsInFixedUpdate);

					if (_forceUpdateHits == true)
					{
						// New position is set, refresh collision hits after the stage.
						stage.RequestUpdateHits(true);
					}

					break;
				}
			}
		}
	}
}
