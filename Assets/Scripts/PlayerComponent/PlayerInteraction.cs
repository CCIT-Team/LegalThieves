using UnityEngine;
using Fusion;
using New_Neo_LT.Scripts.PlayerComponent;
using New_Neo_LT.Scripts.Relic;

public interface IInteractable
{
    void OnServer_Interact(PlayerRef player);
    void OnClient_Interact(PlayerRef player);
}

public class PlayerInteraction : NetworkBehaviour
{
    [SerializeField] private float maxCheckDistance;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private CapsuleCollider ScanCollider;
    private Transform _camTarget;

    public override void Spawned()
    {
        base.Spawned();
        _camTarget = Object.GetComponent<PlayerCharacter>().GetCamTarget();
        UpdateColliderSize();
    }

    private void UpdateColliderSize()
    {
        if (ScanCollider == null) return;

        // CapsuleCollider의 높이를 Ray 길이에 맞춤
        ScanCollider.height = maxCheckDistance;

        // CapsuleCollider의 중심을 Raycast 방향에 맞춰 설정
        ScanCollider.center = new Vector3(0, 0, maxCheckDistance / 2);

    }
    public void Server_CheckInteraction()
    {
        if (!Physics.Raycast(_camTarget.position, _camTarget.forward, out var hit, maxCheckDistance, layerMask))
            return;
        if (!hit.collider.TryGetComponent<IInteractable>(out var interactable))
            return;
        
        interactable.OnServer_Interact(Object.InputAuthority);
    } 
    
    public void CheckInteraction()
    {
        if (!Physics.Raycast(_camTarget.position, _camTarget.forward, out var hit, maxCheckDistance, layerMask))
            return;
        if (!hit.collider.TryGetComponent<IInteractable>(out var interactable))
            return;
        
        interactable.OnClient_Interact(Object.InputAuthority);
    } 
}

