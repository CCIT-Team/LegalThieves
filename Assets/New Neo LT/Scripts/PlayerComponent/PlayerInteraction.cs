using UnityEngine;
using Fusion;

public interface IInteractable
{
    void OnInteract(PlayerRef player);
}

public class PlayerInteraction : NetworkBehaviour
{
    [SerializeField] private float maxCheckDistance;
    [SerializeField] private LayerMask layerMask;

   
    public void CheckInteraction(Transform cam)
    {
        if (!Physics.Raycast(cam.position, cam.forward, out var hit, maxCheckDistance, layerMask))
            return;
        if (!hit.collider.TryGetComponent<IInteractable>(out var interactable))
            return;

        interactable.OnInteract(Object.InputAuthority);
    } 
    
}

