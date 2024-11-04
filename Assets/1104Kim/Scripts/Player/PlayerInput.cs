using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;
using Fusion.Addons.KCC;

/// <summary>
/// 네트워크 입력 데이터 구조체. 이동 방향, 회전 값, 버튼 입력을 포함.
/// </summary>
public struct InputData : INetworkInput
{
    public Vector2 MoveDirection; // 이동 방향
    public Vector2 LookRotationDelta; // 회전 델타값
    public NetworkButtons Actions; // 네트워크 상의 버튼 입력 상태

    // 버튼 인덱스 정의
    public const int JUMP_BUTTON = 0;
    public const int SPRINT_BUTTON = 1;
    public const int HEAL_BUTTON = 2;
    public const int DAMAGE_BUTTON = 3;
    public const int INTERACT_BUTTON = 4;
    public const int CROUCH_BUTTON = 5;
}

/// <summary>
/// PlayerInput 클래스는 네트워크에서 입력을 처리하며, 게임 내에서 플레이어의 움직임과 행동을 처리한다.
/// </summary>
public sealed class PlayerInput : NetworkBehaviour, IBeforeUpdate, IBeforeTick
{
    // 현재와 이전 입력 상태를 가져오는 속성
    public InputData CurrentInput => _currentInput;
    public InputData PreviousInput => _previousInput;

    [SerializeField]
    [Tooltip("마우스 델타 민감도")]
    private Vector2 _lookSensitivity = Vector2.one;

    [Networked]
    private InputData _currentInput { get; set; } // 현재 입력 상태

    private InputData _previousInput; // 이전 입력 상태
    private InputData _accumulatedInput; // 누적 입력
    private bool _resetAccumulatedInput; // 누적 입력을 리셋할지 여부
    private Vector2Accumulator _lookRotationAccumulator = new Vector2Accumulator(0.02f, true); // 회전 값 누적

    public override void Spawned()
    {
        // 기본 값으로 초기화
        _currentInput = default;
        _previousInput = default;
        _accumulatedInput = default;
        _resetAccumulatedInput = default;

        if (HasInputAuthority == true)
        {
            // 로컬 플레이어의 입력 폴링을 등록
            NetworkEvents networkEvents = Runner.GetComponent<NetworkEvents>();
            networkEvents.OnInput.AddListener(OnInput);

            // 모바일 플랫폼이 아니거나, 에디터라면 커서 숨기기
            if (Application.isMobilePlatform == false || Application.isEditor == true)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        // 로컬 플레이어에게만 네트워크 속성 동기화 (트래픽 절약)
       // ReplicateToAll(false);
       // ReplicateTo(Object.InputAuthority, true);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (runner == null)
            return;

        // 로컬 플레이어 입력 폴링 해제
        NetworkEvents networkEvents = runner.GetComponent<NetworkEvents>();
        if (networkEvents != null)
        {
            networkEvents.OnInput.RemoveListener(OnInput);
        }
    }

    /// <summary>
    /// 입력 장치에서 데이터를 수집. FixedUpdateNetwork() 호출 사이에 여러 번 실행 가능.
    /// </summary>
    void IBeforeUpdate.BeforeUpdate()
    {
        if (HasInputAuthority == false)
            return;

        // 누적 입력이 폴링 되었고, 리셋 요청이 있으면 초기화
        if (_resetAccumulatedInput == true)
        {
            _resetAccumulatedInput = false;
            _accumulatedInput = default;
        }

        // 모바일 플랫폼이 아니거나 에디터라면 커서가 잠긴 상태에서만 입력 추적
        if (Application.isMobilePlatform == false || Application.isEditor == true)
        {
            if (Cursor.lockState != CursorLockMode.Locked)
                return;
        }

        ProcessStandaloneInput(); // 독립 실행형 장치에서 입력 처리
    }

    /// <summary>
    /// Fusion에서 입력을 읽어들임.
    /// </summary>
    void IBeforeTick.BeforeTick()
    {
        if (Object == null)
            return;

        // 현재 입력을 이전 입력으로 설정
        _previousInput = _currentInput;

        // LookRotationDelta는 새 입력이 없는 경우 오류 방지를 위해 기본값으로 초기화
        InputData currentInput = _currentInput;
        currentInput.LookRotationDelta = default;
        _currentInput = currentInput;

        // 새 입력이 있다면 현재 입력으로 저장
        if (Object.InputAuthority != PlayerRef.None)
        {
            if (GetInput(out InputData input) == true)
            {
                _currentInput = input;
            }
        }
    }

    /// <summary>
    /// 누적된 입력을 푸시하고 속성을 초기화. 주로 렌더링 속도가 Fusion 시뮬레이션보다 느릴 때 실행.
    /// </summary>
    private void OnInput(NetworkRunner runner, NetworkInput networkInput)
    {
        // 마우스 움직임(델타 값)을 엔진 업데이트와 일치시킴
        _accumulatedInput.LookRotationDelta = _lookRotationAccumulator.ConsumeTickAligned(runner);

        // 누적된 입력 설정
        networkInput.Set(_accumulatedInput);

        // OnInput()이 여러 번 실행될 수 있으므로 플래그로 리셋 처리
        _resetAccumulatedInput = true;
    }

    private void ProcessStandaloneInput()
    {
        // 항상 KeyControl.isPressed, Input.GetMouseButton(), Input.GetKey()를 사용하여 키 입력 상태 확인
        Mouse mouse = Mouse.current;
        if (mouse != null)
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();
            _lookRotationAccumulator.Accumulate(new Vector2(-mouseDelta.y, mouseDelta.x) * _lookSensitivity);
        }

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            Vector2 moveDirection = Vector2.zero;

            // 키보드 입력으로 이동 방향 계산
            if (keyboard.wKey.isPressed == true) { moveDirection += Vector2.up; }
            if (keyboard.sKey.isPressed == true) { moveDirection += Vector2.down; }
            if (keyboard.aKey.isPressed == true) { moveDirection += Vector2.left; }
            if (keyboard.dKey.isPressed == true) { moveDirection += Vector2.right; }

            _accumulatedInput.MoveDirection = moveDirection.normalized;

            // 각 키 상태를 버튼 입력에 반영
            _accumulatedInput.Actions.Set(InputData.JUMP_BUTTON, keyboard.spaceKey.isPressed);
            _accumulatedInput.Actions.Set(InputData.SPRINT_BUTTON, keyboard.leftShiftKey.isPressed);
            _accumulatedInput.Actions.Set(InputData.HEAL_BUTTON, keyboard.hKey.isPressed);
            _accumulatedInput.Actions.Set(InputData.DAMAGE_BUTTON, keyboard.jKey.isPressed);
            _accumulatedInput.Actions.Set(InputData.INTERACT_BUTTON, keyboard.eKey.isPressed);
            _accumulatedInput.Actions.Set(InputData.CROUCH_BUTTON, keyboard.leftCtrlKey.isPressed);
        }
    }
}
