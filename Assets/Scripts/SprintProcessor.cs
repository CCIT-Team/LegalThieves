using UnityEngine;
using Fusion.Addons.KCC;

/// <summary>
/// Example processor - multiplying kinematic speed based on Sprint property.
/// </summary>
public sealed class SprintProcessor : KCCProcessor, ISetKinematicSpeed
{
    // PRIVATE MEMBERS

    [SerializeField]
    private float _kinematicSpeedMultiplier = 2.0f;

    // KCCProcessor INTERFACE

    public override float GetPriority(KCC kcc) => _kinematicSpeedMultiplier;

    // ISetKinematicSpeed INTERFACE

    public void Execute(ISetKinematicSpeed stage, KCC kcc, KCCData data)
    {
        // Apply the multiplier only if the Sprint property is set.
        if (data.Sprint == true)
        {
            data.KinematicSpeed *= _kinematicSpeedMultiplier;

            // Suppress other sprint processors with lower priority.
            kcc.SuppressProcessors<SprintProcessor>();

            // Following call can be used to suppress other processors with lower priority implementing IAbilityProcessor (simulating a category identified by the interface).
            //kcc.SuppressProcessors<IAbilityProcessor>();

            // Following call can be used to suppress other ISetKinematicSpeed processors with lower priority.
            //kcc.SuppressProcessors<ISetKinematicSpeed>();
        }
    }
}
