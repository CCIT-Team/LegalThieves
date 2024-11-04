using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;
using Fusion.Addons.KCC;

/// <summary>
/// ��Ʈ��ũ �Է� ������ ����ü. �̵� ����, ȸ�� ��, ��ư �Է��� ����.
/// </summary>
public struct InputData : INetworkInput
{
    public Vector2 MoveDirection; // �̵� ����
    public Vector2 LookRotationDelta; // ȸ�� ��Ÿ��
    public NetworkButtons Actions; // ��Ʈ��ũ ���� ��ư �Է� ����

    // ��ư �ε��� ����
    public const int JUMP_BUTTON = 0;
    public const int SPRINT_BUTTON = 1;
    public const int HEAL_BUTTON = 2;
    public const int DAMAGE_BUTTON = 3;
    public const int INTERACT_BUTTON = 4;
    public const int CROUCH_BUTTON = 5;
}

/// <summary>
/// PlayerInput Ŭ������ ��Ʈ��ũ���� �Է��� ó���ϸ�, ���� ������ �÷��̾��� �����Ӱ� �ൿ�� ó���Ѵ�.
/// </summary>
public sealed class PlayerInput : NetworkBehaviour, IBeforeUpdate, IBeforeTick
{
    // ����� ���� �Է� ���¸� �������� �Ӽ�
    public InputData CurrentInput => _currentInput;
    public InputData PreviousInput => _previousInput;

    [SerializeField]
    [Tooltip("���콺 ��Ÿ �ΰ���")]
    private Vector2 _lookSensitivity = Vector2.one;

    [Networked]
    private InputData _currentInput { get; set; } // ���� �Է� ����

    private InputData _previousInput; // ���� �Է� ����
    private InputData _accumulatedInput; // ���� �Է�
    private bool _resetAccumulatedInput; // ���� �Է��� �������� ����
    private Vector2Accumulator _lookRotationAccumulator = new Vector2Accumulator(0.02f, true); // ȸ�� �� ����

    public override void Spawned()
    {
        // �⺻ ������ �ʱ�ȭ
        _currentInput = default;
        _previousInput = default;
        _accumulatedInput = default;
        _resetAccumulatedInput = default;

        if (HasInputAuthority == true)
        {
            // ���� �÷��̾��� �Է� ������ ���
            NetworkEvents networkEvents = Runner.GetComponent<NetworkEvents>();
            networkEvents.OnInput.AddListener(OnInput);

            // ����� �÷����� �ƴϰų�, �����Ͷ�� Ŀ�� �����
            if (Application.isMobilePlatform == false || Application.isEditor == true)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        // ���� �÷��̾�Ը� ��Ʈ��ũ �Ӽ� ����ȭ (Ʈ���� ����)
       // ReplicateToAll(false);
       // ReplicateTo(Object.InputAuthority, true);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (runner == null)
            return;

        // ���� �÷��̾� �Է� ���� ����
        NetworkEvents networkEvents = runner.GetComponent<NetworkEvents>();
        if (networkEvents != null)
        {
            networkEvents.OnInput.RemoveListener(OnInput);
        }
    }

    /// <summary>
    /// �Է� ��ġ���� �����͸� ����. FixedUpdateNetwork() ȣ�� ���̿� ���� �� ���� ����.
    /// </summary>
    void IBeforeUpdate.BeforeUpdate()
    {
        if (HasInputAuthority == false)
            return;

        // ���� �Է��� ���� �Ǿ���, ���� ��û�� ������ �ʱ�ȭ
        if (_resetAccumulatedInput == true)
        {
            _resetAccumulatedInput = false;
            _accumulatedInput = default;
        }

        // ����� �÷����� �ƴϰų� �����Ͷ�� Ŀ���� ��� ���¿����� �Է� ����
        if (Application.isMobilePlatform == false || Application.isEditor == true)
        {
            if (Cursor.lockState != CursorLockMode.Locked)
                return;
        }

        ProcessStandaloneInput(); // ���� ������ ��ġ���� �Է� ó��
    }

    /// <summary>
    /// Fusion���� �Է��� �о����.
    /// </summary>
    void IBeforeTick.BeforeTick()
    {
        if (Object == null)
            return;

        // ���� �Է��� ���� �Է����� ����
        _previousInput = _currentInput;

        // LookRotationDelta�� �� �Է��� ���� ��� ���� ������ ���� �⺻������ �ʱ�ȭ
        InputData currentInput = _currentInput;
        currentInput.LookRotationDelta = default;
        _currentInput = currentInput;

        // �� �Է��� �ִٸ� ���� �Է����� ����
        if (Object.InputAuthority != PlayerRef.None)
        {
            if (GetInput(out InputData input) == true)
            {
                _currentInput = input;
            }
        }
    }

    /// <summary>
    /// ������ �Է��� Ǫ���ϰ� �Ӽ��� �ʱ�ȭ. �ַ� ������ �ӵ��� Fusion �ùķ��̼Ǻ��� ���� �� ����.
    /// </summary>
    private void OnInput(NetworkRunner runner, NetworkInput networkInput)
    {
        // ���콺 ������(��Ÿ ��)�� ���� ������Ʈ�� ��ġ��Ŵ
        _accumulatedInput.LookRotationDelta = _lookRotationAccumulator.ConsumeTickAligned(runner);

        // ������ �Է� ����
        networkInput.Set(_accumulatedInput);

        // OnInput()�� ���� �� ����� �� �����Ƿ� �÷��׷� ���� ó��
        _resetAccumulatedInput = true;
    }

    private void ProcessStandaloneInput()
    {
        // �׻� KeyControl.isPressed, Input.GetMouseButton(), Input.GetKey()�� ����Ͽ� Ű �Է� ���� Ȯ��
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

            // Ű���� �Է����� �̵� ���� ���
            if (keyboard.wKey.isPressed == true) { moveDirection += Vector2.up; }
            if (keyboard.sKey.isPressed == true) { moveDirection += Vector2.down; }
            if (keyboard.aKey.isPressed == true) { moveDirection += Vector2.left; }
            if (keyboard.dKey.isPressed == true) { moveDirection += Vector2.right; }

            _accumulatedInput.MoveDirection = moveDirection.normalized;

            // �� Ű ���¸� ��ư �Է¿� �ݿ�
            _accumulatedInput.Actions.Set(InputData.JUMP_BUTTON, keyboard.spaceKey.isPressed);
            _accumulatedInput.Actions.Set(InputData.SPRINT_BUTTON, keyboard.leftShiftKey.isPressed);
            _accumulatedInput.Actions.Set(InputData.HEAL_BUTTON, keyboard.hKey.isPressed);
            _accumulatedInput.Actions.Set(InputData.DAMAGE_BUTTON, keyboard.jKey.isPressed);
            _accumulatedInput.Actions.Set(InputData.INTERACT_BUTTON, keyboard.eKey.isPressed);
            _accumulatedInput.Actions.Set(InputData.CROUCH_BUTTON, keyboard.leftCtrlKey.isPressed);
        }
    }
}
