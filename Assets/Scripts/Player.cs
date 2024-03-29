using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;

    bool jDown;
    bool iDown;
    bool rDown;
    bool aDown; 

    bool isJump;
    bool isRun;
    bool isAttack;

    Vector3 moveVec;

    [SerializeField]
    private float lookSensitivity;

    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX;

    [SerializeField]
    private Camera theCamera;

    private Rigidbody myRigid;
    Animator animator;

    void Start()
    {
        myRigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        GetInput();
        Move();
        CameraRotation();
        CharacterRotation();
        Jump();
        Run();
        Die();
        Attack();


    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        jDown = Input.GetButtonDown("Jump");
        rDown = Input.GetButton("Run");
        aDown = Input.GetButtonDown("Attack");
        iDown = Input.GetButtonDown("Interation");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        Vector3 _moveHorizontal = transform.right * hAxis;
        Vector3 _moveVertical = transform.forward * vAxis;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * speed;

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
        animator.SetBool("isWalk", true);
        animator.SetBool("isRunning", false);
        if(vAxis == 0 && hAxis == 0)
        {
            animator.SetBool("isWalk", false);
            animator.SetBool("isRunning", false);
        }
    }

    void Jump()
    {
        if (jDown && !isJump)
        {
            myRigid.AddForce(Vector3.up * 5, ForceMode.Impulse);
            isJump = true;
            animator.SetBool("isJumping",true);
        }

    }

    void Run()
    {
        if (rDown)
        {

            isRun = true;
            speed = 30;
            animator.SetBool("isWalk", false);
            animator.SetBool("isRunning", true);
        }
        else
        {
            isRun = false;
            speed = 10;
        }
    }

    
    private void CharacterRotation() //좌우 캐릭터 회전
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }

    private void CameraRotation() //상하 카메라 회전
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isJump = false;
            animator.SetBool("isJumping", false);
        }
    }

    void Die()
    {
        if(HealthGauge.health == 0)
        {
            Debug.Log("DIE!");
        }
    }

    void Attack()
    {
        if (aDown)
        {
            isAttack = true;
            animator.SetBool("isAttack", true);
            Debug.Log("Attack!");
        }
        else
        {
            isAttack = false;
            animator.SetBool("isAttack", false);
        }
    }

   
}
