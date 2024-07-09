using Fusion.Addons.KCC;
using UnityEngine;

namespace LegalThieves
{
    public class ImpulseProcessor : KCCProcessor
    {
        public float ImpulseStrength;
        [SerializeField] private bool isSphere;

        public override void OnEnter(KCC kcc, KCCData data)
        {
            var thisPosition = transform;
            Vector3 impulseDirection;
            if (isSphere)
            {
                impulseDirection =
                    Vector3.Normalize(kcc.Collider.ClosestPoint(thisPosition.position) - thisPosition.position);
            }
            else
            {
                impulseDirection = thisPosition.forward;
            }

            kcc.SetDynamicVelocity(data.DynamicVelocity -
                                   Vector3.Scale(data.DynamicVelocity, impulseDirection.normalized));

            kcc.AddExternalImpulse(impulseDirection * ImpulseStrength);
        }
    }
}