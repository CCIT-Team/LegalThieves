namespace Fusion.Addons.KCC
{
	using System;

	public enum EKCCTrace
	{
		None      = 0,
		Stage     = 1,
		Processor = 2,
	}

	/// <summary>
	/// Helper class for tracing stage and processor execution.
	/// </summary>
	public sealed class KCCTraceInfo
	{
		// PUBLIC MEMBERS

		public EKCCTrace     Trace     = default;
		public Type          Type      = default;
		public string        Name      = default;
		public int           Level     = default;
		public IKCCProcessor Processor = default;
		public bool          IsVisible = default;

		public bool IsValid     => Trace != EKCCTrace.None;
		public bool IsStage     => Trace == EKCCTrace.Stage;
		public bool IsProcessor => Trace == EKCCTrace.Processor;

		// PUBLIC METHODS

		public void Set(EKCCTrace trace, Type type, string name, int level, IKCCProcessor processor)
		{
			if (Trace == trace && Name == name && Type == type && Level == level && object.ReferenceEquals(Processor, processor) == true)
				return;

			Trace     = trace;
			Type      = type;
			Name      = name;
			Level     = level;
			Processor = processor;
			IsVisible = level == default;
		}
	}
}
