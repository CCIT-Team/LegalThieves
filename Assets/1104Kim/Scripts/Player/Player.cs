using UnityEngine;
using Fusion;
using Fusion.Addons.KCC;
using TMPro;

/// <summary>
/// Player 클래스: KCC(키 캐릭터 컨트롤러)를 활용한 고급 입력 처리 및 플레이어 제어
/// </summary>
[DefaultExecutionOrder(-5)]
public sealed class Player : NetworkBehaviour
{
    // KCC 및 기타 필요한 컴포넌트와 데이터 참조
    public KCC KCC;
    public PlayerInput Input;
    public Transform CameraPivot;
    public Transform CameraHandle;
    public NetworkObject PlayerObject;
    public NetworkMecanimAnimator PlayerAnimator;
    public InteractionManager interactionManager;
    public Health health;
    public TextMeshPro Name;

    // 애니메이션에 사용할 파라미터
    public float Speed;
    public bool Jump;
    public bool Interact;
    public bool IsCrouch;
    public bool Grounded;
    public bool FreeFall;
    public float MotionSpeed;

    public void Update()
    {
        // 플레이어 이름과 체력을 표시
        Name.text = PlayerObject.Id.ToString() + health.CurrentHealth;
    }

    public override void FixedUpdateNetwork()
    {
        // 애니메이션 파라미터 업데이트
        UpdateAnimationParameters();

        // 카메라 회전을 입력된 값으로 업데이트
        KCC.AddLookRotation(Input.CurrentInput.LookRotationDelta);

        // 플레이어의 월드 공간에서의 이동 방향 설정
        Vector3 inputDirection = KCC.Data.TransformRotation * new Vector3(Input.CurrentInput.MoveDirection.x, 0.0f, Input.CurrentInput.MoveDirection.y);
        KCC.SetInputDirection(inputDirection);

        // 점프 처리 - 점프 버튼이 눌렸고 플레이어가 바닥에 있을 때만 점프
        if (Input.CurrentInput.Actions.WasPressed(Input.PreviousInput.Actions, InputData.JUMP_BUTTON) && KCC.Data.IsGrounded)
        {
            KCC.Jump(Vector3.up * 6.0f); // 점프 힘 설정
            Jump = true; // 애니메이션 파라미터를 위한 점프 상태 설정
        }

        // 스프린트 처리 - 스프린트 버튼이 눌리면 캐릭터 속도 증가
        if (KCC.Data.IsGrounded)
        {
            KCC.SetSprint(Input.CurrentInput.Actions.IsSet(InputData.SPRINT_BUTTON));
            KCC.SetCrouch(Input.CurrentInput.Actions.IsSet(InputData.CROUCH_BUTTON));
        }

        // 체력 관리 - 치유 및 데미지 버튼이 눌렸을 때 처리
        if (Input.CurrentInput.Actions.WasPressed(Input.PreviousInput.Actions, InputData.HEAL_BUTTON))
        {
            health.AddHealth(10f); // 체력 추가
        }
        if (Input.CurrentInput.Actions.WasPressed(Input.PreviousInput.Actions, InputData.DAMAGE_BUTTON))
        {
            health.ApplyDamage(Object.InputAuthority, 10f); // 데미지 적용
        }

        // 상호작용 처리 - 상호작용 가능한 오브젝트가 있고, 상호작용 버튼이 눌렸을 때 처리
        if (interactionManager.curInteractable != null && Input.CurrentInput.Actions.WasPressed(Input.PreviousInput.Actions, InputData.INTERACT_BUTTON))
        {
            interactionManager.curInteractable.OnInteract(Runner); // 상호작용 수행
            interactionManager.curInteractGameobject = null;
            interactionManager.curInteractable = null;
            Interact = true; // 애니메이션 파라미터 설정
        }
    }

    private void LateUpdate()
    {
        // 카메라 갱신은 입력 권한이 있는 플레이어만 수행
        if (!HasInputAuthority) return;

        // 카메라 피봇 회전 업데이트
        Vector2 pitchRotation = KCC.Data.GetLookRotation(true, false);
        CameraPivot.localRotation = Quaternion.Euler(pitchRotation);

        // 카메라 위치 및 회전 설정
        Camera.main.transform.SetPositionAndRotation(CameraHandle.position, CameraHandle.rotation);
    }

    private void UpdateAnimationParameters()
    {
        // Speed - 현재 캐릭터의 실제 속도를 가져와 애니메이션에 적용
        Speed = KCC.Data.RealSpeed;

        // Grounded - 캐릭터가 바닥에 닿아있는지 여부를 설정하여 착지 상태를 애니메이션에 반영
        Grounded = KCC.Data.IsGrounded;

        // FreeFall - 캐릭터가 공중에 떠 있는 상태를 나타내며, 바닥에 있지 않다면 자유 낙하 상태로 처리
        FreeFall = !Grounded;

        // MotionSpeed - 스프린트 상태에 따라 캐릭터의 이동 속도 설정 (1.5배 속도 증가)
        MotionSpeed = KCC.Data.InputDirection.sqrMagnitude > 0 ? (KCC.Data.Sprint ? 1.5f : 1.0f) : 0.0f;

        IsCrouch = KCC.Data.Crouch;

        // 애니메이터 파라미터 설정
        PlayerAnimator.Animator.SetFloat("Speed", Speed);
        PlayerAnimator.Animator.SetBool("Jump", Jump);
        PlayerAnimator.Animator.SetBool("Grounded", Grounded);
        PlayerAnimator.Animator.SetBool("FreeFall", FreeFall);
        PlayerAnimator.Animator.SetFloat("MotionSpeed", MotionSpeed);
        PlayerAnimator.Animator.SetBool("Interact", Interact);
        PlayerAnimator.Animator.SetBool("Crouch", IsCrouch);

        // 점프와 상호작용 파라미터 리셋 - 애니메이션이 한 번만 실행되도록 초기화
        Jump = false;
        Interact = false;
        IsCrouch = false;
    }
}

