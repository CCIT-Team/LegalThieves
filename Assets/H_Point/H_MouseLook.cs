using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f; // ���콺 ���� ����
    public Transform playerBody; // �÷��̾��� ��ü�� ����

    private float xRotation = 0f; // ī�޶��� x�� ȸ�� �� �ʱ�ȭ

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Ŀ���� ȭ�� �߾ӿ� ����
    }

    void Update()
    {
        // ���콺 �Է��� �޾� ȸ�� ���� ���
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY; // x�� ȸ���� ������Ʈ (��/�Ʒ�)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // ���� ����

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // ī�޶� ȸ�� ����
        playerBody.Rotate(Vector3.up * mouseX); // �÷��̾� ��ü ȸ�� ����
    }
}
