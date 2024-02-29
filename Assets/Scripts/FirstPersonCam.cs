using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCam : MonoBehaviour
{
    private Camera mainCam = null;
    private float rotSpeed = 200f;
    private float rotX = 0f;
    private float rotY = 0f;

    

    bool lockedCursor = true; // 마우스 커서 고정

    void Start()
    {
        Cursor.visible = false; // 커서 숨김
        Cursor.lockState = CursorLockMode.Locked; // 커서 잠금
    }

    void Awake()
    {
        mainCam = Camera.main;
        Vector3 rot = mainCam.transform.rotation.eulerAngles;
        rotX = rot.x;
        rotY = rot.y;
    }

    private void CamControl()
    {
        float X = Input.GetAxisRaw("Mouse Y");
        float Y = Input.GetAxisRaw("Mouse X");

        rotX += -X * rotSpeed * Time.deltaTime;
        rotY += Y * rotSpeed * Time.deltaTime;
        mainCam.transform.rotation = Quaternion.Euler(rotX, rotY, 0f);
    }
    
    void Update()
    {
        CamControl();

    }
}
