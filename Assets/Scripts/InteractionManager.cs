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
        // 마지막으로 체크한 시간이 checkRate를 넘겼다면
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            // 화면의 정 중앙에 상호작용 가능한 물체가 있는지 확인하기
            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2)); // 화면의 정 중앙에서 Ray를 쏘겠다.
            RaycastHit hit;

            // ray에 뭔가 충돌했다면 hit에 충돌한 오브젝트에 대한 정보가 넘어오게 된다.
            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                // 부딪힌 오브젝트가 우리가 저장해놓은 상호작용이 가능한 오브젝트들인지 확인하기
                if (hit.collider.gameObject != curInteractGameobject)
                {
                    // 충돌한 물체 가져오기
                    curInteractGameobject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    Debug.Log(curInteractable.ToString());
                }
            }
            else
            {
                // 화면의 정 중앙에 상호작용 가능한 물체가 없는 경우
                curInteractGameobject = null;
                curInteractable = null;
            }
        }

        // 상호작용 버튼이 눌렸을 때
        if (curInteractable != null && Input.GetKeyDown(KeyCode.E) && HasStateAuthority)
        {
            // 상호작용 처리
            curInteractable.OnInteract(Runner);
        }
    }
}

