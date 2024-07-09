namespace Fusion.Addons.KCC
{
	using System;
	using System.Text;
	using UnityEngine;

	/// <summary>
	/// Debug configuration, logging, tracking, visualization.
	/// </summary>
	public sealed class KCCDebug
	{
		// PUBLIC MEMBERS

		public float          LogsTime;
		public bool           ShowPath;
		public bool           ShowSpeed;
		public bool           ShowGrounding;
		public bool           ShowSteppingUp;
		public bool           ShowGroundSnapping;
		public bool           ShowGroundNormal;
		public bool           ShowGroundTangent;
		public bool           ShowMoveDirection;
		public bool           TraceExecution;
		public int            TraceInfoCount;
		public KCCTraceInfo[] TraceInfos = new KCCTraceInfo[0];
		public float          DisplayTime = 30.0f;
		public float          SpeedScale  = 0.1f;
		public float          PointSize   = 0.01f;

		public static readonly Color FixedPathColor            = Color.red;
		public static readonly Color RenderPathColor           = Color.green;
		public static readonly Color FixedToRenderPathColor    = Color.blue;
		public static readonly Color PredictionCorrectionColor = Color.yellow;
		public static readonly Color PredictionErrorColor      = Color.magenta;
		public static readonly Color IsGroundedColor           = Color.green;
		public static readonly Color WasGroundedColor          = Color.red;
		public static readonly Color SpeedColor                = Color.green;
		public static readonly Color IsSteppingUpColor         = Color.green;
		public static readonly Color WasSteppingUpColor        = Color.red;
		public static readonly Color GroundNormalColor         = Color.magenta;
		public static readonly Color GroundTangentColor        = Color.yellow;
		public static readonly Color GroundSnapingColor        = Color.cyan;
		public static readonly Color GroundSnapTargetColor     = Color.blue;
		public static readonly Color GroundSnapPositionColor   = Color.red;
		public static readonly Color MoveDirectionColor        = Color.yellow;

		// PRIVATE MEMBERS

		public StringBuilder _stringBuilder = new StringBuilder(1024);

		// PUBLIC METHODS

		public void SetDefaults()
		{
			LogsTime           = default;
			ShowPath           = default;
			ShowSpeed          = default;
			ShowGrounding      = default;
			ShowSteppingUp     = default;
			ShowGroundSnapping = default;
			ShowGroundNormal   = default;
			ShowGroundTangent  = default;
			ShowMoveDirection  = default;
			TraceExecution     = default;
			TraceInfoCount     = default;
			DisplayTime        = 30.0f;
			SpeedScale         = 0.1f;
			PointSize          = 0.01f;

			if (TraceInfos.Length > 0)
			{
				Array.Clear(TraceInfos, 0, TraceInfos.Length);
			}
		}

		public void BeforePredictedFixedMove(KCC kcc)
		{
			TraceInfoCount = default;
		}

		public void AfterPredictedFixedMove(KCC kcc)
		{
#if UNITY_EDITOR
			if (ShowPath == true)
			{
				KCCData fixedData = kcc.FixedData;

				UnityEngine.Debug.DrawLine(fixedData.BasePosition, fixedData.TargetPosition, FixedPathColor, DisplayTime);

				DrawPoint(fixedData.TargetPosition, FixedPathColor, PointSize, DisplayTime);
			}
#endif

			if (LogsTime != default)
			{
				if (LogsTime > 0.0f && Time.realtimeSinceStartup >= LogsTime)
				{
					LogsTime = default;
				}

				Log(kcc, true);
			}
		}

		public void AfterRenderUpdate(KCC kcc)
		{
#if UNITY_EDITOR
			KCCData fixedData  = kcc.FixedData;
			KCCData renderData = kcc.RenderData;

			if (ShowPath == true)
			{
				if (kcc.IsPredictingInRenderUpdate == true)
				{
					UnityEngine.Debug.DrawLine(renderData.BasePosition, renderData.TargetPosition, RenderPathColor, DisplayTime);
				}

				DrawPoint(renderData.TargetPosition, RenderPathColor, PointSize, DisplayTime);
			}

			KCCData selectedData = kcc.Object.IsInSimulation == true ? fixedData : renderData;

			if (ShowSpeed == true)
			{
				UnityEngine.Debug.DrawLine(selectedData.TargetPosition, selectedData.TargetPosition + Vector3.up * selectedData.RealVelocity.magnitude * SpeedScale, SpeedColor, DisplayTime);
			}

			if (ShowGrounding == true)
			{
				if (selectedData.IsGrounded == true && selectedData.WasGrounded == false)
				{
					UnityEngine.Debug.DrawLine(selectedData.TargetPosition, selectedData.TargetPosition + Vector3.up, IsGroundedColor, DisplayTime);
				}
				else if (selectedData.IsGrounded == false && selectedData.WasGrounded == true)
				{
					UnityEngine.Debug.DrawLine(selectedData.BasePosition, selectedData.BasePosition + Vector3.up, WasGroundedColor, DisplayTime);
				}
			}

			if (ShowSteppingUp == true)
			{
				if (selectedData.IsSteppingUp == true && selectedData.WasSteppingUp == false)
				{
					UnityEngine.Debug.DrawLine(selectedData.TargetPosition, selectedData.TargetPosition + Vector3.up, IsSteppingUpColor, DisplayTime);
				}
				else if (selectedData.IsSteppingUp == false && selectedData.WasSteppingUp == true)
				{
					UnityEngine.Debug.DrawLine(selectedData.TargetPosition, selectedData.TargetPosition + Vector3.up, WasSteppingUpColor, DisplayTime);
				}
			}

			if (ShowGroundNormal  == true) { UnityEngine.Debug.DrawLine(selectedData.TargetPosition, selectedData.TargetPosition + selectedData.GroundNormal,                     GroundNormalColor,  DisplayTime); }
			if (ShowGroundTangent == true) { UnityEngine.Debug.DrawLine(selectedData.TargetPosition, selectedData.TargetPosition + selectedData.GroundTangent,                    GroundTangentColor, DisplayTime); }
			if (ShowMoveDirection == true) { UnityEngine.Debug.DrawLine(selectedData.TargetPosition, selectedData.TargetPosition + selectedData.RealVelocity.ClampToNormalized(), MoveDirectionColor, DisplayTime); }
#endif

			if (LogsTime != default)
			{
				if (LogsTime > 0.0f && Time.realtimeSinceStartup >= LogsTime)
				{
					LogsTime = default;
				}

				Log(kcc, false);
			}
		}

		public void DrawGroundSnapping(Vector3 targetPosition, Vector3 targetGroundedPosition, Vector3 targetSnappedPosition, bool isInFixedUpdate)
		{
			if (isInFixedUpdate == false)
				return;
			if (ShowGroundSnapping == false)
				return;

			UnityEngine.Debug.DrawLine(targetPosition, targetPosition + Vector3.up, GroundSnapingColor, DisplayTime);
			UnityEngine.Debug.DrawLine(targetPosition, targetGroundedPosition, GroundSnapTargetColor, DisplayTime);
			UnityEngine.Debug.DrawLine(targetPosition, targetSnappedPosition, GroundSnapPositionColor, DisplayTime);
		}

		public bool TraceStage(KCC kcc, Type type, int level)
		{
			if (TraceExecution == false)
				return false;
			if (kcc.IsInFixedUpdate == false)
				return false;

			if (TraceInfoCount >= TraceInfos.Length)
			{
				Array.Resize(ref TraceInfos, TraceInfos.Length + KCC.CACHE_SIZE);
			}

			KCCTraceInfo traceInfo = TraceInfos[TraceInfoCount];
			if (traceInfo == null)
			{
				traceInfo = new KCCTraceInfo();
				TraceInfos[TraceInfoCount] = traceInfo;
			}

			traceInfo.Set(EKCCTrace.Stage, type, type.Name, level, default);
			++TraceInfoCount;

			return true;
		}

		public bool TraceStage(KCC kcc, Type type, string name, int level)
		{
			if (TraceExecution == false)
				return false;
			if (kcc.IsInFixedUpdate == false)
				return false;

			if (TraceInfoCount >= TraceInfos.Length)
			{
				Array.Resize(ref TraceInfos, TraceInfos.Length + KCC.CACHE_SIZE);
			}

			KCCTraceInfo traceInfo = TraceInfos[TraceInfoCount];
			if (traceInfo == null)
			{
				traceInfo = new KCCTraceInfo();
				TraceInfos[TraceInfoCount] = traceInfo;
			}

			traceInfo.Set(EKCCTrace.Stage, type, name, level, default);
			++TraceInfoCount;

			return true;
		}

		public bool TraceProcessor(IKCCProcessor processor, int level)
		{
			if (TraceExecution == false)
				return false;

			if (TraceInfoCount >= TraceInfos.Length)
			{
				Array.Resize(ref TraceInfos, TraceInfos.Length + KCC.CACHE_SIZE);
			}

			KCCTraceInfo traceInfo = TraceInfos[TraceInfoCount];
			if (traceInfo == null)
			{
				traceInfo = new KCCTraceInfo();
				TraceInfos[TraceInfoCount] = traceInfo;
			}

			traceInfo.Set(EKCCTrace.Processor, default, default, level, processor);
			++TraceInfoCount;

			return true;
		}

		[HideInCallstack]
		public void Dump(KCC kcc)
		{
			Log(kcc, kcc.IsInFixedUpdate);
		}

		public void EnableLogs(KCC kcc, float duration)
		{
			if (duration == default)
			{
				LogsTime = default;
			}
			else if (duration >= 0.0f)
			{
				LogsTime = Time.realtimeSinceStartup + duration;
			}
			else
			{
				LogsTime = -1.0f;
			}
		}

		// PRIVATE METHODS

		[HideInCallstack]
		private void Log(KCC kcc, bool isInFixedUpdate)
		{
			KCCData data = kcc.Data;

			_stringBuilder.Clear();

			{
				_stringBuilder.Append($" | {nameof(data.Alpha)               } {data.Alpha.ToString("F4")               }");
				_stringBuilder.Append($" | {nameof(data.Time)                } {data.Time.ToString("F6")                }");
				_stringBuilder.Append($" | {nameof(data.DeltaTime)           } {data.DeltaTime.ToString("F6")           }");

				_stringBuilder.Append($" | {nameof(data.BasePosition)        } {data.BasePosition.ToString("F4")        }");
				_stringBuilder.Append($" | {nameof(data.DesiredPosition)     } {data.DesiredPosition.ToString("F4")     }");
				_stringBuilder.Append($" | {nameof(data.TargetPosition)      } {data.TargetPosition.ToString("F4")      }");
				_stringBuilder.Append($" | {nameof(data.LookPitch)           } {data.LookPitch.ToString("0.00°")        }");
				_stringBuilder.Append($" | {nameof(data.LookYaw)             } {data.LookYaw.ToString("0.00°")          }");

				_stringBuilder.Append($" | {nameof(data.InputDirection)      } {data.InputDirection.ToString("F4")      }");
				_stringBuilder.Append($" | {nameof(data.DynamicVelocity)     } {data.DynamicVelocity.ToString("F4")     }");
				_stringBuilder.Append($" | {nameof(data.KinematicSpeed)      } {data.KinematicSpeed.ToString("F4")      }");
				_stringBuilder.Append($" | {nameof(data.KinematicTangent)    } {data.KinematicTangent.ToString("F4")    }");
				_stringBuilder.Append($" | {nameof(data.KinematicDirection)  } {data.KinematicDirection.ToString("F4")  }");
				_stringBuilder.Append($" | {nameof(data.KinematicVelocity)   } {data.KinematicVelocity.ToString("F4")   }");

				_stringBuilder.Append($" | {nameof(data.IsGrounded)          } {(data.IsGrounded          ? "1" : "0")  }");
				_stringBuilder.Append($" | {nameof(data.WasGrounded)         } {(data.WasGrounded         ? "1" : "0")  }");
				_stringBuilder.Append($" | {nameof(data.IsOnEdge)            } {(data.IsOnEdge            ? "1" : "0")  }");
				_stringBuilder.Append($" | {nameof(data.IsSteppingUp)        } {(data.IsSteppingUp        ? "1" : "0")  }");
				_stringBuilder.Append($" | {nameof(data.WasSteppingUp)       } {(data.WasSteppingUp       ? "1" : "0")  }");
				_stringBuilder.Append($" | {nameof(data.IsSnappingToGround)  } {(data.IsSnappingToGround  ? "1" : "0")  }");
				_stringBuilder.Append($" | {nameof(data.WasSnappingToGround) } {(data.WasSnappingToGround ? "1" : "0")  }");
				_stringBuilder.Append($" | {nameof(data.JumpFrames)          } {data.JumpFrames.ToString()              }");
				_stringBuilder.Append($" | {nameof(data.HasJumped)           } {(data.HasJumped           ? "1" : "0")  }");
				_stringBuilder.Append($" | {nameof(data.HasTeleported)       } {(data.HasTeleported       ? "1" : "0")  }");

				_stringBuilder.Append($" | {nameof(data.GroundNormal)        } {data.GroundNormal.ToString("F4")        }");
				_stringBuilder.Append($" | {nameof(data.GroundTangent)       } {data.GroundTangent.ToString("F4")       }");
				_stringBuilder.Append($" | {nameof(data.GroundPosition)      } {data.GroundPosition.ToString("F4")      }");
				_stringBuilder.Append($" | {nameof(data.GroundDistance)      } {data.GroundDistance.ToString("F4")      }");
				_stringBuilder.Append($" | {nameof(data.GroundAngle)         } {data.GroundAngle.ToString("0.00°")      }");

				_stringBuilder.Append($" | {nameof(data.RealSpeed)           } {data.RealSpeed.ToString("F4")           }");
				_stringBuilder.Append($" | {nameof(data.RealVelocity)        } {data.RealVelocity.ToString("F4")        }");

				_stringBuilder.Append($" | {nameof(data.Collisions)          } {data.Collisions.Count.ToString()        }");
				_stringBuilder.Append($" | {nameof(data.Modifiers)           } {data.Modifiers.Count.ToString()         }");
				_stringBuilder.Append($" | {nameof(data.Ignores)             } {data.Ignores.Count.ToString()           }");
				_stringBuilder.Append($" | {nameof(data.Hits)                } {data.Hits.Count.ToString()              }");
			}

			if (isInFixedUpdate == false)
			{
				_stringBuilder.Append($" | {nameof(kcc.PredictionError)      } {kcc.PredictionError.ToString("F4")      }");
			}

			kcc.Log(_stringBuilder.ToString());
		}

		private static void DrawPoint(Vector3 position, Color color, float size, float displayTime)
		{
			Vector3 pX = position + new Vector3( size,  0.0f,  0.0f);
			Vector3 nX = position + new Vector3(-size,  0.0f,  0.0f);
			Vector3 pY = position + new Vector3( 0.0f,  size,  0.0f);
			Vector3 nY = position + new Vector3( 0.0f, -size,  0.0f);
			Vector3 pZ = position + new Vector3( 0.0f,  0.0f,  size);
			Vector3 nZ = position + new Vector3( 0.0f,  0.0f, -size);

			UnityEngine.Debug.DrawLine(pY, pX, color, displayTime);
			UnityEngine.Debug.DrawLine(pY, nX, color, displayTime);
			UnityEngine.Debug.DrawLine(pY, pZ, color, displayTime);
			UnityEngine.Debug.DrawLine(pY, nZ, color, displayTime);

			UnityEngine.Debug.DrawLine(nY, pX, color, displayTime);
			UnityEngine.Debug.DrawLine(nY, nX, color, displayTime);
			UnityEngine.Debug.DrawLine(nY, pZ, color, displayTime);
			UnityEngine.Debug.DrawLine(nY, nZ, color, displayTime);
		}
	}
}
