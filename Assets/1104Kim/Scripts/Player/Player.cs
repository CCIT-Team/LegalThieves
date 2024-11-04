using UnityEngine;
using Fusion;
using Fusion.Addons.KCC;
using TMPro;

/// <summary>
/// Player Ŭ����: KCC(Ű ĳ���� ��Ʈ�ѷ�)�� Ȱ���� ��� �Է� ó�� �� �÷��̾� ����
/// </summary>
[DefaultExecutionOrder(-5)]
public sealed class Player : NetworkBehaviour
{
    // KCC �� ��Ÿ �ʿ��� ������Ʈ�� ������ ����
    public KCC KCC;
    public PlayerInput Input;
    public Transform CameraPivot;
    public Transform CameraHandle;
    public NetworkObject PlayerObject;
    public NetworkMecanimAnimator PlayerAnimator;
    public InteractionManager interactionManager;
    public Health health;
    public TextMeshPro Name;

    // �ִϸ��̼ǿ� ����� �Ķ����
    public float Speed;
    public bool Jump;
    public bool Interact;
    public bool IsCrouch;
    public bool Grounded;
    public bool FreeFall;
    public float MotionSpeed;

    public void Update()
    {
        // �÷��̾� �̸��� ü���� ǥ��
        Name.text = PlayerObject.Id.ToString() + health.CurrentHealth;
    }

    public override void FixedUpdateNetwork()
    {
        // �ִϸ��̼� �Ķ���� ������Ʈ
        UpdateAnimationParameters();

        // ī�޶� ȸ���� �Էµ� ������ ������Ʈ
        KCC.AddLookRotation(Input.CurrentInput.LookRotationDelta);

        // �÷��̾��� ���� ���������� �̵� ���� ����
        Vector3 inputDirection = KCC.Data.TransformRotation * new Vector3(Input.CurrentInput.MoveDirection.x, 0.0f, Input.CurrentInput.MoveDirection.y);
        KCC.SetInputDirection(inputDirection);

        // ���� ó�� - ���� ��ư�� ���Ȱ� �÷��̾ �ٴڿ� ���� ���� ����
        if (Input.CurrentInput.Actions.WasPressed(Input.PreviousInput.Actions, InputData.JUMP_BUTTON) && KCC.Data.IsGrounded)
        {
            KCC.Jump(Vector3.up * 6.0f); // ���� �� ����
            Jump = true; // �ִϸ��̼� �Ķ���͸� ���� ���� ���� ����
        }

        // ������Ʈ ó�� - ������Ʈ ��ư�� ������ ĳ���� �ӵ� ����
        if (KCC.Data.IsGrounded)
        {
            KCC.SetSprint(Input.CurrentInput.Actions.IsSet(InputData.SPRINT_BUTTON));
            KCC.SetCrouch(Input.CurrentInput.Actions.IsSet(InputData.CROUCH_BUTTON));
        }

        // ü�� ���� - ġ�� �� ������ ��ư�� ������ �� ó��
        if (Input.CurrentInput.Actions.WasPressed(Input.PreviousInput.Actions, InputData.HEAL_BUTTON))
        {
            health.AddHealth(10f); // ü�� �߰�
        }
        if (Input.CurrentInput.Actions.WasPressed(Input.PreviousInput.Actions, InputData.DAMAGE_BUTTON))
        {
            health.ApplyDamage(Object.InputAuthority, 10f); // ������ ����
        }

        // ��ȣ�ۿ� ó�� - ��ȣ�ۿ� ������ ������Ʈ�� �ְ�, ��ȣ�ۿ� ��ư�� ������ �� ó��
        if (interactionManager.curInteractable != null && Input.CurrentInput.Actions.WasPressed(Input.PreviousInput.Actions, InputData.INTERACT_BUTTON))
        {
            interactionManager.curInteractable.OnInteract(Runner); // ��ȣ�ۿ� ����
            interactionManager.curInteractGameobject = null;
            interactionManager.curInteractable = null;
            Interact = true; // �ִϸ��̼� �Ķ���� ����
        }
    }

    private void LateUpdate()
    {
        // ī�޶� ������ �Է� ������ �ִ� �÷��̾ ����
        if (!HasInputAuthority) return;

        // ī�޶� �Ǻ� ȸ�� ������Ʈ
        Vector2 pitchRotation = KCC.Data.GetLookRotation(true, false);
        CameraPivot.localRotation = Quaternion.Euler(pitchRotation);

        // ī�޶� ��ġ �� ȸ�� ����
        Camera.main.transform.SetPositionAndRotation(CameraHandle.position, CameraHandle.rotation);
    }

    private void UpdateAnimationParameters()
    {
        // Speed - ���� ĳ������ ���� �ӵ��� ������ �ִϸ��̼ǿ� ����
        Speed = KCC.Data.RealSpeed;

        // Grounded - ĳ���Ͱ� �ٴڿ� ����ִ��� ���θ� �����Ͽ� ���� ���¸� �ִϸ��̼ǿ� �ݿ�
        Grounded = KCC.Data.IsGrounded;

        // FreeFall - ĳ���Ͱ� ���߿� �� �ִ� ���¸� ��Ÿ����, �ٴڿ� ���� �ʴٸ� ���� ���� ���·� ó��
        FreeFall = !Grounded;

        // MotionSpeed - ������Ʈ ���¿� ���� ĳ������ �̵� �ӵ� ���� (1.5�� �ӵ� ����)
        MotionSpeed = KCC.Data.InputDirection.sqrMagnitude > 0 ? (KCC.Data.Sprint ? 1.5f : 1.0f) : 0.0f;

        IsCrouch = KCC.Data.Crouch;

        // �ִϸ����� �Ķ���� ����
        PlayerAnimator.Animator.SetFloat("Speed", Speed);
        PlayerAnimator.Animator.SetBool("Jump", Jump);
        PlayerAnimator.Animator.SetBool("Grounded", Grounded);
        PlayerAnimator.Animator.SetBool("FreeFall", FreeFall);
        PlayerAnimator.Animator.SetFloat("MotionSpeed", MotionSpeed);
        PlayerAnimator.Animator.SetBool("Interact", Interact);
        PlayerAnimator.Animator.SetBool("Crouch", IsCrouch);

        // ������ ��ȣ�ۿ� �Ķ���� ���� - �ִϸ��̼��� �� ���� ����ǵ��� �ʱ�ȭ
        Jump = false;
        Interact = false;
        IsCrouch = false;
    }
}

