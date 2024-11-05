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

   
    public void CheckInteraction(Transform camera)
    {
        
            // ray�� ���� �浹�ߴٸ� hit�� �浹�� ������Ʈ�� ���� ������ �Ѿ���� �ȴ�.
            if (Physics.Raycast(camera.position, camera.forward, out RaycastHit hit, maxCheckDistance, layerMask))
            {
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

