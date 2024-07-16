using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;
using static UnityEngine.InputSystem.InputAction;
using Fusion;
using LegalThieves;

public interface IInteractable
{
    string GetInteractPrompt();
    void OnInteract();
}

public class InteractionManager_k : NetworkBehaviour
{
    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance;
    public LayerMask layerMask;
    public TempPlayer tempPlayer;
    private GameObject curInteractGameobject;
    private IInteractable curInteractable;
    public TextMeshProUGUI promptText;
    private Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;

    }
    // Update is called once per frame
    void Update()
    {

        // 마지막으로 체크한 시간이 checkRate를 넘겼다면
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;
            // 화면의 정 중앙에 상호작용 가능한 물체가 있는지 확인하기

            // Raycast의 순서는 1. 발사할 Ray를 생성해준다. 2. 발사 및 충돌 여부 확인
            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));    // 화면의 정 중앙에서 Ray를 쏘겠다.
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
                    if (curInteractGameobject.CompareTag("Items"))
                    {
                        Debug.Log("줍기");
                        //SetPromptTextItems();
                    }
                    else if (curInteractGameobject.CompareTag("Relics"))
                    {
                        Debug.Log("발굴");
                        //SetPromptTextRelics();
                    }
                }
            }
            else
            {
                // 화면의 정 중앙에 상호작용 가능한 물체가 없는 경우
                curInteractGameobject = null;
                curInteractable = null;
                //promptText.gameObject.SetActive(false);
            }
        }
    }

    private void SetPromptTextItems()
    {
        //promptText.gameObject.SetActive(true);
        //promptText.text = string.Format("<b>[E]</b> {0}", curInteractable.GetInteractPrompt() + " 획득");     // <b></b> : 태그, 마크다운 형식 <b>의 경우 볼드체.
    }
    private void SetPromptTextRelics()
    {
        //promptText.gameObject.SetActive(true);
        //promptText.text = string.Format("<b>[F]</b> {0}", curInteractable.GetInteractPrompt() + " 발굴");     // <b></b> : 태그, 마크다운 형식 <b>의 경우 볼드체.
    }

    public void OnInteractInput(InputAction.CallbackContext callbackContext)
    {
        // E 키를 누른 시점에 현재 바라보는 Interactable 오브젝트가 있다면
        if (callbackContext.phase == InputActionPhase.Started && curInteractable != null && curInteractGameobject.CompareTag("Items"))
        {
           
            Debug.Log("아이템 획득");
            // 아이템과 획득하면 아이템과의 상호작용을 진행하고 초기화 해준다.
            curInteractable.OnInteract();
            curInteractGameobject = null;
            curInteractable = null;
            //promptText.gameObject.SetActive(false);
        }
           
        
    }
    public void OnExcavate(InputAction.CallbackContext callbackContext)
    {
        // 현재 바라보는 Interactable 오브젝트가 있다면
        if (curInteractable != null && curInteractGameobject.CompareTag("Relics") )
        {
            if (callbackContext.phase == InputActionPhase.Started)
            {
                TempPlayer.animator.SetBool("isInteracting", true);
                Debug.Log("유물발굴 시도중");
            }
            else if(callbackContext.phase == InputActionPhase.Performed)
            {
                TempPlayer.animator.SetBool("isInteracting", false);
                Debug.Log("유물발굴 성공");
                curInteractable.OnInteract();
                curInteractGameobject = null;
                curInteractable = null;
                //promptText.gameObject.SetActive(false);
            }
            else if(callbackContext.phase == InputActionPhase.Canceled)
            {
                TempPlayer.animator.SetBool("isInteracting", false);
                Debug.Log("유물발굴 실패");
            }
            
        }
        
    }

    
}
