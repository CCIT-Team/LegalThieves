using LegalThieves;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove_H : MonoBehaviour
{
    RaycastHit hit;
    public float speed = 5f; // 이동 속도
    public float mouseSensitivity = 100f; // 마우스 감도 설정
    public Transform head; // 머리 부분
    private Rigidbody rb;
    bool canJump = true;
    float jummpPower = 4;
    private float xRotation = 0f; // x축 회전 값

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 커서 고정 및 숨김
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = new Vector3(h, 0, v).normalized;


        moveDir = transform.TransformDirection(moveDir);

        rb.MovePosition(transform.position + moveDir * Time.deltaTime * speed);
       

        //카메라
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        head.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
       
        //점프
        if (canJump && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jummpPower, ForceMode.Impulse);
            canJump = false;
            StartCoroutine (Jumping());
        }
    }
    IEnumerator Jumping()
    {
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            if (Physics.Raycast(transform.position , -Vector3.up, out hit))
            {
                Debug.Log(hit.distance);
                if (hit.distance <= 0.75f) { 
                    canJump = true;
                    break;
                }
            }
            yield return new WaitForSeconds (0.1f);
        }        
    }
}


