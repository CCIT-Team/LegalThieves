namespace Fusion.Addons.KCC
{
	using System;
	using UnityEngine;

	// This file contains penetration solver.
	public partial class KCC
	{
		// PUBLIC METHODS

		[Obsolete("ResolvePenetration now works directly with KCCData.BasePosition and KCCData.TargetPosition. Please switch to ResolvePenetration(KCCOverlapInfo overlapInfo, KCCData data, int maxSteps, bool probeGrounding, bool resolveTriggers).")]
		public Vector3 ResolvePenetration(KCCOverlapInfo overlapInfo, KCCData data, Vector3 basePosition, Vector3 targetPosition, bool probeGrounding, int maxSteps, int resolverIterations, bool resolveTriggers)
		{
			data.BasePosition   = basePosition;
			data.TargetPosition = targetPosition;

			ResolvePenetration(overlapInfo, data, maxSteps, probeGrounding, resolveTriggers);

			return data.TargetPosition;
		}

		/// <summary>
		/// Computes penetration vectors for given OverlapInfo and moves the KCC out of collision.
		/// It is safe to pass OverlapInfo with results from inflated capsule overlap. Penetration info is stored in overlap hits.
		/// </summary>
		/// <param name="overlapInfo">Info with overlap hits.</param>
		/// <param name="data">KCCData instance. Results are stored here.</param>
		/// <param name="maxSteps">How many times the penetration is calculated.</param>
		/// <param name="probeGrounding">Make extra ground check after depenetration.</param>
		/// <param name="resolveTriggers">Whether the penetration should be checked against triggers.</param>
		public void ResolvePenetration(KCCOverlapInfo overlapInfo, KCCData data, int maxSteps, bool probeGrounding, bool resolveTriggers)
		{
			if (_settings.SuppressConvexMeshColliders == true)
			{
				overlapInfo.ToggleConvexMeshColliders(false);
			}

			if (overlapInfo.ColliderHitCount == 1)
			{
				DepenetrateSingle(overlapInfo, data, probeGrounding);
			}
			else if (overlapInfo.ColliderHitCount > 1)
			{
				DepenetrateMultiple(overlapInfo, data, probeGrounding, maxSteps);
			}

			RecalculateGroundProperties(data);

			if (resolveTriggers == true)
			{
				for (int i = 0; i < overlapInfo.TriggerHitCount; ++i)
				{
					KCCOverlapHit hit = overlapInfo.TriggerHits[i];
					hit.Transform.GetPositionAndRotation(out hit.CachedPosition, out hit.CachedRotation);

					bool hasPenetration = Physics.ComputePenetration(_collider.Collider, data.TargetPosition, Quaternion.identity, hit.Collider, hit.CachedPosition, hit.CachedRotation, out Vector3 direction, out float distance);

					hit.IsWithinExtent = hasPenetration;
					hit.HasPenetration = hasPenetration;
					hit.MaxPenetration = default;
					hit.CollisionType  = hasPenetration == true ? ECollisionType.Trigger : ECollisionType.None;

					if (distance > hit.MaxPenetration)
					{
						hit.MaxPenetration = distance;
					}
				}
			}

			if (_settings.SuppressConvexMeshColliders == true)
			{
				overlapInfo.ToggleConvexMeshColliders(true);
			}
		}

		// PRIVATE METHODS

		private void DepenetrateSingle(KCCOverlapInfo overlapInfo, KCCData data, bool probeGrounding)
		{
			float minGroundDot = Mathf.Cos(Mathf.Clamp(data.MaxGroundAngle, 0.0f, 90.0f) * Mathf.Deg2Rad);

			KCCOverlapHit hit = overlapInfo.ColliderHits[0];
			hit.IsWithinExtent = default;
			hit.HasPenetration = default;
			hit.MaxPenetration = default;
			hit.UpDirectionDot = default;
			hit.Transform.GetPositionAndRotation(out hit.CachedPosition, out hit.CachedRotation);

			hit.HasPenetration = Physics.ComputePenetration(_collider.Collider, data.TargetPosition, Quaternion.identity, hit.Collider, hit.CachedPosition, hit.CachedRotation, out Vector3 direction, out float distance);
			if (hit.HasPenetration == true)
			{
				hit.IsWithinExtent = true;
				hit.MaxPenetration = distance;
				hit.UpDirectionDot = Vector3.Dot(direction, Vector3.up);

				float minWallDot = -Mathf.Cos(Mathf.Clamp(90.0f - data.MaxWallAngle, 0.0f, 90.0f) * Mathf.Deg2Rad);
				float minHangDot = -Mathf.Cos(Mathf.Clamp(90.0f - data.MaxHangAngle, 0.0f, 90.0f) * Mathf.Deg2Rad);

				if (hit.UpDirectionDot >= minGroundDot)
				{
					hit.CollisionType = ECollisionType.Ground;

					data.IsGrounded     = true;
					data.GroundNormal   = direction;
					data.GroundDistance = default;
					data.GroundAngle    = Vector3.Angle(direction, Vector3.up);
				}
				else if (hit.UpDirectionDot > -minWallDot)
				{
					hit.CollisionType = ECollisionType.Slope;
				}
				else if (hit.UpDirectionDot >= minWallDot)
				{
					hit.CollisionType = ECollisionType.Wall;
				}
				else if (hit.UpDirectionDot >= minHangDot)
				{
					hit.CollisionType = ECollisionType.Hang;
				}
				else
				{
					hit.CollisionType = ECollisionType.Top;
				}

				if (hit.UpDirectionDot > 0.0f && hit.UpDirectionDot < minGroundDot)
				{
					if (distance >= 0.000001f && data.DynamicVelocity.y <= 0.0f)
					{
						Vector3 positionDelta = data.TargetPosition - data.BasePosition;

						float movementDot = Vector3.Dot(positionDelta.OnlyXZ(), direction.OnlyXZ());
						if (movementDot < 0.0f)
						{
							KCCPhysicsUtility.ProjectVerticalPenetration(ref direction, ref distance);
						}
					}
				}

				data.TargetPosition += direction * distance;
			}

			if (data.IsGrounded == true)
			{
				data.GroundPosition = data.TargetPosition + (Vector3.up - data.GroundNormal) * _settings.Radius;
				data.GroundAngle    = Vector3.Angle(data.GroundNormal, Vector3.up);
			}

			if (data.IsGrounded == false && probeGrounding == true)
			{
				bool isGrounded = KCCPhysicsUtility.CheckGround(_collider.Collider, data.TargetPosition, hit.Collider, hit.CachedPosition, hit.CachedRotation, _settings.Radius, _settings.Height, _settings.Extent, minGroundDot, out Vector3 checkGroundPosition, out Vector3 checkGroundNormal, out float checkGroundDistance, out bool isWithinExtent);
				if (isGrounded == true)
				{
					data.IsGrounded     = true;
					data.GroundNormal   = checkGroundNormal;
					data.GroundPosition = checkGroundPosition;
					data.GroundDistance = checkGroundDistance;
					data.GroundAngle    = Vector3.Angle(checkGroundNormal, Vector3.up);

					hit.IsWithinExtent = true;
					hit.CollisionType  = ECollisionType.Ground;
				}
				else if (isWithinExtent == true)
				{
					hit.IsWithinExtent = true;

					if (hit.CollisionType == ECollisionType.None)
					{
						hit.CollisionType = ECollisionType.Slope;
					}
				}
			}
		}

		private void DepenetrateMultiple(KCCOverlapInfo overlapInfo, KCCData data, bool probeGrounding, int maxSteps)
		{
			if (maxSteps <= 0)
			{
				maxSteps = 1;
			}

			float   minGroundDot        = Mathf.Cos(Mathf.Clamp(data.MaxGroundAngle, 0.0f, 90.0f) * Mathf.Deg2Rad);
			float   minWallDot          = -Mathf.Cos(Mathf.Clamp(90.0f - data.MaxWallAngle, 0.0f, 90.0f) * Mathf.Deg2Rad);
			float   minHangDot          = -Mathf.Cos(Mathf.Clamp(90.0f - data.MaxHangAngle, 0.0f, 90.0f) * Mathf.Deg2Rad);
			float   maxGroundDot        = default;
			Vector3 maxGroundNormal     = default;
			Vector3 averageGroundNormal = default;
			Vector3 positionDelta       = data.TargetPosition - data.BasePosition;
			Vector3 positionDeltaXZ     = positionDelta.OnlyXZ();

			for (int i = 0; i < overlapInfo.ColliderHitCount; ++i)
			{
				KCCOverlapHit hit = overlapInfo.ColliderHits[i];
				hit.IsWithinExtent = default;
				hit.HasPenetration = default;
				hit.MaxPenetration = default;
				hit.UpDirectionDot = float.MinValue;
				hit.Transform.GetPositionAndRotation(out hit.CachedPosition, out hit.CachedRotation);
			}

			if (maxSteps > 1)
			{
				float minStepDistance = 0.001f;
				float targetDistance  = Vector3.Magnitude(positionDelta);

				if (targetDistance < maxSteps * minStepDistance)
				{
					maxSteps = Mathf.Max(1, (int)(targetDistance / minStepDistance));
				}
			}

			int remainingSteps = maxSteps;
			while (remainingSteps > 0)
			{
				--remainingSteps;

				_resolver.Reset();

				for (int i = 0; i < overlapInfo.ColliderHitCount; ++i)
				{
					KCCOverlapHit hit = overlapInfo.ColliderHits[i];

					bool hasPenetration = Physics.ComputePenetration(_collider.Collider, data.TargetPosition, Quaternion.identity, hit.Collider, hit.CachedPosition, hit.CachedRotation, out Vector3 direction, out float distance);
					if (hasPenetration == false)
						continue;

					hit.HasPenetration = true;
					hit.IsWithinExtent = true;

					if (distance > hit.MaxPenetration)
					{
						hit.MaxPenetration = distance;
					}

					float upDirectionDot = Vector3.Dot(direction, Vector3.up);
					if (upDirectionDot > hit.UpDirectionDot)
					{
						hit.UpDirectionDot = upDirectionDot;

						if (upDirectionDot >= minGroundDot)
						{
							hit.CollisionType = ECollisionType.Ground;

							data.IsGrounded = true;

							if (upDirectionDot >= maxGroundDot)
							{
								maxGroundDot    = upDirectionDot;
								maxGroundNormal = direction;
							}

							averageGroundNormal += direction * upDirectionDot;
						}
						else if (upDirectionDot > -minWallDot)
						{
							hit.CollisionType = ECollisionType.Slope;
						}
						else if (upDirectionDot >= minWallDot)
						{
							hit.CollisionType = ECollisionType.Wall;
						}
						else if (upDirectionDot >= minHangDot)
						{
							hit.CollisionType = ECollisionType.Hang;
						}
						else
						{
							hit.CollisionType = ECollisionType.Top;
						}
					}

					if (upDirectionDot > 0.0f && upDirectionDot < minGroundDot)
					{
						if (distance >= 0.000001f && data.DynamicVelocity.y <= 0.0f)
						{
							float movementDot = Vector3.Dot(positionDeltaXZ, direction.OnlyXZ());
							if (movementDot < 0.0f)
							{
								KCCPhysicsUtility.ProjectVerticalPenetration(ref direction, ref distance);
							}
						}
					}

					_resolver.AddCorrection(direction, distance);
				}

				if (_resolver.Count <= 0)
					break;

				int   resolverInterations = 8;
				float resolverMaxError    = 0.001f;

				if (remainingSteps == 0)
				{
					resolverInterations = 12;
					resolverMaxError    = 0.0001f;
				}

				Vector3 correction = _resolver.CalculateBest(resolverInterations, resolverMaxError);
				correction = Vector3.ClampMagnitude(correction, _settings.Radius);

				float correctionMultiplier = Mathf.Max(0.25f, 1.0f - remainingSteps * 0.25f);
				correction *= correctionMultiplier;

				data.TargetPosition += correction;
			}

			for (int i = 0; i < overlapInfo.ColliderHitCount; ++i)
			{
				KCCOverlapHit hit = overlapInfo.ColliderHits[i];
				if (hit.UpDirectionDot == float.MinValue)
				{
					hit.UpDirectionDot = default;
				}
			}

			if (data.IsGrounded == true)
			{
				data.GroundNormal = maxGroundNormal;

				averageGroundNormal.Normalize();

				float averageGroundNormalUpDot = Vector3.Dot(averageGroundNormal, Vector3.up);
				if (averageGroundNormalUpDot >= maxGroundDot)
				{
					data.GroundNormal = averageGroundNormal;
				}

				data.GroundPosition = data.TargetPosition + (Vector3.up - data.GroundNormal) * _settings.Radius;
				data.GroundDistance = default;
				data.GroundAngle    = Vector3.Angle(data.GroundNormal, Vector3.up);
			}

			if (data.IsGrounded == false && probeGrounding == true)
			{
				Vector3 closestGroundNormal   = Vector3.up;
				Vector3 closestGroundPosition = default;
				float   closestGroundDistance = 1000.0f;

				for (int i = 0; i < overlapInfo.ColliderHitCount; ++i)
				{
					KCCOverlapHit hit = overlapInfo.ColliderHits[i];

					bool isGrounded = KCCPhysicsUtility.CheckGround(_collider.Collider, data.TargetPosition, hit.Collider, hit.CachedPosition, hit.CachedRotation, _settings.Radius, _settings.Height, _settings.Extent, minGroundDot, out Vector3 checkGroundPosition, out Vector3 checkGroundNormal, out float checkGroundDistance, out bool isWithinExtent);
					if (isGrounded == true)
					{
						data.IsGrounded = true;

						if (checkGroundDistance < closestGroundDistance)
						{
							closestGroundNormal   = checkGroundNormal;
							closestGroundPosition = checkGroundPosition;
							closestGroundDistance = checkGroundDistance;
						}

						hit.IsWithinExtent = true;
						hit.CollisionType  = ECollisionType.Ground;
					}
					else if (isWithinExtent == true)
					{
						hit.IsWithinExtent = true;

						if (hit.CollisionType == ECollisionType.None)
						{
							hit.CollisionType = ECollisionType.Slope;
						}
					}
				}

				if (data.IsGrounded == true)
				{
					data.IsGrounded     = true;
					data.GroundNormal   = closestGroundNormal;
					data.GroundPosition = closestGroundPosition;
					data.GroundDistance = closestGroundDistance;
					data.GroundAngle    = Vector3.Angle(closestGroundNormal, Vector3.up);
				}
			}
		}

		private static void RecalculateGroundProperties(KCCData data)
		{
			if (data.IsGrounded == false)
				return;

			if (KCCPhysicsUtility.ProjectOnGround(data.GroundNormal, data.GroundNormal.OnlyXZ(), out Vector3 projectedGroundNormal) == true)
			{
				data.GroundTangent = projectedGroundNormal.normalized;
				return;
			}

			if (KCCPhysicsUtility.ProjectOnGround(data.GroundNormal, data.DesiredVelocity.OnlyXZ(), out Vector3 projectedDesiredVelocity) == true)
			{
				data.GroundTangent = projectedDesiredVelocity.normalized;
				return;
			}

			data.GroundTangent = data.TransformDirection;
		}
	}
}
