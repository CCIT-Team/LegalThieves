namespace Fusion.Addons.KCC
{
	using UnityEngine;

	public sealed class SmoothVector2 : SmoothValue<Vector2>
	{
		// CONSTRUCTORS

		public SmoothVector2(int records) : base(records)
		{
		}

		// PUBLIC METHODS

		public void FilterValues(bool positiveX, bool negativeX, bool positiveY, bool negativeY)
		{
			SmoothItem<Vector2>[] items = Items;
			SmoothItem<Vector2>   item;

			if (positiveX == true) { for (int i = 0, count = items.Length; i < count; ++i) { item = items[i]; if (item.Value.x > 0.0f) { item.Value.x = 0.0f; } } }
			if (negativeX == true) { for (int i = 0, count = items.Length; i < count; ++i) { item = items[i]; if (item.Value.x < 0.0f) { item.Value.x = 0.0f; } } }
			if (positiveY == true) { for (int i = 0, count = items.Length; i < count; ++i) { item = items[i]; if (item.Value.y > 0.0f) { item.Value.y = 0.0f; } } }
			if (negativeY == true) { for (int i = 0, count = items.Length; i < count; ++i) { item = items[i]; if (item.Value.y < 0.0f) { item.Value.y = 0.0f; } } }
		}

		// SmoothValue INTERFACE

		protected override Vector2 GetDefaultValue()
		{
			return Vector2.zero;
		}

		protected override Vector2 AccumulateValue(Vector2 accumulatedValue, Vector2 value, double scale)
		{
			accumulatedValue.x = (float)(accumulatedValue.x + value.x * scale);
			accumulatedValue.y = (float)(accumulatedValue.y + value.y * scale);
			return accumulatedValue;
		}

		protected override Vector2 GetSmoothValue(Vector2 accumulatedValue, double scale)
		{
			accumulatedValue.x = (float)(accumulatedValue.x * scale);
			accumulatedValue.y = (float)(accumulatedValue.y * scale);
			return accumulatedValue;
		}
	}
}
