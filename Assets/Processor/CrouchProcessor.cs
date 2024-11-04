using UnityEngine;
using Fusion.Addons.KCC;

/// <summary>
/// Example processor - applying changes when crouching.
/// </summary>
public sealed class CrouchProcessor : KCCProcessor, ISetKinematicSpeed
{
    // PRIVATE MEMBERS
    [SerializeField]
    private float _crouchSpeedMultiplier = 0.5f; // 쭈그리기 시 속도 감소 비율
    [SerializeField]
    private float _crouchHeight = 0.5f; // 쭈그리기 시 콜라이더 높이
    private float _originalHeight; // 원래 콜라이더 높이

    // KCCProcessor INTERFACE
    public override float GetPriority(KCC kcc) => _crouchSpeedMultiplier;

    // 이 메서드는 KCC가 쭈그리기 상태인지 확인하고, 해당 속도 및 높이 적용
    public void Execute(ISetKinematicSpeed stage, KCC kcc, KCCData data)
    {
        if (data.Crouch) // Crouch가 true일 때
        {
            // 원래 높이 저장
            if (_originalHeight == 0f)
            {
                _originalHeight = kcc.Collider.height; // 콜라이더의 현재 높이 저장
            }

            // 쭈그리기 상태일 때 속도 조정
            data.KinematicSpeed *= _crouchSpeedMultiplier;
            kcc.Collider.height = _crouchHeight; // 콜라이더 높이 조정

            // 다른 CrouchProcessor가 적용되지 않도록 suppress
            kcc.SuppressProcessors<CrouchProcessor>();
        }
        else
        {
            // 쭈그리기 상태가 아닐 때 원래 높이로 복구
            if (_originalHeight > 0f)
            {
                kcc.Collider.height = _originalHeight; // 원래 높이로 복구
                _originalHeight = 0f; // 원래 높이 리셋
            }
        }
    }
}

