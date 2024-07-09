namespace Fusion.Addons.KCC
{
	using System.Collections.Generic;

	/// <summary>
	/// Base container for all interactions.
	/// </summary>
	public abstract partial class KCCInteraction<TInteraction> where TInteraction : KCCInteraction<TInteraction>, new()
	{
		// PUBLIC MEMBERS

		public KCCNetworkID            NetworkID;
		public NetworkObject           NetworkObject;
		public IKCCInteractionProvider Provider;

		// KCCInteraction<TInteraction> INTERFACE

		public abstract void Initialize();
		public abstract void Deinitialize();
		public abstract void CopyFromOther(TInteraction other);
	}

	/// <summary>
	/// Base collection for tracking all interactions and their providers.
	/// </summary>
	public abstract partial class KCCInteractions<TInteraction> where TInteraction : KCCInteraction<TInteraction>, new ()
	{
		// PUBLIC MEMBERS

		public readonly List<TInteraction> All = new List<TInteraction>();

		public int Count => All.Count;

		// PRIVATE MEMBERS

		private static readonly KCCFastStack<TInteraction> _pool = new KCCFastStack<TInteraction>(256, true);

		// PUBLIC METHODS

		public bool HasProvider<T>() where T : class
		{
			for (int i = 0, count = All.Count; i < count; ++i)
			{
				if (All[i].Provider is T)
					return true;
			}

			return false;
		}

		public bool HasProvider(IKCCInteractionProvider provider)
		{
			for (int i = 0, count = All.Count; i < count; ++i)
			{
				if (object.ReferenceEquals(All[i].Provider, provider) == true)
					return true;
			}

			return false;
		}

		public T GetProvider<T>() where T : class
		{
			for (int i = 0, count = All.Count; i < count; ++i)
			{
				if (All[i].Provider is T provider)
					return provider;
			}

			return default;
		}

		public void GetProviders<T>(List<T> providers, bool clearList = true) where T : class
		{
			if (clearList == true)
			{
				providers.Clear();
			}

			for (int i = 0, count = All.Count; i < count; ++i)
			{
				if (All[i].Provider is T provider)
				{
					providers.Add(provider);
				}
			}
		}

		public TInteraction Find(IKCCInteractionProvider provider)
		{
			return Find(provider, out int index);
		}

		public TInteraction Add(NetworkObject networkObject, IKCCInteractionProvider provider)
		{
			return AddInternal(networkObject, provider, true);
		}

		public bool Add(NetworkObject networkObject, KCCNetworkID networkID)
		{
			if (networkObject == null)
				return false;

			IKCCInteractionProvider provider = networkObject.GetComponentNoAlloc<IKCCInteractionProvider>();

			TInteraction interaction = _pool.PopOrCreate();
			interaction.NetworkID     = networkID;
			interaction.NetworkObject = networkObject;
			interaction.Provider      = provider;
			interaction.Initialize();

			All.Add(interaction);

			return true;
		}

		public bool Remove(TInteraction interaction)
		{
			for (int i = 0, count = All.Count; i < count; ++i)
			{
				if (All[i] == interaction)
				{
					All.RemoveAt(i);
					ReturnToPool(interaction);
					return true;
				}
			}

			return false;
		}

		public void CopyFromOther<T>(T other) where T : KCCInteractions<TInteraction>
		{
			int thisCount  = All.Count;
			int otherCount = other.All.Count;

			if (thisCount == otherCount)
			{
				if (thisCount == 0)
					return;

				for (int i = 0; i < thisCount; ++i)
				{
					TInteraction interaction      = All[i];
					TInteraction otherInteraction = other.All[i];

					interaction.NetworkID     = otherInteraction.NetworkID;
					interaction.NetworkObject = otherInteraction.NetworkObject;
					interaction.Provider      = otherInteraction.Provider;
					interaction.CopyFromOther(otherInteraction);
				}
			}
			else
			{
				Clear();

				for (int i = 0; i < otherCount; ++i)
				{
					TInteraction otherInteraction = other.All[i];

					TInteraction interaction = _pool.PopOrCreate();
					interaction.NetworkID     = otherInteraction.NetworkID;
					interaction.NetworkObject = otherInteraction.NetworkObject;
					interaction.Provider      = otherInteraction.Provider;
					interaction.CopyFromOther(otherInteraction);

					All.Add(interaction);
				}
			}
		}

		public void Clear()
		{
			for (int i = 0, count = All.Count; i < count; ++i)
			{
				ReturnToPool(All[i]);
			}

			All.Clear();
		}

		// PROTECTED METHODS

		protected TInteraction AddInternal(NetworkObject networkObject, IKCCInteractionProvider provider, bool invokeInitialize)
		{
			TInteraction interaction = _pool.PopOrCreate();
			interaction.NetworkID     = KCCNetworkID.GetNetworkID(networkObject);
			interaction.NetworkObject = networkObject;
			interaction.Provider      = provider;

			if (invokeInitialize == true)
			{
				interaction.Initialize();
			}

			All.Add(interaction);

			return interaction;
		}

		protected void AddInternal(TInteraction interaction, NetworkObject networkObject, IKCCInteractionProvider provider, bool invokeInitialize)
		{
			interaction.NetworkID     = KCCNetworkID.GetNetworkID(networkObject);
			interaction.NetworkObject = networkObject;
			interaction.Provider      = provider;

			if (invokeInitialize == true)
			{
				interaction.Initialize();
			}

			All.Add(interaction);
		}

		protected TInteraction Find(IKCCInteractionProvider provider, out int index)
		{
			for (int i = 0, count = All.Count; i < count; ++i)
			{
				TInteraction interaction = All[i];
				if (object.ReferenceEquals(interaction.Provider, provider) == true)
				{
					index = i;
					return interaction;
				}
			}

			index = -1;
			return default;
		}

		protected static TInteraction GetFromPool()
		{
			return _pool.PopOrCreate();
		}

		// PRIVATE METHODS

		private static void ReturnToPool(TInteraction interaction)
		{
			interaction.Deinitialize();
			interaction.NetworkID     = default;
			interaction.NetworkObject = default;
			interaction.Provider      = default;
			_pool.Push(interaction);
		}
	}
}
