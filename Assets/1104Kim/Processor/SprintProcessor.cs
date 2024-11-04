using UnityEngine;
using Fusion.Addons.KCC;

/// <summary>
/// 예제 프로세서 - Sprint 속성에 따라 운동 속도를 배가하는 클래스입니다.
/// </summary>
public sealed class SprintProcessor : KCCProcessor, ISetKinematicSpeed
{
    // PRIVATE MEMBERS

    // 운동 속도 배율을 설정하는 필드입니다.
    [SerializeField]
    private float _kinematicSpeedMultiplier = 2.0f;

    // KCCProcessor 인터페이스

    /// <summary>
    /// 프로세서의 우선순위를 반환합니다.
    /// </summary>
    public override float GetPriority(KCC kcc) => _kinematicSpeedMultiplier;

    // ISetKinematicSpeed 인터페이스

    /// <summary>
    /// 운동 속도 설정 메서드입니다. Sprint 속성이 활성화된 경우 속도를 증가시킵니다.
    /// </summary>
    public void Execute(ISetKinematicSpeed stage, KCC kcc, KCCData data)
    {
        // Sprint 속성이 활성화된 경우에만 배율을 적용합니다.
        if (data.Sprint == true)
        {
            // 설정된 배율만큼 운동 속도를 증가시킵니다.
            data.KinematicSpeed *= _kinematicSpeedMultiplier;

            // 우선순위가 낮은 다른 SprintProcessor를 비활성화합니다.
            kcc.SuppressProcessors<SprintProcessor>();

            // IAbilityProcessor 인터페이스를 구현하는 우선순위가 낮은 프로세서를 비활성화합니다 (카테고리별로 비활성화할 때 사용).
            //kcc.SuppressProcessors<IAbilityProcessor>();

            // ISetKinematicSpeed 인터페이스를 구현하는 우선순위가 낮은 프로세서를 비활성화합니다.
            //kcc.SuppressProcessors<ISetKinematicSpeed>();
        }
    }
}
