using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class H_GameManager : MonoBehaviour
{

    public static GameObject instance;


    private void Start()
    {
        //���ӸŴ���
        if (instance == null)
        {
            instance = this.gameObject;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(instance);
        }

        //// �÷��̾� ��ũ��Ʈ ��� �ʽ�!!!
        //playerScript = new H_PlayerPoint[4];
        //GameObject[] playerArray = GameObject.FindGameObjectsWithTag("Player");

        //for(int i = 0; i< playerArray.Length; i++) { 
        //    playerScript[i] = playerArray[i].GetComponent<H_PlayerPoint>();
        //}

    }



}
