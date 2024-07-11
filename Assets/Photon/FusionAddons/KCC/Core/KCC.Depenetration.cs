namespace Fusion.Addons.KCC
{
	using UnityEngine;

	// This file contains penetration solver.
	public partial class KCC
	{
		// PUBLIC METHODS

		public Vector3 ResolvePenetration(KCCOverlapInfo overlapInfo, KCCData data, Vector3 basePosition, Vector3 targetPosition, bool probeGrounding, int maxSteps, int resolverIterations, bool resolveTriggers)
		{
			if (_settings.SuppressConvexMeshColliders == true)
			{
				overlapInfo.ToggleConvexMeshColliders(false);
			}

			if (overlapInfo.ColliderHitCount == 1)
			{
				targetPosition = DepenetrateSingle(overlapInfo, data, basePosition, targetPosition, probeGrounding, maxSteps);
			}
			else if (overlapInfo.ColliderHitCount > 1)
			{
				targetPosition = DepenetrateMultiple(overlapInfo, data, basePosition, targetPosition, probeGrounding, maxSteps, resolverIterations);
			}

			RecalculateGroundProperties(data);

			if (resolveTriggers == true)
			{
				for (int i = 0; i < overlapInfo.TriggerHitCount; ++i)
				{
					KCCOverlapHit hit = overlapInfo.TriggerHits[i];
					hit.Transform.GetPositionAndRotation(out hit.CachedPosition, out hit.CachedRotation);

					bool hasPenetration = Physics.ComputePenetration(_collider.Collider, data.TargetPosition, Quaternion.identity, hit.Collider, hit.CachedPosition, hit.CachedRotation, out Vector3 direction, out float distance);

					hit.HasPenetration = hasPenetration;
					hit.IsWithinExtent = hasPenetration;
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

			return targetPosition;
		}

		// PRIVATE METHODS

		private Vector3 DepenetrateSingle(KCCOverlapInfo overlapInfo, KCCData data, Vector3 basePosition, Vector3 targetPosition, bool probeGrounding, int maxSteps)
		{
			float   minGroundDot   = Mathf.Cos(Mathf.Clamp(data.MaxGroundAngle, 0.0f, 90.0f) * Mathf.Deg2Rad);
			float   minWallDot     = -Mathf.Cos(Mathf.Clamp(90.0f - data.MaxWallAngle, 0.0f, 90.0f) * Mathf.Deg2Rad);
			float   minHangDot     = -Mathf.Cos(Mathf.Clamp(90.0f - data.MaxHangAngle, 0.0f, 90.0f) * Mathf.Deg2Rad);
			Vector3 groundNormal   = Vector3.up;
			float   groundDistance = default;

			KCCOverlapHit hit = overlapInfo.ColliderHits[0];
			hit.UpDirectionDot = float.MinValue;
			hit.Transform.GetPositionAndRotation(out hit.CachedPosition, out hit.CachedRotation);

			if (maxSteps > 1)
			{
				float minStepDistance = 0.001f;
				float targetDistance  = Vector3.Distance(basePosition, targetPosition);

				if (targetDistance < maxSteps * minStepDistance)
				{
					maxSteps = Mathf.Max(1, (int)(targetDistance / minStepDistance));
				}
			}

			if (maxSteps <= 1)
			{
				hit.HasPenetration = Physics.ComputePenetration(_collider.Collider, targetPosition, Quaternion.identity, hit.Collider, hit.CachedPosition, hit.CachedRotation, out Vector3 direction, out float distance);
				if (hit.HasPenetration == true)
				{
					hit.IsWithinExtent = true;

					if (distance > hit.MaxPenetration)
					{
						hit.MaxPenetration = distance;
					}

					float directionUpDot = Vector3.Dot(direction, Vector3.up);
					if (directionUpDot > hit.UpDirectionDot)
					{
						hit.UpDirectionDot = directionUpDot;

						if (directionUpDot >= minGroundDot)
						{
							hit.CollisionType = ECollisionType.Ground;

							data.IsGrounded = true;

							groundNormal = direction;
						}
						else if (directionUpDot > -minWallDot)
						{
							hit.CollisionType = ECollisionType.Slope;
						}
						else if (directionUpDot >= minWallDot)
						{
							hit.CollisionType = ECollisionType.Wall;
						}
						else if (directionUpDot >= minHangDot)
						{
							hit.CollisionType = ECollisionType.Hang;
						}
						else
						{
							hit.CollisionType = ECollisionType.Top;
						}
					}

					if (directionUpDot > 0.0f && directionUpDot < minGroundDot)
					{
						if (distance >= 0.000001f && data.DynamicVelocity.y <= 0.0f)
						{
							Vector3 positionDelta = targetPosition - basePosition;

							float movementDot = Vector3.Dot(positionDelta.OnlyXZ(), direction.OnlyXZ());
							if (movementDot < 0.0f)
							{
								KCCPhysicsUtility.ProjectVerticalPenetration(ref direction, ref distance);
							}
						}
					}

					targetPosition += direction * distance;
				}
			}
			else
			{
				Vector3 stepPositionDelta = (targetPosition - basePosition) / maxSteps;
				Vector3 desiredPosition   = basePosition;
				int     remainingSteps    = maxSteps;

				while (remainingSteps > 0)
				{
					--remainingSteps;

					desiredPosition += stepPositionDelta;

					hit.HasPenetration = Physics.ComputePenetration(_collider.Collider, desiredPosition, Quaternion.identity, hit.Collider, hit.CachedPosition, hit.CachedRotation, out Vector3 direction, out float distance);
					if (hit.HasPenetration == false)
						continue;

					hit.IsWithinExtent = true;

					if (distance > hit.MaxPenetration)
					{
						hit.MaxPenetration = distance;
					}

					float directionUpDot = Vector3.Dot(direction, Vector3.up);
					if (directionUpDot > hit.UpDirectionDot)
					{
						hit.UpDirectionDot = directionUpDot;

						if (directionUpDot >= minGroundDot)
						{
							hit.CollisionType = ECollisionType.Ground;

							data.IsGrounded = true;

							groundNormal = direction;
						}
						else if (directionUpDot > -minWallDot)
						{
							hit.CollisionType = ECollisionType.Slope;
						}
						else if (directionUpDot >= minWallDot)
						{
							hit.CollisionType = ECollisionType.Wall;
						}
						else if (directionUpDot >= minHangDot)
						{
							hit.CollisionType = ECollisionType.Hang;
						}
						else
						{
							hit.CollisionType = ECollisionType.Top;
						}
					}

					if (directionUpDot > 0.0f && directionUpDot < minGroundDot)
					{
						if (distance >= 0.000001f && data.DynamicVelocity.y <= 0.0f)
						{
							float movementDot = Vector3.Dot(stepPositionDelta.OnlyXZ(), direction.OnlyXZ());
							if (movementDot < 0.0f)
							{
								KCCPhysicsUtility.ProjectVerticalPenetration(ref direction, ref distance);
							}
						}
					}

					desiredPosition += direction * distance;
				}

				targetPosition = desiredPosition;
			}

			if (hit.UpDirectionDot == float.MinValue)
			{
				hit.UpDirectionDot = default;
			}

			if (probeGrounding == true && data.IsGrounded == false)
			{
				bool isGrounded = KCCPhysicsUtility.CheckGround(_collider.Collider, targetPosition, hit.Collider, hit.CachedPosition, hit.CachedRotation, _settings.Radius, _settings.Height, _settings.Extent, minGroundDot, out Vector3 checkGroundNormal, out float checkGroundDistance, out bool isWithinExtent);
				if (isGrounded == true)
				{
					data.IsGrounded = true;

					groundNormal   = checkGroundNormal;
					groundDistance = checkGroundDistance;

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
				data.GroundNormal   = groundNormal;
				data.GroundAngle    = Vector3.Angle(groundNormal, Vector3.up);
				data.GroundPosition = targetPosition + new Vector3(0.0f, _settings.Radius, 0.0f) - groundNormal * (_settings.Radius + groundDistance);
				data.GroundDistance = groundDistance;
			}

			return targetPosition;
		}

		private Vector3 DepenetrateMultiple(KCCOverlapInfo overlapInfo, KCCData data, Vector3 basePosition, Vector3 targetPosition, bool probeGrounding, int maxSteps, int resolverIterations)
		{
			float   minGroundDot        = Mathf.Cos(Mathf.Clamp(data.MaxGroundAngle, 0.0f, 90.0f) * Mathf.Deg2Rad);
			float   minWallDot          = -Mathf.Cos(Mathf.Clamp(90.0f - data.MaxWallAngle, 0.0f, 90.0f) * Mathf.Deg2Rad);
			float   minHangDot          = -Mathf.Cos(Mathf.Clamp(90.0f - data.MaxHangAngle, 0.0f, 90.0f) * Mathf.Deg2Rad);
			float   groundDistance      = default;
			float   maxGroundDot        = default;
			Vector3 maxGroundNormal     = default;
			Vector3 averageGroundNormal = default;
			Vector3 positionDelta       = targetPosition - basePosition;
			Vector3 positionDeltaXZ     = positionDelta.OnlyXZ();

			for (int i = 0; i < overlapInfo.ColliderHitCount; ++i)
			{
				KCCOverlapHit hit = overlapInfo.ColliderHits[i];
				hit.UpDirectionDot = float.MinValue;
				hit.Transform.GetPositionAndRotation(out hit.CachedPosition, out hit.CachedRotation);
			}

			if (maxSteps > 1)
			{
				float minStepDistance = 0.001f;
				float targetDistance  = Vector3.Distance(basePosition, targetPosition);

				if (targetDistance < maxSteps * minStepDistance)
				{
					maxSteps = Mathf.Max(1, (int)(targetDistance / minStepDistance));
				}
			}

			if (maxSteps <= 1)
			{
				_resolver.Reset();

				for (int i = 0; i < overlapInfo.ColliderHitCount; ++i)
				{
					KCCOverlapHit hit = overlapInfo.ColliderHits[i];

					hit.HasPenetration = Physics.ComputePenetration(_collider.Collider, targetPosition, Quaternion.identity, hit.Collider, hit.CachedPosition, hit.CachedRotation, out Vector3 direction, out float distance);
					if (hit.HasPenetration == false)
						continue;

					hit.IsWithinExtent = true;

					if (distance > hit.MaxPenetration)
					{
						hit.MaxPenetration = distance;
					}

					float directionUpDot = Vector3.Dot(direction, Vector3.up);
					if (directionUpDot > hit.UpDirectionDot)
					{
						hit.UpDirectionDot = directionUpDot;

						if (directionUpDot >= minGroundDot)
						{
							hit.CollisionType = ECollisionType.Ground;

							data.IsGrounded = true;

							if (directionUpDot >= maxGroundDot)
							{
								maxGroundDot    = directionUpDot;
								maxGroundNormal = direction;
							}

							averageGroundNormal += direction * directionUpDot;
						}
						else if (directionUpDot > -minWallDot)
						{
							hit.CollisionType = ECollisionType.Slope;
						}
						else if (directionUpDot >= minWallDot)
						{
							hit.CollisionType = ECollisionType.Wall;
						}
						else if (directionUpDot >= minHangDot)
						{
							hit.CollisionType = ECollisionType.Hang;
						}
						else
						{
							hit.CollisionType = ECollisionType.Top;
						}
					}

					if (directionUpDot > 0.0f && directionUpDot < minGroundDot)
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

				int remainingSubSteps = Mathf.Max(0, resolverIterations);

				float multiplier = 1.0f - Mathf.Min(remainingSubSteps, 2) * 0.25f;

				if (_resolver.Size == 2)
				{
					_resolver.GetCorrection(0, out Vector3 direction0);
					_resolver.GetCorrection(1, out Vector3 direction1);

					if (Vector3.Dot(direction0, direction1) >= 0.0f)
					{
						targetPosition += _resolver.CalculateMinMax() * multiplier;
					}
					else
					{
						targetPosition += _resolver.CalculateBinary() * multiplier;
					}
				}
				else
				{
					targetPosition += _resolver.CalculateGradientDescent(12, 0.0001f) * multiplier;
				}

				while (remainingSubSteps > 0)
				{
					--remainingSubSteps;

					_resolver.Reset();

					for (int i = 0; i < overlapInfo.ColliderHitCount; ++i)
					{
						KCCOverlapHit hit = overlapInfo.ColliderHits[i];

						bool hasPenetration = Physics.ComputePenetration(_collider.Collider, targetPosition, Quaternion.identity, hit.Collider, hit.CachedPosition, hit.CachedRotation, out Vector3 direction, out float distance);
						if (hasPenetration == false)
							continue;

						hit.IsWithinExtent = true;
						hit.HasPenetration = true;

						if (distance > hit.MaxPenetration)
						{
							hit.MaxPenetration = distance;
						}

						float directionUpDot = Vector3.Dot(direction, Vector3.up);
						if (directionUpDot > hit.UpDirectionDot)
						{
							hit.UpDirectionDot = directionUpDot;

							if (directionUpDot >= minGroundDot)
							{
								hit.CollisionType = ECollisionType.Ground;

								data.IsGrounded = true;

								if (directionUpDot >= maxGroundDot)
								{
									maxGroundDot    = directionUpDot;
									maxGroundNormal = direction;
								}

								averageGroundNormal += direction * directionUpDot;
							}
							else if (directionUpDot > -minWallDot)
							{
								hit.CollisionType = ECollisionType.Slope;
							}
							else if (directionUpDot >= minWallDot)
							{
								hit.CollisionType = ECollisionType.Wall;
							}
							else if (directionUpDot >= minHangDot)
							{
								hit.CollisionType = ECollisionType.Hang;
							}
							else
							{
								hit.CollisionType = ECollisionType.Top;
							}
						}

						if (directionUpDot > 0.0f && directionUpDot < minGroundDot)
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

					if (_resolver.Size == 0)
						break;

					if (remainingSubSteps == 0)
					{
						if (_resolver.Size == 2)
						{
							_resolver.GetCorrection(0, out Vector3 direction0);
							_resolver.GetCorrection(1, out Vector3 direction1);

							if (Vector3.Dot(direction0, direction1) >= 0.0f)
							{
								targetPosition += _resolver.CalculateGradientDescent(12, 0.0001f);
							}
							else
							{
								targetPosition += _resolver.CalculateBinary();
							}
						}
						else
						{
							targetPosition += _resolver.CalculateGradientDescent(12, 0.0001f);
						}
					}
					else if (remainingSubSteps == 1)
					{
						targetPosition += _resolver.CalculateMinMax() * 0.75f;
					}
					else
					{
						targetPosition += _resolver.CalculateMinMax() * 0.5f;
					}
				}
			}
			else
			{
				Vector3 stepPositionDelta = (targetPosition - basePosition) / maxSteps;
				Vector3 desiredPosition   = basePosition;
				int     remainingSteps    = maxSteps;

				while (remainingSteps > 1)
				{
					--remainingSteps;

					desiredPosition += stepPositionDelta;

					_resolver.Reset();

					for (int i = 0; i < overlapInfo.ColliderHitCount; ++i)
					{
						KCCOverlapHit hit = overlapInfo.ColliderHits[i];

						hit.HasPenetration = Physics.ComputePenetration(_collider.Collider, desiredPosition, Quaternion.identity, hit.Collider, hit.CachedPosition, hit.CachedRotation, out Vector3 direction, out float distance);
						if (hit.HasPenetration == false)
							continue;

						hit.IsWithinExtent = true;

						if (distance > hit.MaxPenetration)
						{
							hit.MaxPenetration = distance;
						}

						float directionUpDot = Vector3.Dot(direction, Vector3.up);
						if (directionUpDot > hit.UpDirectionDot)
						{
							hit.UpDirectionDot = directionUpDot;

							if (directionUpDot >= minGroundDot)
							{
								hit.CollisionType = ECollisionType.Ground;

								data.IsGrounded = true;

								if (directionUpDot >= maxGroundDot)
								{
									maxGroundDot    = directionUpDot;
									maxGroundNormal = direction;
								}

								averageGroundNormal += direction * directionUpDot;
							}
							else if (directionUpDot > -minWallDot)
							{
								hit.CollisionType = ECollisionType.Slope;
							}
							else if (directionUpDot >= minWallDot)
							{
								hit.CollisionType = ECollisionType.Wall;
							}
							else if (directionUpDot >= minHangDot)
							{
								hit.CollisionType = ECollisionType.Hang;
							}
							else
							{
								hit.CollisionType = ECollisionType.Top;
							}
						}

						if (directionUpDot > 0.0f && directionUpDot < minGroundDot)
						{
							if (distance >= 0.000001f && data.DynamicVelocity.y <= 0.0f)
							{
								float movementDot = Vector3.Dot(stepPositionDelta.OnlyXZ(), direction.OnlyXZ());
								if (movementDot < 0.0f)
								{
									KCCPhysicsUtility.ProjectVerticalPenetration(ref direction, ref distance);
								}
							}
						}

						_resolver.AddCorrection(direction, distance);
					}

					if (_resolver.Size == 2)
					{
						_resolver.GetCorrection(0, out Vector3 direction0);
						_resolver.GetCorrection(1, out Vector3 direction1);

						if (Vector3.Dot(direction0, direction1) >= 0.0f)
						{
							desiredPosition += _resolver.CalculateMinMax();
						}
						else
						{
							desiredPosition += _resolver.CalculateBinary();
						}
					}
					else
					{
						desiredPosition += _resolver.CalculateMinMax();
					}
				}

				--remainingSteps;

				desiredPosition += stepPositionDelta;

				_resolver.Reset();

				for (int i = 0; i < overlapInfo.ColliderHitCount; ++i)
				{
					KCCOverlapHit hit = overlapInfo.ColliderHits[i];

					hit.HasPenetration = Physics.ComputePenetration(_collider.Collider, desiredPosition, Quaternion.identity, hit.Collider, hit.CachedPosition, hit.CachedRotation, out Vector3 direction, out float distance);
					if (hit.HasPenetration == false)
						continue;

					hit.IsWithinExtent = true;

					if (distance > hit.MaxPenetration)
					{
						hit.MaxPenetration = distance;
					}

					float directionUpDot = Vector3.Dot(direction, Vector3.up);
					if (directionUpDot > hit.UpDirectionDot)
					{
						hit.UpDirectionDot = directionUpDot;

						if (directionUpDot >= minGroundDot)
						{
							hit.CollisionType = ECollisionType.Ground;

							data.IsGrounded = true;

							if (directionUpDot >= maxGroundDot)
							{
								maxGroundDot    = directionUpDot;
								maxGroundNormal = direction;
							}

							averageGroundNormal += direction * directionUpDot;
						}
						else if (directionUpDot > -minWallDot)
						{
							hit.CollisionType = ECollisionType.Slope;
						}
						else if (directionUpDot >= minWallDot)
						{
							hit.CollisionType = ECollisionType.Wall;
						}
						else if (directionUpDot >= minHangDot)
						{
							hit.CollisionType = ECollisionType.Hang;
						}
						else
						{
							hit.CollisionType = ECollisionType.Top;
						}
					}

					if (directionUpDot > 0.0f && directionUpDot < minGroundDot)
					{
						if (distance >= 0.000001f && data.DynamicVelocity.y <= 0.0f)
						{
							float movementDot = Vector3.Dot(stepPositionDelta.OnlyXZ(), direction.OnlyXZ());
							if (movementDot < 0.0f)
							{
								KCCPhysicsUtility.ProjectVerticalPenetration(ref direction, ref distance);
							}
						}
					}

					_resolver.AddCorrection(direction, distance);
				}

				if (_resolver.Size == 2)
				{
					_resolver.GetCorrection(0, out Vector3 direction0);
					_resolver.GetCorrection(1, out Vector3 direction1);

					if (Vector3.Dot(direction0, direction1) >= 0.0f)
					{
						desiredPosition += _resolver.CalculateMinMax();
					}
					else
					{
						desiredPosition += _resolver.CalculateBinary();
					}
				}
				else
				{
					desiredPosition += _resolver.CalculateGradientDescent(12, 0.0001f);
				}

				targetPosition = desiredPosition;
			}

			for (int i = 0; i < overlapInfo.ColliderHitCount; ++i)
			{
				KCCOverlapHit hit = overlapInfo.ColliderHits[i];
				if (hit.UpDirectionDot == float.MinValue)
				{
					hit.UpDirectionDot = default;
				}
			}

			if (probeGrounding == true && data.IsGrounded == false)
			{
				Vector3 closestGroundNormal   = Vector3.up;
				float   closestGroundDistance = 1000.0f;

				for (int i = 0; i < overlapInfo.ColliderHitCount; ++i)
				{
					KCCOverlapHit hit = overlapInfo.ColliderHits[i];

					bool isGrounded = KCCPhysicsUtility.CheckGround(_collider.Collider, targetPosition, hit.Collider, hit.CachedPosition, hit.CachedRotation, _settings.Radius, _settings.Height, _settings.Extent, minGroundDot, out Vector3 checkGroundNormal, out float checkGroundDistance, out bool isWithinExtent);
					if (isGrounded == true)
					{
						data.IsGrounded = true;

						if (checkGroundDistance < closestGroundDistance)
						{
							closestGroundNormal   = checkGroundNormal;
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
					maxGroundNormal     = closestGroundNormal;
					averageGroundNormal = closestGroundNormal;
					groundDistance      = closestGroundDistance;
				}
			}

			if (data.IsGrounded == true)
			{
				if (averageGroundNormal.IsEqual(maxGroundNormal) == false)
				{
					averageGroundNormal.Normalize();
				}

				data.GroundNormal   = averageGroundNormal;
				data.GroundAngle    = Vector3.Angle(data.GroundNormal, Vector3.up);
				data.GroundPosition = targetPosition + new Vector3(0.0f, _settings.Radius, 0.0f) - data.GroundNormal * (_settings.Radius + groundDistance);
				data.GroundDistance = groundDistance;
			}

			return targetPosition;
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
