namespace Fusion.Addons.KCC
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Utility for calculation single depenetration vector based on multiple unrelated vectors.
	/// Starting point is sum of min and max components of these vectors, putting the target vector close to correct position and minimising number of iterations.
	/// This method uses gradient descent algorithm with sum of absolute errors as function to find the local minimum.
	/// This approach has good results on depenetration vectors with various normals, but fails on corrections with same direction but different correction distance.
	/// For best results use at least 2 compute penetration passes and apply target correction fully in last pass only.
	/// </summary>
	public sealed class KCCResolver
	{
		// PUBLIC MEMBERS

		/// <summary>Count of input corrections.</summary>
		public int Count => _count;

		// PRIVATE MEMBERS

		private int          _count;
		private Correction[] _corrections;

		// CONSTRUCTORS

		public KCCResolver(int maxSize)
		{
			_corrections = new Correction[maxSize];
			for (int i = 0; i < maxSize; ++i)
			{
				_corrections[i] = new Correction();
			}
		}

		// PUBLIC METHODS

		/// <summary>
		/// Resets resolver. Call this before adding corrections.
		/// </summary>
		public void Reset()
		{
			_count = default;
		}

		/// <summary>
		/// Adds single correction vector.
		/// </summary>
		public void AddCorrection(Vector3 direction, float distance)
		{
			Correction correction = _corrections[_count];

			correction.Amount    = direction * distance;
			correction.Direction = direction;
			correction.Distance  = distance;
			correction.Error     = default;

			++_count;
		}

		/// <summary>
		/// Returns correction at specific index.
		/// </summary>
		public Vector3 GetCorrection(int index)
		{
			return _corrections[index].Amount;
		}

		/// <summary>
		/// Returns correction amount and direction at specific index.
		/// </summary>
		public Vector3 GetCorrection(int index, out Vector3 direction)
		{
			Correction correction = _corrections[index];
			direction = correction.Direction;
			return correction.Amount;
		}

		/// <summary>
		/// Returns correction amount, direction and distance at specific index.
		/// </summary>
		public Vector3 GetCorrection(int index, out Vector3 direction, out float distance)
		{
			Correction correction = _corrections[index];
			direction = correction.Direction;
			distance  = correction.Distance;
			return correction.Amount;
		}

		/// <summary>
		/// Calculates target correction vector based on added corrections.
		/// </summary>
		public Vector3 CalculateBest(int maxIterations, float maxError)
		{
			if (_count <= 0)
				return default;
			if (_count == 1)
				return _corrections[0].Amount;

			if (_count == 2)
			{
				Correction correction0 = _corrections[0];
				Correction correction1 = _corrections[1];

				if (Vector3.Dot(correction0.Direction, correction1.Direction) < 0.0f)
				{
					return CalculateBinary(correction0, correction1);
				}
			}

			if (_count == 3)
			{
				Correction correction0 = _corrections[0];
				Correction correction1 = _corrections[1];
				Correction correction2 = _corrections[2];

				float absUpDot0 = Mathf.Abs(Vector3.Dot(correction0.Direction, Vector3.up));
				float absUpDot1 = Mathf.Abs(Vector3.Dot(correction1.Direction, Vector3.up));
				float absUpDot2 = Mathf.Abs(Vector3.Dot(correction2.Direction, Vector3.up));

				const float wallThreshold  = 0.025f;
				const float floorThreshold = 0.9995f;

				if (absUpDot0 > floorThreshold)
				{
					if (absUpDot1 < wallThreshold && absUpDot2 < wallThreshold && Vector3.Dot(correction1.Direction, correction2.Direction) < 0.0f)
					{
						return CalculateMinMax(correction0.Amount, CalculateBinary(correction1, correction2));
					}
				}
				else if (absUpDot1 > floorThreshold)
				{
					if (absUpDot0 < wallThreshold && absUpDot2 < wallThreshold && Vector3.Dot(correction0.Direction, correction2.Direction) < 0.0f)
					{
						return CalculateMinMax(correction1.Amount, CalculateBinary(correction0, correction2));
					}
				}
				else if (absUpDot2 > floorThreshold)
				{
					if (absUpDot0 < wallThreshold && absUpDot1 < wallThreshold && Vector3.Dot(correction0.Direction, correction1.Direction) < 0.0f)
					{
						return CalculateMinMax(correction2.Amount, CalculateBinary(correction0, correction1));
					}
				}
			}

			return CalculateErrorDescent(maxIterations, maxError);
		}

		/// <summary>
		/// Calculates target correction vector based on added corrections.
		/// </summary>
		public Vector3 CalculateMinMax()
		{
			if (_count <= 0)
				return default;
			if (_count == 1)
				return _corrections[0].Amount;

			Vector3 minCorrection = default;
			Vector3 maxCorrection = default;

			for (int i = 0; i < _count; ++i)
			{
				Correction correction = _corrections[i];

				minCorrection = Vector3.Min(minCorrection, correction.Amount);
				maxCorrection = Vector3.Max(maxCorrection, correction.Amount);
			}

			return minCorrection + maxCorrection;
		}

		/// <summary>
		/// Calculates target correction vector based on added corrections.
		/// </summary>
		public Vector3 CalculateSum()
		{
			if (_count <= 0)
				return default;
			if (_count == 1)
				return _corrections[0].Amount;

			Vector3 targetCorrection = default;

			for (int i = 0; i < _count; ++i)
			{
				targetCorrection += _corrections[i].Amount;
			}

			return targetCorrection;
		}

		/// <summary>
		/// Calculates target correction vector based on added corrections.
		/// </summary>
		public Vector3 CalculateAverage()
		{
			if (_count <= 0)
				return default;
			if (_count == 1)
				return _corrections[0].Amount;

			return CalculateSum() / _count;
		}

		/// <summary>
		/// Calculates target correction vector based on added corrections.
		/// </summary>
		public Vector3 CalculateBinary()
		{
			if (_count <= 0)
				return default;
			if (_count == 1)
				return _corrections[0].Amount;
			if (_count > 2)
				throw new InvalidOperationException("Count of corrections must be 2 at max!");

			return CalculateBinary(_corrections[0], _corrections[1]);
		}

		/// <summary>
		/// Calculates target correction vector based on added corrections.
		/// </summary>
		public Vector3 CalculateErrorDescent(int maxIterations, float maxError)
		{
			if (_count <= 0)
				return default;
			if (_count == 1)
				return _corrections[0].Amount;

			int     iterations       = default;
			Vector3 targetCorrection = default;

			while (iterations < maxIterations)
			{
				++iterations;

				float accumulatedError = 0.0f;

				for (int i = 0; i < _count; ++i)
				{
					Correction correction = _corrections[i];

					float error = correction.Distance - Vector3.Dot(targetCorrection, correction.Direction);
					if (error > 0.0f)
					{
						accumulatedError += error;
						targetCorrection += error * correction.Direction;
					}
				}

				if(accumulatedError < maxError)
					break;
			}

			return targetCorrection;
		}

		/// <summary>
		/// Calculates target correction vector based on added corrections.
		/// </summary>
		public Vector3 CalculateGradientDescent(int maxIterations, float maxError)
		{
			if (_count <= 0)
				return default;
			if (_count == 1)
				return _corrections[0].Amount;

			Vector3      error;
			float        errorDot;
			float        errorCorrection;
			float        errorCorrectionSize;
			Vector3      targetCorrection = default;
			int          iterations = default;

			while (iterations < maxIterations)
			{
				++iterations;

				error               = default;
				errorCorrection     = default;
				errorCorrectionSize = default;

				for (int i = 0; i < _count; ++i)
				{
					Correction correction = _corrections[i];

					// Calculate error of desired correction relative to single correction.
					correction.Error = correction.Direction.x * targetCorrection.x + correction.Direction.y * targetCorrection.y + correction.Direction.z * targetCorrection.z - correction.Distance;

					// Accumulate error of all corrections.
					error += correction.Direction * correction.Error;
				}

				// The accumulated error is almost zero which means we hit a local minimum.
				if (error.IsAlmostZero(maxError) == true)
					break;

				// Normalize the error => now we know what is the wrong direction => desired correction needs to move in opposite direction to lower the error.
				error.Normalize();

				for (int i = 0; i < _count; ++i)
				{
					Correction correction = _corrections[i];

					// Compare single correction direction with the accumulated error direction.
					errorDot = correction.Direction.x * error.x + correction.Direction.y * error.y + correction.Direction.z * error.z;

					// Accumulate error correction based on relative correction errors.
					// Corrections with direction aligned to accumulated error have more impact.
					errorCorrection += correction.Error * errorDot;

					if (errorDot >= 0.0f)
					{
						errorCorrectionSize += errorDot;
					}
					else
					{
						errorCorrectionSize -= errorDot;
					}
				}

				if (errorCorrectionSize < 0.000001f)
					break;

				// The error correction is almost zero and desired correction won't change.
				errorCorrection /= errorCorrectionSize;
				if (errorCorrection.IsAlmostZero(maxError) == true)
					break;

				// Move desired correction in opposite way of the accumulated error.
				targetCorrection -= error * errorCorrection;
			}

			return targetCorrection;
		}

		// PRIVATE METHODS

		private static Vector3 CalculateMinMax(params Vector3[] corrections)
		{
			Vector3 minCorrection = default;
			Vector3 maxCorrection = default;

			for (int i = 0; i < corrections.Length; ++i)
			{
				Vector3 correction = corrections[i];
				minCorrection = Vector3.Min(minCorrection, correction);
				maxCorrection = Vector3.Max(maxCorrection, correction);
			}

			return minCorrection + maxCorrection;
		}

		private static Vector3 CalculateBinary(Correction correction0, Correction correction1)
		{
			Vector3D correction0Direction = new Vector3D(correction0.Direction);
			Vector3D correction1Direction = new Vector3D(correction1.Direction);

			double correctionDot = Dot(correction0Direction, correction1Direction);
			if (correctionDot > 0.999999 || correctionDot < -0.999999)
			{
				Vector3 minCorrection = Vector3.Min(Vector3.Min(default, correction0.Amount), correction1.Amount);
				Vector3 maxCorrection = Vector3.Max(Vector3.Max(default, correction0.Amount), correction1.Amount);
				return minCorrection + maxCorrection;
			}

			Vector3D deltaCorrectionDirection = Normalize(Cross(Cross(correction0Direction, correction1Direction), correction0Direction));
			double   deltaCorrectionDistance  = ((double)correction1.Distance - (double)correction0.Distance * correctionDot) / Math.Sqrt(1.0 - correctionDot * correctionDot);

			Vector3 targetCorrection = correction0.Amount;

			targetCorrection.x += (float)(deltaCorrectionDirection.x * deltaCorrectionDistance);
			targetCorrection.y += (float)(deltaCorrectionDirection.y * deltaCorrectionDistance);
			targetCorrection.z += (float)(deltaCorrectionDirection.z * deltaCorrectionDistance);

			return targetCorrection;
		}

		private static double Dot(Vector3D lhs, Vector3D rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}

		private static Vector3D Cross(Vector3D lhs, Vector3D rhs)
		{
			return new Vector3D(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
		}

		private static Vector3D Normalize(Vector3D value)
		{
			double magnitude = Magnitude(value);
			return magnitude > 0.000000000001 ? new Vector3D(value.x / magnitude, value.y / magnitude, value.z / magnitude) : new Vector3D(0.0, 0.0, 0.0);
		}

		private static double Magnitude(Vector3D vector)
		{
			return Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
		}

		// DATA STRUCTURES

		private sealed class Correction
		{
			public Vector3 Amount;
			public Vector3 Direction;
			public float   Distance;
			public float   Error;
		}

		private struct Vector3D
		{
			public double x;
			public double y;
			public double z;

			public Vector3D(double x, double y, double z)
			{
				this.x = x;
				this.y = y;
				this.z = z;
			}

			public Vector3D(Vector3 vector)
			{
				this.x = vector.x;
				this.y = vector.y;
				this.z = vector.z;
			}
		}
	}
}
