using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;
using static UnityEngine.InputSystem.InputAction;
using Fusion;
using LegalThieves;
using ExitGames.Client.Photon.StructWrapping;
using System;
/*
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
    [SerializeField] private Item_Torch_Temp Torch;
    private GameObject curInteractGameobject;
    private IInteractable curInteractable;
    public TextMeshProUGUI promptText;
    private Camera camera;
    private bool isPicked = false;
    private bool PickTorch = false;
    private bool isAttacking = false;
    [SerializeField] private GameObject[] EquipItems;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;

    }
    // Update is called once per frame
    void Update()
    {
        // ���������� üũ�� �ð��� checkRate�� �Ѱ�ٸ�
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;
            // ȭ���� �� �߾ӿ� ��ȣ�ۿ� ������ ��ü�� �ִ��� Ȯ���ϱ�

            // Raycast�� ������ 1. �߻��� Ray�� �������ش�. 2. �߻� �� �浹 ���� Ȯ��
            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));    // ȭ���� �� �߾ӿ��� Ray�� ��ڴ�.
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
                    if (curInteractGameobject.CompareTag("Items"))
                    {
                        Debug.Log("�ݱ�");
                        //SetPromptTextItems();
                    }
                    else if (curInteractGameobject.CompareTag("Relics"))
                    {
                        Debug.Log("�߱�");
                        //SetPromptTextRelics();
                    }
                }
            }
            else
            {
                // ȭ���� �� �߾ӿ� ��ȣ�ۿ� ������ ��ü�� ���� ���
                curInteractGameobject = null;
                curInteractable = null;
                //promptText.gameObject.SetActive(false);
            }
        }
    }

    private void SetPromptTextItems()
    {
        //promptText.gameObject.SetActive(true);
        //promptText.text = string.Format("<b>[E]</b> {0}", curInteractable.GetInteractPrompt() + " ȹ��");     // <b></b> : �±�, ��ũ�ٿ� ���� <b>�� ��� ����ü.
    }
    private void SetPromptTextRelics()
    {
        //promptText.gameObject.SetActive(true);
        //promptText.text = string.Format("<b>[F]</b> {0}", curInteractable.GetInteractPrompt() + " �߱�");     // <b></b> : �±�, ��ũ�ٿ� ���� <b>�� ��� ����ü.
    }

    public void OnInteractInput(InputAction.CallbackContext callbackContext)
    {
        // E Ű�� ���� ������ ���� �ٶ󺸴� Interactable ������Ʈ�� �ִٸ�
        if (callbackContext.phase == InputActionPhase.Started && curInteractable != null && curInteractGameobject.CompareTag("Items"))
        {

            Debug.Log("������ ȹ��");
            TempPlayer.animator.SetTrigger("isPickUp");
            // �����۰� ȹ���ϸ� �����۰��� ��ȣ�ۿ��� �����ϰ� �ʱ�ȭ ���ش�.
            curInteractable.OnInteract();
            curInteractGameobject = null;
            curInteractable = null;
            //promptText.gameObject.SetActive(false);
        }


    }
    public void OnExcavate(InputAction.CallbackContext callbackContext)
    {
        // ���� �ٶ󺸴� Interactable ������Ʈ�� �ִٸ�
        if (curInteractable != null && curInteractGameobject.CompareTag("Relics"))
        {
            if (callbackContext.phase == InputActionPhase.Started)
            {
                TempPlayer.animator.Animator.SetBool("isInteracting", true);
                Debug.Log("�����߱� �õ���");
            }
            else if (callbackContext.phase == InputActionPhase.Performed)
            {
                TempPlayer.animator.Animator.SetBool("isInteracting", false);
                Debug.Log("�����߱� ����");
                curInteractable.OnInteract();
                curInteractGameobject = null;
                curInteractable = null;
                //promptText.gameObject.SetActive(false);
            }
            else if (callbackContext.phase == InputActionPhase.Canceled)
            {
                TempPlayer.animator.Animator.SetBool("isInteracting", false);
                Debug.Log("�����߱� ����");
            }

        }

    }

    public void OnAttack(InputAction.CallbackContext callbackContext)
    {
        if (!isAttacking && isPicked && PickTorch && callbackContext.phase == InputActionPhase.Started)
        {
            StartCoroutine(Attack());
        }

    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        TempPlayer.animator.SetTrigger("Attack");
        AudioManager.instance.PlaySfx(AudioManager.Sfx.TorchSwing4);
        yield return new WaitForSeconds(1.20f);
        isAttacking = false;
    }

    public void OnTorch(InputAction.CallbackContext callbackContext)
    {
        if (isPicked == false)
        {
            StartCoroutine(WaitOn());
        }
        else
        {
            StartCoroutine(WaitOff());
        }
    }

    public void OnBandage(InputAction.CallbackContext callbackContext)
    {
        if (isPicked == false && callbackContext.phase == InputActionPhase.Started)
        {
            StartCoroutine(useBandage());
        }
        AudioManager.instance.PlaySfx(AudioManager.Sfx.BandageSound1);
    }

    private IEnumerator WaitOn()
    {
        TempPlayer.animator.Animator.SetBool("pickTorch", true);
        yield return new WaitForSeconds(1f);
        EquipItems[2].SetActive(true);
        Torch.TurnOnLight();
        isPicked = true;
        PickTorch = true;
    }
    private IEnumerator WaitOff()
    {
        Torch.TurnOffLight();
        yield return new WaitForSeconds(1.0f);
        TempPlayer.animator.Animator.SetBool("pickTorch", false);
        EquipItems[2].SetActive(false);
        isPicked = false;
        PickTorch = false;
    }
    private IEnumerator useBandage()
    {
        EquipItems[0].SetActive(true);
        TempPlayer.animator.SetTrigger("useBandage");
        yield return new WaitForSeconds(5.967f);
        EquipItems[0].SetActive(false);
    }

}
*/