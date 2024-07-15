namespace Fusion.Addons.KCC
{
	using UnityEngine;

	public sealed class Vector3Accumulator
	{
		// PUBLIC MEMBERS

		public float SmoothingWindow;
		public bool  UseDirectionFilter;

		public Vector3 AccumulatedValue => _accumulatedValue;

		// PRIVATE MEMBERS

		private SmoothVector3 _smoothValues = new SmoothVector3(256);
		private Vector3       _accumulatedValue;
		private Vector3       _unprocessedValue;
		private float         _unprocessedDeltaTime;
		private int           _lastAccumulateFrame;
		private int           _lastConsumeFrame;

		// CONSTRUCTORS

		public Vector3Accumulator()                      : this(default,         default) {}
		public Vector3Accumulator(float smoothingWindow) : this(smoothingWindow, default) {}

		public Vector3Accumulator(float smoothingWindow, bool useDirectionFilter)
		{
			SmoothingWindow    = smoothingWindow;
			UseDirectionFilter = useDirectionFilter;
		}

		// PUBLIC METHODS

		public void Accumulate(Vector3 value)
		{
			int currentFrame = Time.frameCount;
			if (currentFrame == _lastAccumulateFrame)
				return;

			float unscaledDeltaTime = Time.unscaledDeltaTime;

			// Calculate average value if the smoothing window is valid.
			if (SmoothingWindow > 0.0f)
			{
				// Clear all values in opposite direction for instant flip.
				if (UseDirectionFilter == true)
				{
					_smoothValues.FilterValues(value.x < 0.0f, value.x > 0.0f, value.y < 0.0f, value.y > 0.0f, value.z < 0.0f, value.z > 0.0f);
				}

				// Add or update value for current frame.
				_smoothValues.AddValue(currentFrame, unscaledDeltaTime, value);

				// Calculate smooth value.
				value = _smoothValues.CalculateSmoothValue(SmoothingWindow, unscaledDeltaTime);
			}

			_accumulatedValue += value;

			_unprocessedValue     = value;
			_unprocessedDeltaTime = unscaledDeltaTime;
			_lastAccumulateFrame  = currentFrame;
		}

		public Vector3 Consume()
		{
			Vector3 consumeValue = _accumulatedValue;

			_accumulatedValue     = default;
			_unprocessedValue     = default;
			_unprocessedDeltaTime = default;
			_lastConsumeFrame     = Time.frameCount;

			return consumeValue;
		}

		public Vector3 ConsumeTickAligned(NetworkRunner runner)
		{
			int currentFrame = Time.frameCount;

			// Revert accumulated value to state before latest accumulation.
			Vector3 consumeValue = _accumulatedValue - _unprocessedValue;

			// In the first call (within single Unity frame) we want to accumulate only "missing" part since last Render to align timing with fixed tick (last Runner.LocalAlpha => 1.0).
			// All subsequent calls return remaining value which is not yet consumed, but again within alignment limits of fixed ticks (0.0 => 1.0 = current => next).
			float baseAlpha = _lastConsumeFrame != currentFrame ? runner.LocalAlpha : 0.0f;

			// Here we calculate delta time between last Render time (or last tick aligned time) and time of the pending simulation tick.
			float tickAlignedDeltaTime = (1.0f - baseAlpha) * runner.DeltaTime;

			// The unprocessed value is usually not aligned with ticks, we need to remove delta which is ahead of fixed tick time.
			Vector3 tickAlignedValue = _unprocessedValue * Mathf.Clamp01(tickAlignedDeltaTime / _unprocessedDeltaTime);

			// Accumulate consume value up to next aligned tick time.
			consumeValue += tickAlignedValue;

			// Decrease remaining unprocessed value by the partial value consumed by accumulation.
			_unprocessedValue -= tickAlignedValue;

			// Decrease remaining unprocessed delta time by the partial delta time consumed by accumulation.
			_unprocessedDeltaTime -= tickAlignedDeltaTime;

			// Removed the calculated consume value from total accumulated value. This is a remaining value that will be polled with for next tick.
			_accumulatedValue -= consumeValue;

			_lastConsumeFrame = currentFrame;

			return consumeValue;
		}

		public void Clear()
		{
			_smoothValues.ClearValues();

			_accumulatedValue     = default;
			_unprocessedValue     = default;
			_unprocessedDeltaTime = default;
		}
	}
}
