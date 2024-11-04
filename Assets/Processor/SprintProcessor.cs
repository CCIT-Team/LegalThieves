using UnityEngine;
using Fusion.Addons.KCC;

/// <summary>
/// ���� ���μ��� - Sprint �Ӽ��� ���� � �ӵ��� �谡�ϴ� Ŭ�����Դϴ�.
/// </summary>
public sealed class SprintProcessor : KCCProcessor, ISetKinematicSpeed
{
    // PRIVATE MEMBERS

    // � �ӵ� ������ �����ϴ� �ʵ��Դϴ�.
    [SerializeField]
    private float _kinematicSpeedMultiplier = 2.0f;

    // KCCProcessor �������̽�

    /// <summary>
    /// ���μ����� �켱������ ��ȯ�մϴ�.
    /// </summary>
    public override float GetPriority(KCC kcc) => _kinematicSpeedMultiplier;

    // ISetKinematicSpeed �������̽�

    /// <summary>
    /// � �ӵ� ���� �޼����Դϴ�. Sprint �Ӽ��� Ȱ��ȭ�� ��� �ӵ��� ������ŵ�ϴ�.
    /// </summary>
    public void Execute(ISetKinematicSpeed stage, KCC kcc, KCCData data)
    {
        // Sprint �Ӽ��� Ȱ��ȭ�� ��쿡�� ������ �����մϴ�.
        if (data.Sprint == true)
        {
            // ������ ������ŭ � �ӵ��� ������ŵ�ϴ�.
            data.KinematicSpeed *= _kinematicSpeedMultiplier;

            // �켱������ ���� �ٸ� SprintProcessor�� ��Ȱ��ȭ�մϴ�.
            kcc.SuppressProcessors<SprintProcessor>();

            // IAbilityProcessor �������̽��� �����ϴ� �켱������ ���� ���μ����� ��Ȱ��ȭ�մϴ� (ī�װ����� ��Ȱ��ȭ�� �� ���).
            //kcc.SuppressProcessors<IAbilityProcessor>();

            // ISetKinematicSpeed �������̽��� �����ϴ� �켱������ ���� ���μ����� ��Ȱ��ȭ�մϴ�.
            //kcc.SuppressProcessors<ISetKinematicSpeed>();
        }
    }
}
