namespace Fusion.Addons.KCC
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Data structure representing single collider/trigger ignore entry. Read-only, managed entirely by <c>KCC</c>.
	/// </summary>
	public sealed class KCCIgnore
	{
		public KCCNetworkID  NetworkID;
		public NetworkObject NetworkObject;
		public Collider      Collider;

		public void CopyFromOther(KCCIgnore other)
		{
			NetworkID     = other.NetworkID;
			NetworkObject = other.NetworkObject;
			Collider      = other.Collider;
		}

		public void Clear()
		{
			NetworkID     = default;
			NetworkObject = default;
			Collider      = default;
		}
	}

	/// <summary>
	/// Collection dedicated to tracking ignored colliders. Managed entirely by <c>KCC</c> component.
	/// </summary>
	public sealed class KCCIgnores
	{
		// PUBLIC MEMBERS

		public readonly List<KCCIgnore> All = new List<KCCIgnore>();

		public int Count => All.Count;

		// PRIVATE MEMBERS

		private static readonly KCCFastStack<KCCIgnore> _pool = new KCCFastStack<KCCIgnore>(128, true);

		// PUBLIC METHODS

		public bool HasCollider(Collider collider)
		{
			return Find(collider, out int index) != null;
		}

		public KCCIgnore Add(NetworkObject networkObject, Collider collider, bool checkExisting)
		{
			KCCIgnore ignore = checkExisting == true ? Find(collider, out int index) : null;
			if (ignore == null)
			{
				ignore = _pool.PopOrCreate();

				ignore.NetworkID     = KCCNetworkID.GetNetworkID(networkObject);
				ignore.NetworkObject = networkObject;
				ignore.Collider      = collider;

				All.Add(ignore);
			}

			return ignore;
		}

		public bool Add(NetworkObject networkObject, KCCNetworkID networkID)
		{
			if (networkObject == null)
				return false;

			KCCIgnore ignore = _pool.PopOrCreate();
			ignore.NetworkID     = networkID;
			ignore.NetworkObject = networkObject;
			ignore.Collider      = networkObject.GetComponentNoAlloc<Collider>();

			All.Add(ignore);

			return true;
		}

		public bool Remove(Collider collider)
		{
			KCCIgnore ignore = Find(collider, out int index);
			if (ignore != null)
			{
				All.RemoveAt(index);
				ignore.Clear();
				_pool.Push(ignore);
				return true;
			}

			return false;
		}

		public void CopyFromOther(KCCIgnores other)
		{
			int thisCount  = All.Count;
			int otherCount = other.All.Count;

			if (thisCount == otherCount)
			{
				if (thisCount == 0)
					return;

				for (int i = 0; i < thisCount; ++i)
				{
					All[i].CopyFromOther(other.All[i]);
				}
			}
			else
			{
				Clear();

				for (int i = 0; i < otherCount; ++i)
				{
					KCCIgnore ignore = _pool.PopOrCreate();
					ignore.CopyFromOther(other.All[i]);
					All.Add(ignore);
				}
			}
		}

		public void Clear()
		{
			for (int i = 0, count = All.Count; i < count; ++i)
			{
				KCCIgnore ignore = All[i];
				ignore.Clear();
				_pool.Push(ignore);
			}

			All.Clear();
		}

		// PRIVATE METHODS

		private KCCIgnore Find(Collider collider, out int index)
		{
			for (int i = 0, count = All.Count; i < count; ++i)
			{
				KCCIgnore ignore = All[i];
				if (object.ReferenceEquals(ignore.Collider, collider) == true)
				{
					index = i;
					return ignore;
				}
			}

			index = -1;
			return default;
		}
	}
}
