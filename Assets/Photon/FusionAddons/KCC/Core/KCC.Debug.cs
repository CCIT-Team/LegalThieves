namespace Fusion.Addons.KCC
{
	using UnityEngine;
	using System.Runtime.CompilerServices;

	// This file contains utilities for debugging.
	public partial class KCC
	{
		// PRIVATE MEMBERS

		private static readonly Vector3[] _spherePoints          = MakeUnitSphere(24);
		private static readonly Vector3[] _lowerHemiSpherePoints = MakeLowerUnitHemiSphere(24);
		private static readonly Vector3[] _upperHemiSpherePoints = MakeUpperUnitHemiSphere(24);

		// PUBLIC METHODS

		/// <summary>
		/// Logs current KCC and KCCData state.
		/// </summary>
		[HideInCallstack]
		public void Dump()
		{
			_debug.Dump(this);
		}

		/// <summary>
		/// Enable logs for a given duration. This outputs same logs as <c>Dump()</c>.
		/// <param name="duration">Duration of logs. Use negative value for infinite logging.</param>
		/// </summary>
		public void EnableLogs(float duration = -1.0f)
		{
			_debug.EnableLogs(this, duration);
		}

		/// <summary>
		/// Logs info message into console with frame/tick metadata.
		/// </summary>
		/// <param name="messages">Custom message objects.</param>
		[HideInCallstack]
		public void Log(params object[] messages)
		{
			KCCUtility.Log(this, default, EKCCLogType.Info, messages);
		}

		/// <summary>
		/// Logs warning message into console with frame/tick metadata.
		/// </summary>
		/// <param name="messages">Custom message objects.</param>
		[HideInCallstack]
		public void LogWarning(params object[] messages)
		{
			KCCUtility.Log(this, default, EKCCLogType.Warning, messages);
		}

		/// <summary>
		/// Logs error message into console with frame/tick metadata.
		/// </summary>
		/// <param name="messages">Custom message objects.</param>
		[HideInCallstack]
		public void LogError(params object[] messages)
		{
			KCCUtility.Log(this, default, EKCCLogType.Error, messages);
		}

		/// <summary>
		/// Draws debug line in editor scene view.
		/// The color is yellow (forward tick), red (resimulation tick) or green (render).
		/// </summary>
		/// <param name="duration">Duration of the drawing.</param>
		public void DrawLine(float duration = 0.0f)
		{
			Color   color;
			Vector3 position;

			NetworkRunner runner = Runner;
			if (runner != null)
			{
				bool isInFixedUpdate = runner.Stage != default;
				bool isInForwardTick = runner.IsForward == true;

				if (isInFixedUpdate == true)
				{
					position = _fixedData.TargetPosition;

					if (isInForwardTick == true)
					{
						color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
					}
					else
					{
						color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
					}
				}
				else
				{
					position = _renderData.TargetPosition;
					color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
				}
			}
			else
			{
				position = _transform.position;
				color = new Color(0.0f, 0.0f, 1.0f, 1.0f);
			}

			UnityEngine.Debug.DrawLine(position, position + Vector3.up, color, duration);
		}

		/// <summary>
		/// Draws debug line in editor scene view.
		/// </summary>
		/// <param name="color">Color of the line.</param>
		/// <param name="duration">Duration of the drawing.</param>
		public void DrawLine(Color color, float duration = 0.0f)
		{
			Vector3 position = Data.TargetPosition;
			UnityEngine.Debug.DrawLine(position, position + Vector3.up, color, duration);
		}

		/// <summary>
		/// Draws debug sphere in editor scene view, using radius defined in KCC Settings.
		/// </summary>
		/// <param name="position">Base position of the sphere.</param>
		/// <param name="color">Color of the sphere.</param>
		/// <param name="duration">Duration of the drawing.</param>
		public void DrawSphere(Vector3 position, Color color, float duration = 0.0f)
		{
			DrawSphere(position, _settings.Radius, color, duration);
		}

		/// <summary>
		/// Draws debug sphere in editor scene view.
		/// </summary>
		/// <param name="position">Base position of the sphere.</param>
		/// <param name="radius">Radius of the sphere.</param>
		/// <param name="color">Color of the sphere.</param>
		/// <param name="duration">Duration of the drawing.</param>
		public void DrawSphere(Vector3 position, float radius, Color color, float duration = 0.0f)
		{
			if (radius <= 0.0f)
				return;

			DrawWireSphere(position, radius, color, duration);
		}

		/// <summary>
		/// Draws debug capsule in editor scene view, using radius and height defined in KCC Settings.
		/// </summary>
		/// <param name="position">Base position of the capsule.</param>
		/// <param name="color">Color of the capsule.</param>
		/// <param name="duration">Duration of the drawing.</param>
		public void DrawCapsule(Vector3 position, Color color, float duration = 0.0f)
		{
			DrawCapsule(position, _settings.Radius, _settings.Height, color, duration);
		}

		/// <summary>
		/// Draws debug capsule in editor scene view.
		/// </summary>
		/// <param name="position">Base position of the capsule.</param>
		/// <param name="radius">Radius of the capsule.</param>
		/// <param name="height">Height of the capsule.</param>
		/// <param name="color">Color of the capsule.</param>
		/// <param name="duration">Duration of the drawing.</param>
		public void DrawCapsule(Vector3 position, float radius, float height, Color color, float duration = 0.0f)
		{
			if (radius <= 0.0f)
				return;

			height = Mathf.Max(radius * 2.0f, height);

			Vector3 baseLow     = position + Vector3.up * radius;
			Vector3 baseHigh    = position + Vector3.up * (height - radius);
			Vector3 offsetFront = Vector3.forward * radius;
			Vector3 offsetBack  = Vector3.back    * radius;
			Vector3 offsetLeft  = Vector3.left    * radius;
			Vector3 offsetRight = Vector3.right   * radius;

			DrawWireHemiSphere(_lowerHemiSpherePoints, baseLow, radius, color, duration);
			DrawWireHemiSphere(_upperHemiSpherePoints, baseHigh, radius, color, duration);

			UnityEngine.Debug.DrawLine(baseLow + offsetFront, baseHigh + offsetFront, color, duration);
			UnityEngine.Debug.DrawLine(baseLow + offsetBack,  baseHigh + offsetBack, color, duration);
			UnityEngine.Debug.DrawLine(baseLow + offsetLeft,  baseHigh + offsetLeft, color, duration);
			UnityEngine.Debug.DrawLine(baseLow + offsetRight, baseHigh + offsetRight, color, duration);
		}

		// PRIVATE METHODS

		[HideInCallstack]
		[System.Diagnostics.Conditional(TRACING_SCRIPT_DEFINE)]
		private void Trace(params object[] messages)
		{
			KCCUtility.Trace<KCC>(this, messages);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool CheckSpawned()
		{
			if (_isSpawned == false)
			{
				LogError($"{nameof(KCC)}.{nameof(Spawned)}() has not been called yet! Use {nameof(KCC)}.{nameof(InvokeOnSpawn)}() to register a callback.", this);
				return false;
			}

			return true;
		}

		private static void DrawWireSphere(Vector3 position, float radius, Color color, float duration = 0.0f)
		{
			Vector3[] points = _spherePoints;

			int length = points.Length / 3;
			for (int i = 0; i < length; i++)
			{
				int j = (i + 1) % length;

				Vector3 startX = position + radius * points[length * 0 + i];
				Vector3 endX   = position + radius * points[length * 0 + j];
				Vector3 startY = position + radius * points[length * 1 + i];
				Vector3 endY   = position + radius * points[length * 1 + j];
				Vector3 startZ = position + radius * points[length * 2 + i];
				Vector3 endZ   = position + radius * points[length * 2 + j];

				UnityEngine.Debug.DrawLine(startX, endX, color, duration);
				UnityEngine.Debug.DrawLine(startY, endY, color, duration);
				UnityEngine.Debug.DrawLine(startZ, endZ, color, duration);
			}
		}

		private static void DrawWireHemiSphere(Vector3[] points, Vector3 position, float radius, Color color, float duration = 0.0f)
		{
			int length = points.Length / 3;
			for (int i = 0; i < length - 1; i++)
			{
				int j = (i + 1) % length;

				Vector3 startX = position + radius * points[length * 0 + i];
				Vector3 endX   = position + radius * points[length * 0 + j];
				Vector3 startY = position + radius * points[length * 1 + i];
				Vector3 endY   = position + radius * points[length * 1 + j];
				Vector3 startZ = position + radius * points[length * 2 + i];
				Vector3 endZ   = position + radius * points[length * 2 + j];

				UnityEngine.Debug.DrawLine(startX, endX, color, duration);
				UnityEngine.Debug.DrawLine(startY, endY, color, duration);
				UnityEngine.Debug.DrawLine(startZ, endZ, color, duration);
			}
		}

		private static Vector3[] MakeUnitSphere(int length)
		{
			Vector3[] points = new Vector3[length * 3];
			for (int i = 0; i < length; i++)
			{
				float f = i / (float)length;
				float c = Mathf.Cos(f * (float)(System.Math.PI * 2.0f));
				float s = Mathf.Sin(f * (float)(System.Math.PI * 2.0f));

				points[length * 0 + i] = new Vector3(c, s, 0);
				points[length * 1 + i] = new Vector3(0, c, s);
				points[length * 2 + i] = new Vector3(s, 0, c);
			}

			return points;
		}

		private static Vector3[] MakeLowerUnitHemiSphere(int length)
		{
			int count = length + 1;

			Vector3[] points = new Vector3[count * 3];
			for (int i = 0; i < count; i++)
			{
				float f  = i / (float)length;
				float c1 = Mathf.Cos(Mathf.PI * f * 2.0f);
				float s1 = Mathf.Sin(Mathf.PI * f * 2.0f);
				float c2 = Mathf.Cos(Mathf.PI * (f + 0.5f));
				float s2 = Mathf.Sin(Mathf.PI * (f + 0.5f));
				float c3 = Mathf.Cos(Mathf.PI * (f + 1.0f));
				float s3 = Mathf.Sin(Mathf.PI * (f + 1.0f));

				points[count * 0 + i] = new Vector3(c3, s3, 0);
				points[count * 1 + i] = new Vector3(0, c2, s2);
				points[count * 2 + i] = new Vector3(s1, 0, c1);
			}

			return points;
		}

		private static Vector3[] MakeUpperUnitHemiSphere(int length)
		{
			int count = length + 1;

			Vector3[] points = new Vector3[count * 3];
			for (int i = 0; i < count; i++)
			{
				float f  = i / (float)length;
				float c1 = Mathf.Cos(Mathf.PI * f * 2.0f);
				float s1 = Mathf.Sin(Mathf.PI * f * 2.0f);
				float c2 = Mathf.Cos(Mathf.PI * (f - 0.5f));
				float s2 = Mathf.Sin(Mathf.PI * (f - 0.5f));
				float c3 = Mathf.Cos(Mathf.PI * f);
				float s3 = Mathf.Sin(Mathf.PI * f);

				points[count * 0 + i] = new Vector3(c3, s3, 0);
				points[count * 1 + i] = new Vector3(0, c2, s2);
				points[count * 2 + i] = new Vector3(s1, 0, c1);
			}

			return points;
		}
	}
}
