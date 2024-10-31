namespace Fusion.Addons.KCC
{
	using UnityEngine;

	public sealed class PenetrationSolver : MonoBehaviour
	{
		public CapsuleCollider Collider;
		public LayerMask       CollisionLayerMask = 1;
		public int             PenetrationSteps = 8;

		private Collider[]  _hitColliders = new Collider[KCC.CACHE_SIZE];
		private KCCResolver _resolver     = new KCCResolver(KCC.CACHE_SIZE);

		private void OnDrawGizmosSelected()
		{
			if (Collider == null)
				return;

			Vector3 position = transform.position;
			float   radius   = Collider.radius;
			float   height   = Collider.height;

			position = Depenetrate(position, radius, height, radius);

			DrawCapsule(position, radius, height, Color.green);
		}

		private Vector3 Depenetrate(Vector3 position, float radius, float height, float extent)
		{
			Vector3 capsulePoint0 = position + new Vector3(0.0f, radius, 0.0f);
			Vector3 capsulePoint1 = position + new Vector3(0.0f, height - radius, 0.0f);

			int hitColliderCount = Physics.OverlapCapsuleNonAlloc(capsulePoint0, capsulePoint1, radius + extent, _hitColliders, CollisionLayerMask, QueryTriggerInteraction.Ignore);
			if (hitColliderCount <= 0)
				return position;

			int remainingSteps = PenetrationSteps;
			while (remainingSteps > 0)
			{
				--remainingSteps;

				_resolver.Reset();

				for (int i = 0; i < hitColliderCount; ++i)
				{
					Collider hitCollider = _hitColliders[i];
					if (hitCollider == Collider)
						continue;

					hitCollider.transform.GetPositionAndRotation(out Vector3 hitPosition, out Quaternion hitRotation);

					bool hasPenetration = Physics.ComputePenetration(Collider, position, Quaternion.identity, hitCollider, hitPosition, hitRotation, out Vector3 direction, out float distance);
					if (hasPenetration == true)
					{
						_resolver.AddCorrection(direction, distance);

						if (remainingSteps == PenetrationSteps - 1)
						{
							DrawLine(position, position + direction * distance, Color.red);
						}
					}
				}

				if (_resolver.Count <= 0)
					break;

				Vector3 correction = _resolver.CalculateBest(8, 0.0001f);
				correction = Vector3.ClampMagnitude(correction, radius);

				float correctionMultiplier = Mathf.Max(0.25f, 1.0f - remainingSteps * 0.25f);
				correction *= correctionMultiplier;

				DrawLine(position, position + correction, Color.yellow);
				position += correction;
				DrawLine(position, position + Vector3.up * 0.1f, Color.magenta);
			}

			return position;
		}

		private static void DrawLine(Vector3 fromPosition, Vector3 toPosition, Color color)
		{
			Color gizmosColor = Gizmos.color;
			Gizmos.color = color;
			Gizmos.DrawLine(fromPosition, toPosition);
			Gizmos.color = gizmosColor;
		}

		private static void DrawCapsule(Vector3 position, float radius, float height, Color color)
		{
			Color gizmosColor = Gizmos.color;

			Vector3 baseLow     = position + Vector3.up * radius;
			Vector3 baseHigh    = position + Vector3.up * (height - radius);
			Vector3 offsetFront = Vector3.forward * radius;
			Vector3 offsetBack  = Vector3.back    * radius;
			Vector3 offsetLeft  = Vector3.left    * radius;
			Vector3 offsetRight = Vector3.right   * radius;

			Gizmos.color = color;

			Gizmos.DrawWireSphere(baseLow, radius);
			Gizmos.DrawWireSphere(baseHigh, radius);

			Gizmos.DrawLine(baseLow + offsetFront, baseHigh + offsetFront);
			Gizmos.DrawLine(baseLow + offsetBack,  baseHigh + offsetBack);
			Gizmos.DrawLine(baseLow + offsetLeft,  baseHigh + offsetLeft);
			Gizmos.DrawLine(baseLow + offsetRight, baseHigh + offsetRight);

			Gizmos.color = gizmosColor;
		}
	}
}
