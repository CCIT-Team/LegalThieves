namespace Fusion.Addons.KCC
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Data structure representing single collider/trigger overlap (radius + extent). Read-only, managed entirely by <c>KCC</c>.
	/// </summary>
	public sealed partial class KCCHit
	{
		// PUBLIC MEMBERS

		/// <summary>Reference to collider/trigger component.</summary>
		public Collider Collider;

		/// <summary>Reference to collider transform component.</summary>
		public Transform Transform;

		/// <summary>
		/// Collision type, valid only for penetrating collisions.
		/// Non-penetrating collisions within (radius + extent) have ECollisionType.None.
		/// </summary>
		public ECollisionType CollisionType;
	}

	/// <summary>
	/// Collection dedicated to tracking all colliders/triggers the KCC collides with (radius + extent). Managed entirely by <c>KCC</c>.
	/// </summary>
	public sealed partial class KCCHits
	{
		// PUBLIC MEMBERS

		public readonly List<KCCHit> All = new List<KCCHit>();

		public int Count => All.Count;

		// PRIVATE MEMBERS

		private static readonly KCCFastStack<KCCHit> _pool = new KCCFastStack<KCCHit>(256, true);

		// PUBLIC METHODS

		public bool HasCollider(Collider collider)
		{
			return Find(collider, out int index) != null;
		}

		public KCCHit Add(KCCOverlapHit overlapHit)
		{
			KCCHit hit = _pool.PopOrCreate();
			hit.Collider      = overlapHit.Collider;
			hit.Transform     = overlapHit.Transform;
			hit.CollisionType = overlapHit.CollisionType;

			All.Add(hit);

			return hit;
		}

		public void CopyFromOther(KCCHits other)
		{
			KCCHit thisHit;
			KCCHit otherHit;
			int    thisCount  = All.Count;
			int    otherCount = other.All.Count;

			if (thisCount == otherCount)
			{
				if (thisCount == 0)
					return;

				for (int i = 0; i < thisCount; ++i)
				{
					thisHit  = All[i];
					otherHit = other.All[i];

					thisHit.Collider      = otherHit.Collider;
					thisHit.Transform     = otherHit.Transform;
					thisHit.CollisionType = otherHit.CollisionType;
				}
			}
			else
			{
				Clear();

				for (int i = 0; i < otherCount; ++i)
				{
					thisHit  = _pool.PopOrCreate();
					otherHit = other.All[i];

					thisHit.Collider      = otherHit.Collider;
					thisHit.Transform     = otherHit.Transform;
					thisHit.CollisionType = otherHit.CollisionType;

					All.Add(thisHit);
				}
			}
		}

		public void Clear()
		{
			for (int i = 0, count = All.Count; i < count; ++i)
			{
				KCCHit hit = All[i];
				hit.Collider      = default;
				hit.Transform     = default;
				hit.CollisionType = default;
				_pool.Push(hit);
			}

			All.Clear();
		}

		// PRIVATE METHODS

		private KCCHit Find(Collider collider, out int index)
		{
			for (int i = 0, count = All.Count; i < count; ++i)
			{
				KCCHit hit = All[i];
				if (object.ReferenceEquals(hit.Collider, collider) == true)
				{
					index = i;
					return hit;
				}
			}

			index = -1;
			return default;
		}
	}
}
