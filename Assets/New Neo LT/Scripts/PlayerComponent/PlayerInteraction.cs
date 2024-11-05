using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;
using Fusion.Addons.KCC;

public interface IInteractable
{
    string GetInteractPrompt();
    void OnInteract(NetworkRunner runner);
}

public class PlayerInteraction : NetworkBehaviour
{
    public float maxCheckDistance;
    public LayerMask layerMask;

    public GameObject curInteractGameobject;
    public IInteractable curInteractable;

    private Camera camera;

    void Start()
    {
        camera = Camera.main;
    }

    public void CheckInteraction()
    {
        if (HasStateAuthority)
        {
            // ȭ���� �� �߾ӿ� ��ȣ�ۿ� ������ ��ü�� �ִ��� Ȯ���ϱ�
            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2)); // ȭ���� �� �߾ӿ��� Ray�� ��ڴ�.
            RaycastHit hit;

            // ray�� ���� �浹�ߴٸ� hit�� �浹�� ������Ʈ�� ���� ������ �Ѿ���� �ȴ�.
            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {

                // �浹�� ��ü ��������
                curInteractGameobject = hit.collider.gameObject;
                curInteractable = hit.collider.GetComponent<IInteractable>();
                curInteractable.OnInteract(Runner);
            }
            else
            {
                // ȭ���� �� �߾ӿ� ��ȣ�ۿ� ������ ��ü�� ���� ���
                curInteractGameobject = null;
                curInteractable = null;
            }
        }
    } 
    
}

