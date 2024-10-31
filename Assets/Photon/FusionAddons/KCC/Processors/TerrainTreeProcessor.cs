namespace Fusion.Addons.KCC
{
	using UnityEngine;

	/// <summary>
	/// This processor runs extra capsule-cast after move step and resolves collisions with terrain tree colliders.
	/// Why? The KCC core movement algorithm is based on capsule overlap + Physics.ComputePenetration() and this approach doesn't work with terrain tree colliders.
	/// </summary>
	public class TerrainTreeProcessor : KCCProcessor, IAfterMoveStep
	{
		// CONSTANTS

		public static readonly int DefaultPriority = -500;

		// PRIVATE MEMBERS

		[SerializeField][Tooltip("If true, the initial KCC capsule overlap must contain a TerrainCollider.")]
		public bool _requireTerrainCollider = true;
		[SerializeField][Tooltip("Minimum distance the KCC must move in single step to check tree colliders.")]
		public float _minMoveDistance = 0.001f;
		[SerializeField][Tooltip("Size of the capsule used for shape-cast. A value of 0.9f means 90% of the KCC capsule size.")]
		[Range(0.0f, 1.0f)]
		private float _capsuleScale = 0.9f;

		private KCCShapeCastInfo _shapeCastInfo = new KCCShapeCastInfo();

		// KCCProcessor INTERFACE

		public override float GetPriority(KCC kcc) => DefaultPriority;

		// IAfterMoveStep INTERFACE

		public void Execute(AfterMoveStep stage, KCC kcc, KCCData data)
		{
			// At this point, both KCCData.BasePosition and KCCData.TargetPosition should be in a depenetrated state => no or very tiny overlap with a terrain.

			float   capsuleOffset   = kcc.Settings.Radius * (1.0f - _capsuleScale);
			Vector3 moveDirection   = data.TargetPosition - data.BasePosition;
			float   moveDistance    = moveDirection.magnitude;
			Vector3 collisionNormal = default;

			// Check for minimum move distance. Performance optimization.
			if (moveDistance < _minMoveDistance)
				return;

			// Check if the KCC has a TerrainCollider in the initial overlap (which is ~2x radius) - it doesn't have to be within KCC capsule extent. Performance optimization.
			if (_requireTerrainCollider == true && stage.OverlapInfo.HasCollider(EColliderType.Terrain) == false)
				return;

			moveDirection.Normalize();

			// We don't want to check initial overlaps, therefore the capsule size must be a bit smaller.
			if (kcc.CapsuleCast(_shapeCastInfo, data.BasePosition + Vector3.up * capsuleOffset, kcc.Settings.Radius - capsuleOffset, kcc.Settings.Height - capsuleOffset * 2.0f, moveDirection, moveDistance + capsuleOffset, QueryTriggerInteraction.Ignore, false) == true)
			{
				for (int i = 0, count = _shapeCastInfo.ColliderHitCount; i < count; ++i)
				{
					KCCShapeCastHit shapeCastHit = _shapeCastInfo.ColliderHits[i];
					if (shapeCastHit.Type == EColliderType.Terrain)
					{
						// Unfortunately Unity doesn't provide enough information about tree colliders.
						// Since BasePosition and TargetPosition are in depenetrated state, we can assume all TerrainCollider hits are tree colliders.

						// Shrink max KCC move distance based on shape-cast hit distance.
						moveDistance = Mathf.Min(moveDistance, shapeCastHit.RaycastHit.distance);
						collisionNormal += shapeCastHit.RaycastHit.normal;
					}

					/*
					kcc.DrawCapsule(data.BasePosition, kcc.Settings.Radius, kcc.Settings.Height, Color.blue, 10.0f);
					kcc.DrawCapsule(data.BasePosition + Vector3.up * capsuleOffset, kcc.Settings.Radius - capsuleOffset, kcc.Settings.Height - capsuleOffset * 2.0f, Color.red, 10.0f);
					kcc.DrawCapsule(data.BasePosition + Vector3.up * capsuleOffset + moveDirection * (moveDistance + capsuleOffset), kcc.Settings.Radius - capsuleOffset, kcc.Settings.Height - capsuleOffset * 2.0f, Color.green, 10.0f);
					kcc.DrawSphere(shapeCastHit.RaycastHit.point, kcc.Settings.Radius * 0.1f, Color.yellow, 10.0f);
					*/
				}
			}

			if (collisionNormal != default)
			{
				collisionNormal.Normalize();

				// The move distance was shortened, we need to recalculate target position based on it.
				// We also push KCC along collision normal which results in a natural sliding effect.
				data.TargetPosition = data.BasePosition + moveDirection * moveDistance + collisionNormal * capsuleOffset;
			}
		}
	}
}
