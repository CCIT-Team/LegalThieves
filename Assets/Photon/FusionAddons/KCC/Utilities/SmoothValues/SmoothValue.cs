namespace Fusion.Addons.KCC
{
	using System;

	public sealed class SmoothItem<T>
	{
		public int    Frame;
		public double Size;
		public T      Value;
	}

	public abstract class SmoothValue<T>
	{
		// PUBLIC MEMBERS

		public SmoothItem<T>[] Items => _items;

		// PRIVATE MEMBERS

		private SmoothItem<T>[] _items;
		private int             _index;

		// CONSTRUCTORS

		public SmoothValue(int capacity)
		{
			_items = new SmoothItem<T>[capacity];

			for (int i = 0; i < capacity; ++i)
			{
				_items[i] = new SmoothItem<T>();
			}
		}

		// PUBLIC METHODS

		public void AddValue(int frame, double size, T value)
		{
			if (size <= 0.0)
				throw new ArgumentException(nameof(size));

			SmoothItem<T> item = _items[_index];
			if (item.Frame == frame)
			{
				item.Size  = size;
				item.Value = value;
				return;
			}

			_index = (_index + 1) % _items.Length;

			item = _items[_index];
			item.Frame = frame;
			item.Size  = size;
			item.Value = value;
		}

		public void ClearValues()
		{
			SmoothItem<T> smoothItem;

			for (int i = 0, count = _items.Length; i < count; ++i)
			{
				smoothItem = _items[i];
				smoothItem.Frame = default;
				smoothItem.Size  = default;
				smoothItem.Value = GetDefaultValue();
			}

			_index = default;
		}

		public T CalculateSmoothValue(double window, double size)
		{
			return CalculateSmoothValue(window, size, out int frames);
		}

		public T CalculateSmoothValue(double window, double size, out int frames)
		{
			SmoothItem<T> item;

			frames = default;

			if (window <= 0.0f)
			{
				item = _items[_index];
				if (item.Frame == default)
					return default;

				frames = 1;
				return item.Value;
			}

			double remainingWindow  = window;
			T      accumulatedValue = GetDefaultValue();

			for (int i = _index; i >= 0; --i)
			{
				item = _items[i];
				if (item.Frame == default)
					continue;

				if (remainingWindow <= item.Size)
				{
					double scale = remainingWindow / item.Size;

					accumulatedValue = AccumulateValue(accumulatedValue, item.Value, scale);

					++frames;

					return GetSmoothValue(accumulatedValue, size / window);
				}

				remainingWindow -= item.Size;
				accumulatedValue = AccumulateValue(accumulatedValue, item.Value, 1.0);

				++frames;
			}

			for (int i = _items.Length - 1; i > _index; --i)
			{
				item = _items[i];
				if (item.Frame == default)
					continue;

				if (remainingWindow < item.Size)
				{
					double scale = remainingWindow / item.Size;

					accumulatedValue = AccumulateValue(accumulatedValue, item.Value, scale);

					++frames;

					return GetSmoothValue(accumulatedValue, size / window);
				}

				remainingWindow -= item.Size;
				accumulatedValue = AccumulateValue(accumulatedValue, item.Value, 1.0);

				++frames;
			}

			if (remainingWindow >= window)
				return default;

			double accumulatedSize = window - remainingWindow;

			return GetSmoothValue(accumulatedValue, size / accumulatedSize);
		}

		// SmoothValue INTERFACE

		protected abstract T GetDefaultValue();
		protected abstract T AccumulateValue(T accumulatedValue, T value, double scale);
		protected abstract T GetSmoothValue(T accumulatedValue, double scale);
	}
}
