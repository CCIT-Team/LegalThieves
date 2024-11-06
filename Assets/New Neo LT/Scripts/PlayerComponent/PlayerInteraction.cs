using UnityEngine;
using Fusion;

public interface IInteractable
{
    string GetInteractPrompt();
    void OnInteract(PlayerRef player);
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
                curInteractable.OnInteract(Object.InputAuthority);
            }
            else
            {
               
                // ȭ���� �� �߾ӿ� ��ȣ�ۿ� ������ ��ü�� ���� ���
                curInteractGameobject = null;
                curInteractable = null;
            }
        
    } 
    
}

