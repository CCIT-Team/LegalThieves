using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


}
