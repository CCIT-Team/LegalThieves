using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f; // 마우스 감도 설정
    public Transform playerBody; // 플레이어의 몸체를 참조

    private float xRotation = 0f; // 카메라의 x축 회전 값 초기화

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 커서를 화면 중앙에 고정
    }

    void Update()
    {
        // 마우스 입력을 받아 회전 값을 계산
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY; // x축 회전을 업데이트 (위/아래)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // 시점 제한

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // 카메라 회전 적용
        playerBody.Rotate(Vector3.up * mouseX); // 플레이어 몸체 회전 적용
    }
}
