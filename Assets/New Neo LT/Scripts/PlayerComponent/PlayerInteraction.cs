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
            // 화면의 정 중앙에 상호작용 가능한 물체가 있는지 확인하기
            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2)); // 화면의 정 중앙에서 Ray를 쏘겠다.
            RaycastHit hit;

            // ray에 뭔가 충돌했다면 hit에 충돌한 오브젝트에 대한 정보가 넘어오게 된다.
            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {

                // 충돌한 물체 가져오기
                curInteractGameobject = hit.collider.gameObject;
                curInteractable = hit.collider.GetComponent<IInteractable>();
                curInteractable.OnInteract(Runner);
            }
            else
            {
                // 화면의 정 중앙에 상호작용 가능한 물체가 없는 경우
                curInteractGameobject = null;
                curInteractable = null;
            }
        }
    } 
    
}

