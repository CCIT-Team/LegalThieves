using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

public interface IInteractable
{
    string GetInteractPrompt();
    void OnInteract(NetworkRunner runner);
}

public class InteractionManager : NetworkBehaviour
{
    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance;
    public LayerMask layerMask;

    public GameObject curInteractGameobject;
    public IInteractable curInteractable;

    private Camera camera;

    void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        if (!HasStateAuthority) return;
        // ���������� üũ�� �ð��� checkRate�� �Ѱ�ٸ�
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            // ȭ���� �� �߾ӿ� ��ȣ�ۿ� ������ ��ü�� �ִ��� Ȯ���ϱ�
            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2)); // ȭ���� �� �߾ӿ��� Ray�� ��ڴ�.
            RaycastHit hit;

            // ray�� ���� �浹�ߴٸ� hit�� �浹�� ������Ʈ�� ���� ������ �Ѿ���� �ȴ�.
            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                // �ε��� ������Ʈ�� �츮�� �����س��� ��ȣ�ۿ��� ������ ������Ʈ������ Ȯ���ϱ�
                if (hit.collider.gameObject != curInteractGameobject)
                {
                    // �浹�� ��ü ��������
                    curInteractGameobject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    Debug.Log(curInteractable.ToString());
                }
            }
            else
            {
                // ȭ���� �� �߾ӿ� ��ȣ�ۿ� ������ ��ü�� ���� ���
                curInteractGameobject = null;
                curInteractable = null;
            }
        }

        // ��ȣ�ۿ� ��ư�� ������ ��
        if (curInteractable != null && Input.GetKeyDown(KeyCode.E) && HasStateAuthority)
        {
            // ��ȣ�ۿ� ó��
            curInteractable.OnInteract(Runner);
        }
    }
}

