namespace Fusion.Addons.KCC
{
	using System;
	using System.Collections.Generic;

	using PostProcess = System.Action<KCC, KCCData>;

	/// <summary>
	/// Helper class for hierarchical stage execution. Only for internal purposes.
	/// </summary>
	public sealed class KCCStageInfo
	{
		// PUBLIC MEMBERS

		public Type              Type            = default;
		public int               Level           = default;
		public IKCCStage         StageObject     = default;
		public Type              StageObjectType = default;
		public IKCCProcessor[]   Processors      = new IKCCProcessor[KCC.CACHE_SIZE];
		public int               ProcessorCount  = default;
		public int               ProcessorIndex  = default;
		public List<PostProcess> PostProcesses   = new List<PostProcess>();

		// PRIVATE MEMBERS

		private static readonly KCCFastStack<KCCStageInfo> _pool = new KCCFastStack<KCCStageInfo>(KCC.CACHE_SIZE, true);

		// PUBLIC METHODS

		public bool HasPendingProcessor(IKCCProcessor processor)
		{
			for (int i = ProcessorIndex + 1, count = ProcessorCount; i < count; ++i)
			{
				if (ReferenceEquals(Processors[i], processor) == true)
					return true;
			}

			return false;
		}

		public bool HasPendingProcessor<T>() where T : class
		{
			for (int i = ProcessorIndex + 1, count = ProcessorCount; i < count; ++i)
			{
				if (Processors[i] is T)
					return true;
			}

			return false;
		}

		public bool GetPendingProcessor<T>(out T processor) where T : class
		{
			for (int i = ProcessorIndex + 1, count = ProcessorCount; i < count; ++i)
			{
				if (Processors[i] is T pendingProcessor)
				{
					processor = pendingProcessor;
					return true;
				}
			}

			processor = default;
			return false;
		}

		public void GetPendingProcessors<T>(List<T> processors, bool clearList) where T : class
		{
			if (clearList == true)
			{
				processors.Clear();
			}

			for (int i = ProcessorIndex + 1, count = ProcessorCount; i < count; ++i)
			{
				if (Processors[i] is T pendingProcessor)
				{
					processors.Add(pendingProcessor);
				}
			}
		}

		public bool HasExecutedProcessor(IKCCProcessor processor)
		{
			for (int i = ProcessorIndex - 1; i >= 0; --i)
			{
				if (ReferenceEquals(Processors[i], processor) == true)
					return true;
			}

			return false;
		}

		public bool HasExecutedProcessor<T>() where T : class
		{
			for (int i = ProcessorIndex - 1; i >= 0; --i)
			{
				if (Processors[i] is T)
					return true;
			}

			return false;
		}

		public bool GetExecutedProcessor<T>(out T processor) where T : class
		{
			for (int i = ProcessorIndex - 1; i >= 0; --i)
			{
				if (Processors[i] is T executedProcessor)
				{
					processor = executedProcessor;
					return true;
				}
			}

			processor = default;
			return false;
		}

		public void GetExecutedProcessors<T>(List<T> processors, bool clearList) where T : class
		{
			if (clearList == true)
			{
				processors.Clear();
			}

			for (int i = 0, count = ProcessorIndex; i < count; ++i)
			{
				if (Processors[i] is T executedProcessor)
				{
					processors.Add(executedProcessor);
				}
			}
		}

		public bool SuppressProcessor(IKCCProcessor processor)
		{
			for (int i = ProcessorIndex + 1, count = ProcessorCount; i < count; ++i)
			{
				if (ReferenceEquals(Processors[i], processor) == true)
				{
					Processors[i] = null;
					return true;
				}
			}

			return false;
		}

		public bool SuppressProcessors<T>() where T : class
		{
			bool result = false;

			for (int i = ProcessorIndex + 1, count = ProcessorCount; i < count; ++i)
			{
				if (Processors[i] is T)
				{
					Processors[i] = null;
					result = true;
				}
			}

			return result;
		}

		public void SuppressProcessorsExcept(IKCCProcessor processor)
		{
			for (int i = ProcessorIndex + 1, count = ProcessorCount; i < count; ++i)
			{
				if (ReferenceEquals(Processors[i], processor) == false)
				{
					Processors[i] = null;
				}
			}
		}

		public void SuppressProcessorsExcept<T>() where T : class
		{
			for (int i = ProcessorIndex + 1, count = ProcessorCount; i < count; ++i)
			{
				if (Processors[i] is not T)
				{
					Processors[i] = null;
				}
			}
		}

		public bool IsAssignable<T>() where T : IKCCStage
		{
			Type type = typeof(T);

			if (type == StageObjectType || type == Type)
				return true;
			if (type.IsAssignableFrom(StageObjectType) == true)
				return true;
			if (type.IsAssignableFrom(Type) == true)
				return true;

			return false;
		}

		public void Clear()
		{
			if (ProcessorCount > 0)
			{
				Array.Clear(Processors, 0, ProcessorCount);
			}

			PostProcesses.Clear();

			Type            = default;
			Level           = default;
			StageObject     = default;
			StageObjectType = default;
			ProcessorCount  = default;
			ProcessorIndex  = default;
		}

		public static KCCStageInfo Allocate()
		{
			return _pool.PopOrCreate();
		}

		public static void Release(KCCStageInfo stageInfo)
		{
			stageInfo.Clear();
			_pool.Push(stageInfo);
		}
	}
}
