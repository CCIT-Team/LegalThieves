using UnityEngine;
using Fusion.Addons.KCC;

/// <summary>
/// Example processor - applying changes when crouching.
/// </summary>
public sealed class CrouchProcessor : KCCProcessor, ISetKinematicSpeed
{
    // PRIVATE MEMBERS
    [SerializeField]
    private float _crouchSpeedMultiplier = 0.5f; // �ޱ׸��� �� �ӵ� ���� ����
    [SerializeField]
    private float _crouchHeight = 0.5f; // �ޱ׸��� �� �ݶ��̴� ����
    private float _originalHeight; // ���� �ݶ��̴� ����

    // KCCProcessor INTERFACE
    public override float GetPriority(KCC kcc) => _crouchSpeedMultiplier;

    // �� �޼���� KCC�� �ޱ׸��� �������� Ȯ���ϰ�, �ش� �ӵ� �� ���� ����
    public void Execute(ISetKinematicSpeed stage, KCC kcc, KCCData data)
    {
        if (data.Crouch) // Crouch�� true�� ��
        {
            // ���� ���� ����
            if (_originalHeight == 0f)
            {
                _originalHeight = kcc.Collider.height; // �ݶ��̴��� ���� ���� ����
            }

            // �ޱ׸��� ������ �� �ӵ� ����
            data.KinematicSpeed *= _crouchSpeedMultiplier;
            kcc.Collider.height = _crouchHeight; // �ݶ��̴� ���� ����

            // �ٸ� CrouchProcessor�� ������� �ʵ��� suppress
            kcc.SuppressProcessors<CrouchProcessor>();
        }
        else
        {
            // �ޱ׸��� ���°� �ƴ� �� ���� ���̷� ����
            if (_originalHeight > 0f)
            {
                kcc.Collider.height = _originalHeight; // ���� ���̷� ����
                _originalHeight = 0f; // ���� ���� ����
            }
        }
    }
}

