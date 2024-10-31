namespace Fusion.Addons.KCC
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using Fusion;

	/// <summary>
	/// Used to update PlatformProcessor instances independently of their Object.IsInSimulation state.
	/// </summary>
	[DefaultExecutionOrder(PlatformProcessor.EXECUTION_ORDER)]
    public unsafe class PlatformProcessorUpdater : SimulationBehaviour
    {
		private HashSet<PlatformProcessor> _processors = new HashSet<PlatformProcessor>();

		public void Register(PlatformProcessor processor)
		{
			_processors.Add(processor);
		}

		public void Unregister(PlatformProcessor processor)
		{
			_processors.Remove(processor);
		}

		public override void FixedUpdateNetwork()
		{
			foreach (PlatformProcessor processor in _processors)
			{
				try
				{
					processor.ProcessFixedUpdate();
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
		}

		public override void Render()
		{
			foreach (PlatformProcessor processor in _processors)
			{
				try
				{
					processor.ProcessRender();
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
		}
	}
}
