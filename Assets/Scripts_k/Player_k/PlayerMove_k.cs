using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMove_k : MonoBehaviour
{
    //����
    Vector2 inputVector;
    Vector3 moveVector;
    Vector2 _rotation;
    
    public float moveSpeed = 4f;
    public float _sensitivity = 20f;
    public float jumpForce = 5f;
    public bool isGrounded;

    private Rigidbody rb;
    private Animator animator;
    private Player_k playerInput;

    [SerializeField] private Transform head;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
    private void Awake()
    {
        //��ǲ�ý��� Generate C# Ŭ������ �̿��ؼ� �޾ƿ���
        playerInput = new Player_k();

        //Move (������ ��, ������ ���� ��, �� �� �� �޾ƿ���)
        playerInput.PlayerActions.Move.started += OnMove;
        playerInput.PlayerActions.Move.performed += OnMove;
        playerInput.PlayerActions.Move.canceled += OnMove;

        //Jump (������ �� �޾ƿ���)
        playerInput.PlayerActions.Jump.started += OnJump;

        //Mouse (������ ����, ����, ���� �� �޾ƿ���)
        playerInput.PlayerActions.MousePos.started += OnMousePos;
        playerInput.PlayerActions.MousePos.performed += OnMousePos;
        playerInput.PlayerActions.MousePos.canceled += OnMousePos;

    }

    void Update()
    {
        
        transform.Translate(moveVector.normalized * Time.deltaTime * moveSpeed); //OnMove���� ��ȭ�� moveVector�� ��ġ �̵�
        Cursor.lockState = CursorLockMode.Locked; //ȭ�鿡�� Ŀ���� �����ǰ� ������
        transform.rotation = Quaternion.Euler(_rotation); //OnMousePos���� ��ȭ�� _rotation���� ȸ��
    }

    public void ToggleCursor(bool toggle)
    {
        // �κ��丮 ���� �� �ٽ� ���콺 �����Ͱ� ��Ÿ������ ���ִ� �ڵ�
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        // �κ��丮�� ���� �ִ� ���� ���콺�� ȸ������ �ʵ��� ���ִ� �ڵ�
        //canLook = !toggle;
    }
    public void OnMove(InputAction.CallbackContext value)
    {
        inputVector = value.ReadValue<Vector2>(); //������ �� �޾ƿ���
        moveVector = new Vector3(inputVector.x, 0f, inputVector.y); //������ ��
    }

   
    public void OnJump(InputAction.CallbackContext value)
    {

        if (isGrounded)  //���� �ֳ���?
        {         
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); //���� ����
            isGrounded = false; //���������ϱ� false��
        }
            

    }

    void OnCollisionEnter(Collision collision) //�浹����
    {
       if (collision.gameObject.tag == "Ground") //��ü �±׷� ����
        {
            isGrounded = true; //���̶� ������� true
        }
    }

    public void OnMousePos(InputAction.CallbackContext value)
    {
        Vector2 LookPos = playerInput.PlayerActions.MousePos.ReadValue<Vector2>()*Time.deltaTime; //���콺 ������ ��(��Ÿ) �޾ƿ���
        if (_rotation.x <= 30f && _rotation.x >= -30f) //���� ���� ����
        {
            _rotation.x += -LookPos.y * 5f * _sensitivity; //���� ȸ�� ����
        }
        else if (_rotation.x > 30f)
        {
            _rotation.x = 30f;
        }
        else if (_rotation.x < -30f)
        {
            _rotation.x = -30f;
        }

        _rotation.y += LookPos.x * 5f * _sensitivity; //�¿� ȸ�� ����
    }


    private void OnEnable()
    {
        playerInput.PlayerActions.Enable();
    }

    private void OnDisable()
    {
        playerInput.PlayerActions.Disable();
    }
}
