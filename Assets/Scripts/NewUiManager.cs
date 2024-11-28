using Fusion;
using New_Neo_LT.Scripts.Game_Play;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LegalThieves;
using New_Neo_LT.Scripts.Elements;

public class NewUiManager : MonoBehaviour
{
    public static NewUiManager instance;
    public static NewUiManager Instance {  get { return instance; } }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }
    [SerializeField] private GameObject[] Slots;
    int preindex ;

    [SerializeField] private GameObject progressBar;
    [SerializeField] private InteractionProgressUITemp InteractionProgressUITemp;

    [SerializeField] private SellingUITest shopUI;

    [SerializeField] private GameObject pointUI;



    private void Start()
    {
        Slots[0].transform.GetChild(1).gameObject.SetActive(true);
        preindex = 0;
    }
    public void SelectTogle(int index)
    {
        Slots[preindex].transform.GetChild(1).gameObject.SetActive(false);
        Slots[index].transform.GetChild(1).gameObject.SetActive(true);
        preindex = index;
    }

    //������ ������ ��������Ʈ�� �̹��� �ִ� �޼���
    public void SetRelicSprite(int index,int relicId, bool isFull) {
        
        //relic �˻��ؼ� ��������
        
        Image tempImage = Slots[index].transform.GetChild(2).GetComponent<Image>();
      //  tempImage.sprite = icon;    <--���⼭ ���� sprite �ٲ㲸����
        tempImage.enabled = isFull;
    }


    public void OnProgressbar()
    {
        progressBar.SetActive(true);
        InteractionProgressUITemp.StartProgress(10f, "���� �Ѽ� ��", "���� �Ѽ� �Ϸ�!");

    }
    public void OffProgressbar()
    {
        progressBar.SetActive(false);
         

    }

    public void DisplayRelicP(int relicIndex)
    {
        if (relicIndex == -1)
            pointUI.SetActive(false);
        else
        {
            pointUI.SetActive(true);
            pointUI.GetComponent<RelicPriceUI>().SetUIPoint(relicIndex);
        }
    }

    public void ToggleShop(int[] inventory, Shop shop = null)
    {
        if(ToggleCusorSetting())
        {
            shopUI.gameObject.SetActive(true);
            shopUI.SetInventoryGrid(inventory);
        }
        else
        {
            shopUI.ClearUI();
            shopUI.gameObject.SetActive(false);
        }
        
    }

    public bool ToggleCusorSetting()
    {
        Cursor.visible = !Cursor.visible;
        if (Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        return Cursor.visible;
    }
}
