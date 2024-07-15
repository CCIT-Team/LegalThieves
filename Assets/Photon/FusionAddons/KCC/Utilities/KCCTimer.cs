namespace Fusion.Addons.KCC
{
	using System;
	using System.Diagnostics;
	using System.Runtime.CompilerServices;

	public sealed partial class KCCTimer
	{
		public enum EState
		{
			Stopped = 0,
			Running = 1,
			Paused  = 2,
		}

		// PUBLIC MEMBERS

		public readonly int    ID;
		public readonly string Name;

		public EState   State      { get { return _state;   } }
		public int      Counter    { get { return _counter; } }
		public TimeSpan TotalTime  { get { if (_state == EState.Running) { Update(); } return new TimeSpan(_totalTicks);  } }
		public TimeSpan RecentTime { get { if (_state == EState.Running) { Update(); } return new TimeSpan(_recentTicks); } }
		public TimeSpan PeakTime   { get { if (_state == EState.Running) { Update(); } return new TimeSpan(_peakTicks);   } }
		public TimeSpan LastTime   { get { if (_state == EState.Running) { Update(); } return new TimeSpan(_lastTicks);   } }

		// PRIVATE MEMBERS

		private EState _state;
		private int    _counter;
		private long   _baseTicks;
		private long   _totalTicks;
		private long   _recentTicks;
		private long   _peakTicks;
		private long   _lastTicks;

		// CONSTRUCTORS

		public KCCTimer()                        : this(-1, null, false) {}
		public KCCTimer(string name)             : this(-1, name, false) {}
		public KCCTimer(bool start)              : this(-1, null, start) {}
		public KCCTimer(string name, bool start) : this(-1, name, start) {}
		public KCCTimer(int id, string name)     : this(id, name, false) {}

		public KCCTimer(int id, string name, bool start)
		{
			ID   = id;
			Name = name;

			if (start == true)
			{
				Start();
			}
		}

		// PUBLIC METHODS

		public void Start()
		{
			if (_state == EState.Running)
				return;

			if (_state != EState.Paused)
			{
				if (_recentTicks != 0)
				{
					_lastTicks   = _recentTicks;
					_recentTicks = 0;
				}

				++_counter;
			}

			_baseTicks = Stopwatch.GetTimestamp();
			_state     = EState.Running;
		}

		public void Pause()
		{
			if (_state != EState.Running)
				return;

			Update();

			_state = EState.Paused;
		}

		public void Stop()
		{
			if (_state == EState.Running)
			{
				Update();
			}

			_state = EState.Stopped;
		}

		public void Restart()
		{
			if (_recentTicks != 0)
			{
				_lastTicks = _recentTicks;
			}

			_state       = EState.Running;
			_counter     = 1;
			_baseTicks   = Stopwatch.GetTimestamp();
			_recentTicks = 0;
			_totalTicks  = 0;
			_peakTicks   = 0;
		}

		public void Reset()
		{
			if (_recentTicks != 0)
			{
				_lastTicks = _recentTicks;
			}

			_state       = EState.Stopped;
			_counter     = 0;
			_baseTicks   = 0;
			_recentTicks = 0;
			_totalTicks  = 0;
			_peakTicks   = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetTotalSeconds()
		{
			return (float)TotalTime.TotalSeconds;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetTotalMilliseconds()
		{
			return (float)TotalTime.TotalMilliseconds;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetRecentSeconds()
		{
			return (float)RecentTime.TotalSeconds;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetRecentMilliseconds()
		{
			return (float)RecentTime.TotalMilliseconds;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetPeakSeconds()
		{
			return (float)PeakTime.TotalSeconds;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetPeakMilliseconds()
		{
			return (float)PeakTime.TotalMilliseconds;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetLastSeconds()
		{
			return (float)LastTime.TotalSeconds;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetLastMilliseconds()
		{
			return (float)LastTime.TotalMilliseconds;
		}

		public void LogSeconds(string prefix = null)
		{
			UnityEngine.Debug.Log($"{prefix}{TotalTime.TotalSeconds:F3}s");
		}

		public void LogMilliseconds(string prefix = null)
		{
			UnityEngine.Debug.Log($"{prefix}{TotalTime.TotalMilliseconds:F3}ms");
		}

		// PRIVATE METHODS

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Update()
		{
			long ticks = Stopwatch.GetTimestamp();

			_totalTicks  += ticks - _baseTicks;
			_recentTicks += ticks - _baseTicks;

			_baseTicks = ticks;

			if (_recentTicks > _peakTicks)
			{
				_peakTicks = _recentTicks;
			}

			if (_totalTicks < 0L)
			{
				_totalTicks = 0L;
			}
		}
	}
}
