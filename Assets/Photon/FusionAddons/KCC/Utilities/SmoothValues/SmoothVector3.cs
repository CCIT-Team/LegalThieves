namespace Fusion.Addons.KCC
{
	using UnityEngine;

	public sealed class SmoothVector3 : SmoothValue<Vector3>
	{
		// CONSTRUCTORS

		public SmoothVector3(int records) : base(records)
		{
		}

		// PUBLIC METHODS

		public void FilterValues(bool positiveX, bool negativeX, bool positiveY, bool negativeY, bool positiveZ, bool negativeZ)
		{
			SmoothItem<Vector3>[] items = Items;
			SmoothItem<Vector3>   item;

			if (positiveX == true) { for (int i = 0, count = items.Length; i < count; ++i) { item = items[i]; if (item.Value.x > 0.0f) { item.Value.x = 0.0f; } } }
			if (negativeX == true) { for (int i = 0, count = items.Length; i < count; ++i) { item = items[i]; if (item.Value.x < 0.0f) { item.Value.x = 0.0f; } } }
			if (positiveY == true) { for (int i = 0, count = items.Length; i < count; ++i) { item = items[i]; if (item.Value.y > 0.0f) { item.Value.y = 0.0f; } } }
			if (negativeY == true) { for (int i = 0, count = items.Length; i < count; ++i) { item = items[i]; if (item.Value.y < 0.0f) { item.Value.y = 0.0f; } } }
			if (positiveZ == true) { for (int i = 0, count = items.Length; i < count; ++i) { item = items[i]; if (item.Value.z > 0.0f) { item.Value.z = 0.0f; } } }
			if (negativeZ == true) { for (int i = 0, count = items.Length; i < count; ++i) { item = items[i]; if (item.Value.z < 0.0f) { item.Value.z = 0.0f; } } }
		}

		// SmoothValue INTERFACE

		protected override Vector3 GetDefaultValue()
		{
			return Vector3.zero;
		}

		protected override Vector3 AccumulateValue(Vector3 accumulatedValue, Vector3 value, double scale)
		{
			accumulatedValue.x = (float)(accumulatedValue.x + value.x * scale);
			accumulatedValue.y = (float)(accumulatedValue.y + value.y * scale);
			accumulatedValue.z = (float)(accumulatedValue.z + value.z * scale);
			return accumulatedValue;
		}

		protected override Vector3 GetSmoothValue(Vector3 accumulatedValue, double scale)
		{
			accumulatedValue.x = (float)(accumulatedValue.x * scale);
			accumulatedValue.y = (float)(accumulatedValue.y * scale);
			accumulatedValue.z = (float)(accumulatedValue.z * scale);
			return accumulatedValue;
		}
	}
}
