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
        
            // ray에 뭔가 충돌했다면 hit에 충돌한 오브젝트에 대한 정보가 넘어오게 된다.
            if (Physics.Raycast(camera.position, camera.forward, out RaycastHit hit, maxCheckDistance, layerMask))
            {
                curInteractGameobject = hit.collider.gameObject;
                curInteractable = hit.collider.GetComponent<IInteractable>();
                curInteractable.OnInteract(Object.InputAuthority);
            }
            else
            {
               
                // 화면의 정 중앙에 상호작용 가능한 물체가 없는 경우
                curInteractGameobject = null;
                curInteractable = null;
            }
        
    } 
    
}

