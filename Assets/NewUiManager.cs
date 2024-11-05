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

    //아이템 들어오면 스프라이트에 이미지 넣는 메서드
    public void SetRelicSprite(int index,Sprite icon,bool isFull) {
        Image tempImage = Slots[index].transform.GetChild(2).GetComponent<Image>();
        tempImage.sprite = icon;
        tempImage.enabled = isFull;
    }
}
