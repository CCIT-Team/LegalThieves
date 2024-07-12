using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMove_k : MonoBehaviour
{
    //선언
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
        //인풋시스템 Generate C# 클래스를 이용해서 받아오기
        playerInput = new Player_k();

        //Move (눌렀을 때, 누르고 있을 때, 뗄 때 다 받아오기)
        playerInput.PlayerActions.Move.started += OnMove;
        playerInput.PlayerActions.Move.performed += OnMove;
        playerInput.PlayerActions.Move.canceled += OnMove;

        //Jump (눌렀을 때 받아오기)
        playerInput.PlayerActions.Jump.started += OnJump;

        //Mouse (움직임 시작, 도중, 종료 다 받아오기)
        playerInput.PlayerActions.MousePos.started += OnMousePos;
        playerInput.PlayerActions.MousePos.performed += OnMousePos;
        playerInput.PlayerActions.MousePos.canceled += OnMousePos;

    }

    void Update()
    {
        
        transform.Translate(moveVector.normalized * Time.deltaTime * moveSpeed); //OnMove에서 변화한 moveVector로 위치 이동
        Cursor.lockState = CursorLockMode.Locked; //화면에서 커서는 고정되고 숨겨짐
        transform.rotation = Quaternion.Euler(_rotation); //OnMousePos에서 변화한 _rotation으로 회전
    }

    public void ToggleCursor(bool toggle)
    {
        // 인벤토리 켜질 시 다시 마우스 포인터가 나타나도록 해주는 코드
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        // 인벤토리가 켜져 있는 동안 마우스가 회전하지 않도록 해주는 코드
        //canLook = !toggle;
    }
    public void OnMove(InputAction.CallbackContext value)
    {
        inputVector = value.ReadValue<Vector2>(); //움직임 값 받아오기
        moveVector = new Vector3(inputVector.x, 0f, inputVector.y); //움직임 값
    }

   
    public void OnJump(InputAction.CallbackContext value)
    {

        if (isGrounded)  //땅에 있나요?
        {         
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); //점프 연산
            isGrounded = false; //점프했으니까 false로
        }
            

    }

    void OnCollisionEnter(Collision collision) //충돌감지
    {
       if (collision.gameObject.tag == "Ground") //객체 태그로 동작
        {
            isGrounded = true; //땅이랑 닿았으니 true
        }
    }

    public void OnMousePos(InputAction.CallbackContext value)
    {
        Vector2 LookPos = playerInput.PlayerActions.MousePos.ReadValue<Vector2>()*Time.deltaTime; //마우스 움직인 값(델타) 받아오기
        if (_rotation.x <= 30f && _rotation.x >= -30f) //상하 각도 제한
        {
            _rotation.x += -LookPos.y * 5f * _sensitivity; //상하 회전 연산
        }
        else if (_rotation.x > 30f)
        {
            _rotation.x = 30f;
        }
        else if (_rotation.x < -30f)
        {
            _rotation.x = -30f;
        }

        _rotation.y += LookPos.x * 5f * _sensitivity; //좌우 회전 연산
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
