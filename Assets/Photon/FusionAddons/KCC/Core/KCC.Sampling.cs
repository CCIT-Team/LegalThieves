namespace Fusion.Addons.KCC
{
	using System;
	using UnityEngine;

	// This file contains API which resolves render position of a KCC / child transform and render look rotation of the KCC with given render alpha.
	// These methods can be used to get render accurate position of the origin for sub-tick accurate lag compensated casts (for example player's camera state on client at the time of clicling mouse).
	// It is required to pass correct render alpha (Runner.LocalAlpha passed from client through input, check KCC sample for correct usage).
	public partial class KCC
	{
		// PUBLIC METHODS

		/// <summary>
		/// Returns render position of the KCC with given render alpha.
		/// </summary>
		/// <param name="renderAlpha">Runner.LocalAlpha from the client at the time of Render().</param>
		/// <param name="renderPosition">Position of the KCC in render with given render alpha.</param>
		public bool ResolveRenderPosition(float renderAlpha, out Vector3 renderPosition)
		{
			renderAlpha = Mathf.Clamp01(renderAlpha);

			KCCData fromData;
			KCCData toData;

			if (IsInFixedUpdate == true)
			{
				int currentTick = Runner.Tick;

				if (_settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_PredictRender)
				{
					if (_lastPredictedFixedTick < currentTick)
					{
						LogError("Missing data for calculation of render position for current tick. The KCC is set to predict in render and must run fixed update for current tick before calling this method!");
						renderPosition = _fixedData.TargetPosition;
						return false;
					}

					fromData = GetHistoryData(currentTick - 1);
					toData   = GetHistoryData(currentTick);
				}
				else if (_settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_InterpolateRender)
				{
					fromData = GetHistoryData(currentTick - 2);
					toData   = GetHistoryData(currentTick - 1);
				}
				else
				{
					throw new NotImplementedException(_settings.InputAuthorityBehavior.ToString());
				}
			}
			else
			{
				if (_settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_PredictRender)
				{
					if (_lastPredictedLookRotationFrame < Time.frameCount)
					{
						LogError("Missing data for calculation of render position for current frame. The KCC is set to predict in render and must run render update for current frame before calling this method!");
						renderPosition = _renderData.TargetPosition;
						return false;
					}

					fromData = _fixedData;
					toData   = _renderData;

					if (renderAlpha > _renderData.Alpha)
					{
						renderAlpha = 1.0f;
					}
					else if (_renderData.Alpha > 0.000001f)
					{
						renderAlpha = Mathf.Clamp01(renderAlpha / _renderData.Alpha);
					}
				}
				else if (_settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_InterpolateRender)
				{
					int currentTick = Runner.Tick;

					fromData = GetHistoryData(currentTick - 1);
					toData   = GetHistoryData(currentTick);
				}
				else
				{
					throw new NotImplementedException(_settings.InputAuthorityBehavior.ToString());
				}
			}

			if (fromData == null || toData == null)
			{
				renderPosition = Data.TargetPosition;
				return false;
			}

			Vector3 fromPosition  = fromData.TargetPosition;
			Vector3 toPosition    = toData.TargetPosition;
			Vector3 positionDelta = toPosition - fromPosition;

			if (Vector3.SqrMagnitude(positionDelta) >= _settings.TeleportThreshold * _settings.TeleportThreshold)
			{
				if (renderAlpha >= 0.5f)
				{
					renderPosition = toPosition;
				}
				else
				{
					renderPosition = fromPosition;
				}
			}
			else
			{
				Vector3 targetPosition;

				if (_activeFeatures.Has(EKCCFeature.AntiJitter) == true && _settings.AntiJitterDistance.IsZero() == false)
				{
					targetPosition = fromPosition;
					positionDelta  = Vector3.Lerp(fromPosition, toPosition, renderAlpha) - fromPosition;

					float distanceY = Mathf.Abs(positionDelta.y);
					if (distanceY > 0.000001f && distanceY > _settings.AntiJitterDistance.y)
					{
						targetPosition.y += positionDelta.y * ((distanceY - _settings.AntiJitterDistance.y) / distanceY);
					}

					Vector3 positionDeltaXZ = positionDelta.OnlyXZ();

					float distanceXZ = Vector3.Magnitude(positionDeltaXZ);
					if (distanceXZ > 0.000001f && distanceXZ > _settings.AntiJitterDistance.x)
					{
						targetPosition += positionDeltaXZ * ((distanceXZ - _settings.AntiJitterDistance.x) / distanceXZ);
					}
				}
				else
				{
					targetPosition = Vector3.Lerp(fromPosition, toPosition, renderAlpha);
				}

				renderPosition = targetPosition;
			}

			return true;
		}

		/// <summary>
		/// Returns render position of a KCC child transform with given render alpha.
		/// </summary>
		/// <param name="origin">Child transform of the KCC. For example a camera handle.</param>
		/// <param name="renderAlpha">Runner.LocalAlpha from a client at the time of Render().</param>
		/// <param name="renderPosition">Position of the origin in render with given render alpha.</param>
		public bool ResolveRenderPosition(Transform origin, float renderAlpha, out Vector3 renderPosition)
		{
			if (ReferenceEquals(origin, _transform) == true)
				return ResolveRenderPosition(renderAlpha, out renderPosition);

			if (origin.IsChildOf(_transform) == false)
					throw new NotSupportedException($"Origin must be child of the KCC!");

			renderAlpha = Mathf.Clamp01(renderAlpha);

			KCCData fromData;
			KCCData toData;

			if (IsInFixedUpdate == true)
			{
				int currentTick = Runner.Tick;

				if (_settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_PredictRender)
				{
					if (_lastPredictedFixedTick < currentTick)
					{
						LogError("Missing data for calculation of render position for current tick. The KCC is set to predict in render and must run fixed update for current tick before calling this method!");
						renderPosition = origin.position;
						return false;
					}

					fromData = GetHistoryData(currentTick - 1);
					toData   = GetHistoryData(currentTick);
				}
				else if (_settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_InterpolateRender)
				{
					fromData = GetHistoryData(currentTick - 2);
					toData   = GetHistoryData(currentTick - 1);
				}
				else
				{
					throw new NotImplementedException(_settings.InputAuthorityBehavior.ToString());
				}
			}
			else
			{
				if (_settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_PredictRender)
				{
					if (_lastPredictedLookRotationFrame < Time.frameCount)
					{
						LogError("Missing data for calculation of render position for current frame. The KCC is set to predict in render and must run render update for current frame before calling this method!");
						renderPosition = origin.position;
						return false;
					}

					fromData = _fixedData;
					toData   = _renderData;

					if (renderAlpha > _renderData.Alpha)
					{
						renderAlpha = 1.0f;
					}
					else if (_renderData.Alpha > 0.000001f)
					{
						renderAlpha = Mathf.Clamp01(renderAlpha / _renderData.Alpha);
					}
				}
				else if (_settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_InterpolateRender)
				{
					int currentTick = Runner.Tick;

					fromData = GetHistoryData(currentTick - 1);
					toData   = GetHistoryData(currentTick);
				}
				else
				{
					throw new NotImplementedException(_settings.InputAuthorityBehavior.ToString());
				}
			}

			if (fromData == null || toData == null)
			{
				renderPosition = origin.position;
				return false;
			}

			Vector3 fromPosition  = fromData.TargetPosition;
			Vector3 toPosition    = toData.TargetPosition;
			Vector3 positionDelta = toPosition - fromPosition;
			Vector3 offset        = _transform.InverseTransformPoint(origin.position);

			if (Vector3.SqrMagnitude(positionDelta) >= _settings.TeleportThreshold * _settings.TeleportThreshold)
			{
				if (renderAlpha >= 0.5f)
				{
					renderPosition = toPosition + toData.TransformRotation * offset;
				}
				else
				{
					renderPosition = fromPosition + fromData.TransformRotation * offset;
				}
			}
			else
			{
				Vector3 fromOffset = fromData.TransformRotation * offset;
				Vector3 toOffset   = toData.TransformRotation   * offset;

				Vector3 targetPosition;

				if (_activeFeatures.Has(EKCCFeature.AntiJitter) == true && _settings.AntiJitterDistance.IsZero() == false)
				{
					targetPosition = fromPosition;
					positionDelta  = Vector3.Lerp(fromPosition, toPosition, renderAlpha) - fromPosition;

					float distanceY = Mathf.Abs(positionDelta.y);
					if (distanceY > 0.000001f && distanceY > _settings.AntiJitterDistance.y)
					{
						targetPosition.y += positionDelta.y * ((distanceY - _settings.AntiJitterDistance.y) / distanceY);
					}

					Vector3 positionDeltaXZ = positionDelta.OnlyXZ();

					float distanceXZ = Vector3.Magnitude(positionDeltaXZ);
					if (distanceXZ > 0.000001f && distanceXZ > _settings.AntiJitterDistance.x)
					{
						targetPosition += positionDeltaXZ * ((distanceXZ - _settings.AntiJitterDistance.x) / distanceXZ);
					}
				}
				else
				{
					targetPosition = Vector3.Lerp(fromPosition, toPosition, renderAlpha);
				}

				renderPosition = targetPosition + Vector3.Slerp(fromOffset, toOffset, renderAlpha);
			}

			return true;
		}

		/// <summary>
		/// Returns render look rotation of the KCC with given render alpha.
		/// </summary>
		/// <param name="renderAlpha">Runner.LocalAlpha from the client at the time of Render().</param>
		/// <param name="renderLookRotation">Look Rotation of the KCC in render with given render alpha.</param>
		public bool ResolveRenderLookRotation(float renderAlpha, out Quaternion renderLookRotation)
		{
			renderAlpha = Mathf.Clamp01(renderAlpha);

			KCCData fromPositionData;
			KCCData toPositionData;
			KCCData fromRotationData;
			KCCData toRotationData;

			if (IsInFixedUpdate == true)
			{
				int currentTick = Runner.Tick;

				if (_settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_PredictRender)
				{
					if (_lastPredictedFixedTick < currentTick)
					{
						LogError("Missing data for calculation of render look rotation for current tick. The KCC is set to predict in render and must run fixed update for current tick before calling this method!");
						renderLookRotation = _fixedData.LookRotation;
						return false;
					}

					fromPositionData = GetHistoryData(currentTick - 1);
					toPositionData   = GetHistoryData(currentTick);
					fromRotationData = fromPositionData;
					toRotationData   = toPositionData;
				}
				else if (_settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_InterpolateRender)
				{
					if (_settings.ForcePredictedLookRotation == true)
					{
						if (_lastPredictedFixedTick < currentTick)
						{
							LogError("Missing data for calculation of render look rotation for current tick. The KCC has Force Predicted Look Rotation enabled and must run fixed update for current tick before calling this method!");
							renderLookRotation = _fixedData.LookRotation;
							return false;
						}

						fromPositionData = GetHistoryData(currentTick - 2);
						toPositionData   = GetHistoryData(currentTick - 1);
						fromRotationData = toPositionData;
						toRotationData   = GetHistoryData(currentTick);
					}
					else
					{
						fromPositionData = GetHistoryData(currentTick - 2);
						toPositionData   = GetHistoryData(currentTick - 1);
						fromRotationData = fromPositionData;
						toRotationData   = toPositionData;
					}
				}
				else
				{
					throw new NotImplementedException(_settings.InputAuthorityBehavior.ToString());
				}
			}
			else
			{
				if (_settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_PredictRender)
				{
					if (_lastPredictedLookRotationFrame < Time.frameCount)
					{
						LogError("Missing data for calculation of render look rotation for current frame. The KCC is set to predict in render and must run render update for current frame before calling this method!");
						renderLookRotation = _renderData.LookRotation;
						return false;
					}

					fromPositionData = _fixedData;
					toPositionData   = _renderData;
					fromRotationData = _fixedData;
					toRotationData   = _renderData;

					if (renderAlpha > _renderData.Alpha)
					{
						renderAlpha = 1.0f;
					}
					else if (_renderData.Alpha > 0.000001f)
					{
						renderAlpha = Mathf.Clamp01(renderAlpha / _renderData.Alpha);
					}
				}
				else if (_settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_InterpolateRender)
				{
					int currentTick = Runner.Tick;

					fromPositionData = GetHistoryData(currentTick - 1);
					toPositionData   = GetHistoryData(currentTick);

					if (_settings.ForcePredictedLookRotation == true)
					{
						if (_lastPredictedLookRotationFrame < Time.frameCount)
						{
							LogError("Missing data for calculation of render look rotation for current frame. The KCC has Force Predicted Look Rotation enabled and must run render update for current frame before calling this method!");
							renderLookRotation = _renderData.LookRotation;
							return false;
						}

						fromRotationData = _fixedData;
						toRotationData   = _renderData;

						if (renderAlpha > _renderData.Alpha)
						{
							renderAlpha = 1.0f;
						}
						else if (_renderData.Alpha > 0.000001f)
						{
							renderAlpha = Mathf.Clamp01(renderAlpha / _renderData.Alpha);
						}
					}
					else
					{
						fromRotationData = fromPositionData;
						toRotationData   = toPositionData;
					}
				}
				else
				{
					throw new NotImplementedException(_settings.InputAuthorityBehavior.ToString());
				}
			}

			if (fromPositionData == null || toPositionData == null || fromRotationData == null || toRotationData == null)
			{
				renderLookRotation = Data.LookRotation;
				return false;
			}

			Vector3 fromPosition  = fromPositionData.TargetPosition;
			Vector3 toPosition    = toPositionData.TargetPosition;
			Vector3 positionDelta = toPosition - fromPosition;

			if (Vector3.SqrMagnitude(positionDelta) >= _settings.TeleportThreshold * _settings.TeleportThreshold)
			{
				if (renderAlpha >= 0.5f)
				{
					renderLookRotation = toRotationData.LookRotation;
				}
				else
				{
					renderLookRotation = fromRotationData.LookRotation;
				}
			}
			else
			{
				renderLookRotation = Quaternion.Slerp(fromRotationData.LookRotation, toRotationData.LookRotation, renderAlpha);
			}

			return true;
		}

		/// <summary>
		/// Returns render position and look rotation of the KCC with given render alpha.
		/// </summary>
		/// <param name="renderAlpha">Runner.LocalAlpha from the client at the time of Render().</param>
		/// <param name="renderPosition">Position of the KCC in render with given render alpha.</param>
		/// <param name="renderLookRotation">Look Rotation of the KCC in render with given render alpha.</param>
		public bool ResolveRenderPositionAndLookRotation(float renderAlpha, out Vector3 renderPosition, out Quaternion renderLookRotation)
		{
			renderAlpha = Mathf.Clamp01(renderAlpha);

			float positionAlpha = renderAlpha;
			float rotationAlpha = renderAlpha;

			KCCData fromPositionData;
			KCCData toPositionData;
			KCCData fromRotationData;
			KCCData toRotationData;

			if (IsInFixedUpdate == true)
			{
				int currentTick = Runner.Tick;

				if (_settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_PredictRender)
				{
					if (_lastPredictedFixedTick < currentTick)
					{
						LogError("Missing data for calculation of render position and look rotation for current tick. The KCC is set to predict in render and must run fixed update for current tick before calling this method!");
						renderPosition     = _fixedData.TargetPosition;
						renderLookRotation = _fixedData.LookRotation;
						return false;
					}

					fromPositionData = GetHistoryData(currentTick - 1);
					toPositionData   = GetHistoryData(currentTick);
					fromRotationData = fromPositionData;
					toRotationData   = toPositionData;
				}
				else if (_settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_InterpolateRender)
				{
					if (_settings.ForcePredictedLookRotation == true)
					{
						if (_lastPredictedFixedTick < currentTick)
						{
							LogError("Missing data for calculation of render position and look rotation for current tick. The KCC has Force Predicted Look Rotation enabled and must run fixed update for current tick before calling this method!");
							renderPosition     = _fixedData.TargetPosition;
							renderLookRotation = _fixedData.LookRotation;
							return false;
						}

						fromPositionData = GetHistoryData(currentTick - 2);
						toPositionData   = GetHistoryData(currentTick - 1);
						fromRotationData = toPositionData;
						toRotationData   = GetHistoryData(currentTick);
					}
					else
					{
						fromPositionData = GetHistoryData(currentTick - 2);
						toPositionData   = GetHistoryData(currentTick - 1);
						fromRotationData = fromPositionData;
						toRotationData   = toPositionData;
					}
				}
				else
				{
					throw new NotImplementedException(_settings.InputAuthorityBehavior.ToString());
				}
			}
			else
			{
				if (_settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_PredictRender)
				{
					if (_lastPredictedLookRotationFrame < Time.frameCount)
					{
						LogError("Missing data for calculation of render position and look rotation for current frame. The KCC is set to predict in render and must run render update for current frame before calling this method!");
						renderPosition     = _renderData.TargetPosition;
						renderLookRotation = _renderData.LookRotation;
						return false;
					}

					fromPositionData = _fixedData;
					toPositionData   = _renderData;
					fromRotationData = _fixedData;
					toRotationData   = _renderData;

					if (rotationAlpha > _renderData.Alpha)
					{
						rotationAlpha = 1.0f;
					}
					else if (_renderData.Alpha > 0.000001f)
					{
						rotationAlpha = Mathf.Clamp01(rotationAlpha / _renderData.Alpha);
					}

					positionAlpha = rotationAlpha;
				}
				else if (_settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_InterpolateRender)
				{
					int currentTick = Runner.Tick;

					fromPositionData = GetHistoryData(currentTick - 1);
					toPositionData   = GetHistoryData(currentTick);

					if (_settings.ForcePredictedLookRotation == true)
					{
						if (_lastPredictedLookRotationFrame < Time.frameCount)
						{
							LogError("Missing data for calculation of render position and look rotation for current frame. The KCC has Force Predicted Look Rotation enabled and must run render update for current frame before calling this method!");
							renderPosition     = _renderData.TargetPosition;
							renderLookRotation = _renderData.LookRotation;
							return false;
						}

						fromRotationData = _fixedData;
						toRotationData   = _renderData;

						if (rotationAlpha > _renderData.Alpha)
						{
							rotationAlpha = 1.0f;
						}
						else if (_renderData.Alpha > 0.000001f)
						{
							rotationAlpha = Mathf.Clamp01(rotationAlpha / _renderData.Alpha);
						}
					}
					else
					{
						fromRotationData = fromPositionData;
						toRotationData   = toPositionData;
					}
				}
				else
				{
					throw new NotImplementedException(_settings.InputAuthorityBehavior.ToString());
				}
			}

			if (fromPositionData == null || toPositionData == null || fromRotationData == null || toRotationData == null)
			{
				renderPosition     = Data.TargetPosition;
				renderLookRotation = Data.LookRotation;
				return false;
			}

			Vector3 fromPosition  = fromPositionData.TargetPosition;
			Vector3 toPosition    = toPositionData.TargetPosition;
			Vector3 positionDelta = toPosition - fromPosition;

			if (Vector3.SqrMagnitude(positionDelta) >= _settings.TeleportThreshold * _settings.TeleportThreshold)
			{
				if (positionAlpha >= 0.5f)
				{
					renderPosition     = toPosition;
					renderLookRotation = toRotationData.LookRotation;
				}
				else
				{
					renderPosition     = fromPosition;
					renderLookRotation = fromRotationData.LookRotation;
				}
			}
			else
			{
				Vector3 targetPosition;

				if (_activeFeatures.Has(EKCCFeature.AntiJitter) == true && _settings.AntiJitterDistance.IsZero() == false)
				{
					targetPosition = fromPosition;
					positionDelta  = Vector3.Lerp(fromPosition, toPosition, positionAlpha) - fromPosition;

					float distanceY = Mathf.Abs(positionDelta.y);
					if (distanceY > 0.000001f && distanceY > _settings.AntiJitterDistance.y)
					{
						targetPosition.y += positionDelta.y * ((distanceY - _settings.AntiJitterDistance.y) / distanceY);
					}

					Vector3 positionDeltaXZ = positionDelta.OnlyXZ();

					float distanceXZ = Vector3.Magnitude(positionDeltaXZ);
					if (distanceXZ > 0.000001f && distanceXZ > _settings.AntiJitterDistance.x)
					{
						targetPosition += positionDeltaXZ * ((distanceXZ - _settings.AntiJitterDistance.x) / distanceXZ);
					}
				}
				else
				{
					targetPosition = Vector3.Lerp(fromPosition, toPosition, positionAlpha);
				}

				renderPosition     = targetPosition;
				renderLookRotation = Quaternion.Slerp(fromRotationData.LookRotation, toRotationData.LookRotation, rotationAlpha);
			}

			return true;
		}

		/// <summary>
		/// Returns render position of a KCC child transform and render look rotation of the KCC with given render alpha.
		/// </summary>
		/// <param name="origin">Child transform of the KCC. For example a camera handle.</param>
		/// <param name="renderAlpha">Runner.LocalAlpha from a client at the time of Render().</param>
		/// <param name="renderPosition">Position of the origin in render with given render alpha.</param>
		/// <param name="renderLookRotation">Look rotation of the KCC in render with given render alpha.</param>
		public bool ResolveRenderPositionAndLookRotation(Transform origin, float renderAlpha, out Vector3 renderPosition, out Quaternion renderLookRotation)
		{
			if (ReferenceEquals(origin, _transform) == true)
				return ResolveRenderPositionAndLookRotation(renderAlpha, out renderPosition, out renderLookRotation);

			if (origin.IsChildOf(_transform) == false)
					throw new NotSupportedException($"Origin must be child of the KCC!");

			float positionAlpha = renderAlpha;
			float rotationAlpha = renderAlpha;

			KCCData fromPositionData;
			KCCData toPositionData;
			KCCData fromRotationData;
			KCCData toRotationData;

			if (IsInFixedUpdate == true)
			{
				int currentTick = Runner.Tick;

				if (_settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_PredictRender)
				{
					if (_lastPredictedFixedTick < currentTick)
					{
						LogError("Missing data for calculation of render position and look rotation for current tick. The KCC is set to predict in render and must run fixed update for current tick before calling this method!");
						renderPosition     = origin.position;
						renderLookRotation = _fixedData.LookRotation;
						return false;
					}

					fromPositionData = GetHistoryData(currentTick - 1);
					toPositionData   = GetHistoryData(currentTick);
					fromRotationData = fromPositionData;
					toRotationData   = toPositionData;
				}
				else if (_settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_InterpolateRender)
				{
					if (_settings.ForcePredictedLookRotation == true)
					{
						if (_lastPredictedFixedTick < currentTick)
						{
							LogError("Missing data for calculation of render position and look rotation for current tick. The KCC has Force Predicted Look Rotation enabled and must run fixed update for current tick before calling this method!");
							renderPosition     = origin.position;
							renderLookRotation = _fixedData.LookRotation;
							return false;
						}

						fromPositionData = GetHistoryData(currentTick - 2);
						toPositionData   = GetHistoryData(currentTick - 1);
						fromRotationData = toPositionData;
						toRotationData   = GetHistoryData(currentTick);
					}
					else
					{
						fromPositionData = GetHistoryData(currentTick - 2);
						toPositionData   = GetHistoryData(currentTick - 1);
						fromRotationData = fromPositionData;
						toRotationData   = toPositionData;
					}
				}
				else
				{
					throw new NotImplementedException(_settings.InputAuthorityBehavior.ToString());
				}
			}
			else
			{
				if (_settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_PredictRender)
				{
					if (_lastPredictedLookRotationFrame < Time.frameCount)
					{
						LogError("Missing data for calculation of render position and look rotation for current frame. The KCC is set to predict in render and must run render update for current frame before calling this method!");
						renderPosition     = origin.position;
						renderLookRotation = _renderData.LookRotation;
						return false;
					}

					fromPositionData = _fixedData;
					toPositionData   = _renderData;
					fromRotationData = _fixedData;
					toRotationData   = _renderData;

					if (rotationAlpha > _renderData.Alpha)
					{
						rotationAlpha = 1.0f;
					}
					else if (_renderData.Alpha > 0.000001f)
					{
						rotationAlpha = Mathf.Clamp01(rotationAlpha / _renderData.Alpha);
					}

					positionAlpha = rotationAlpha;
				}
				else if (_settings.InputAuthorityBehavior == EKCCAuthorityBehavior.PredictFixed_InterpolateRender)
				{
					int currentTick = Runner.Tick;

					fromPositionData = GetHistoryData(currentTick - 1);
					toPositionData   = GetHistoryData(currentTick);

					if (_settings.ForcePredictedLookRotation == true)
					{
						if (_lastPredictedLookRotationFrame < Time.frameCount)
						{
							LogError("Missing data for calculation of render position and look rotation for current frame. The KCC has Force Predicted Look Rotation enabled and must run render update for current frame before calling this method!");
							renderPosition     = origin.position;
							renderLookRotation = _renderData.LookRotation;
							return false;
						}

						fromRotationData = _fixedData;
						toRotationData   = _renderData;

						if (rotationAlpha > _renderData.Alpha)
						{
							rotationAlpha = 1.0f;
						}
						else if (_renderData.Alpha > 0.000001f)
						{
							rotationAlpha = Mathf.Clamp01(rotationAlpha / _renderData.Alpha);
						}
					}
					else
					{
						fromRotationData = fromPositionData;
						toRotationData   = toPositionData;
					}
				}
				else
				{
					throw new NotImplementedException(_settings.InputAuthorityBehavior.ToString());
				}
			}

			if (fromPositionData == null || toPositionData == null || fromRotationData == null || toRotationData == null)
			{
				renderPosition     = origin.position;
				renderLookRotation = Data.LookRotation;
				return false;
			}

			Vector3 fromPosition  = fromPositionData.TargetPosition;
			Vector3 toPosition    = toPositionData.TargetPosition;
			Vector3 positionDelta = toPosition - fromPosition;
			Vector3 offset        = _transform.InverseTransformPoint(origin.position);

			if (Vector3.SqrMagnitude(positionDelta) >= _settings.TeleportThreshold * _settings.TeleportThreshold)
			{
				if (positionAlpha >= 0.5f)
				{
					renderPosition     = toPosition + toPositionData.TransformRotation * offset;
					renderLookRotation = toRotationData.LookRotation;
				}
				else
				{
					renderPosition     = fromPosition + fromPositionData.TransformRotation * offset;
					renderLookRotation = fromRotationData.LookRotation;
				}
			}
			else
			{
				Vector3 fromOffset = fromPositionData.TransformRotation * offset;
				Vector3 toOffset   = toPositionData.TransformRotation   * offset;

				Vector3 targetPosition;

				if (_activeFeatures.Has(EKCCFeature.AntiJitter) == true && _settings.AntiJitterDistance.IsZero() == false)
				{
					targetPosition = fromPosition;
					positionDelta  = Vector3.Lerp(fromPosition, toPosition, positionAlpha) - fromPosition;

					float distanceY = Mathf.Abs(positionDelta.y);
					if (distanceY > 0.000001f && distanceY > _settings.AntiJitterDistance.y)
					{
						targetPosition.y += positionDelta.y * ((distanceY - _settings.AntiJitterDistance.y) / distanceY);
					}

					Vector3 positionDeltaXZ = positionDelta.OnlyXZ();

					float distanceXZ = Vector3.Magnitude(positionDeltaXZ);
					if (distanceXZ > 0.000001f && distanceXZ > _settings.AntiJitterDistance.x)
					{
						targetPosition += positionDeltaXZ * ((distanceXZ - _settings.AntiJitterDistance.x) / distanceXZ);
					}
				}
				else
				{
					targetPosition = Vector3.Lerp(fromPosition, toPosition, positionAlpha);
				}

				renderPosition     = targetPosition + Vector3.Slerp(fromOffset, toOffset, positionAlpha);
				renderLookRotation = Quaternion.Slerp(fromRotationData.LookRotation, toRotationData.LookRotation, rotationAlpha);
			}

			return true;
		}
	}
}
