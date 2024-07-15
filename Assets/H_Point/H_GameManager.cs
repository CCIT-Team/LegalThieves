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
        //게임매니저
        if (instance == null)
        {
            instance = this.gameObject;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(instance);
        }

        //// 플레이어 스크립트 담는 초식!!!
        //playerScript = new H_PlayerPoint[4];
        //GameObject[] playerArray = GameObject.FindGameObjectsWithTag("Player");

        //for(int i = 0; i< playerArray.Length; i++) { 
        //    playerScript[i] = playerArray[i].GetComponent<H_PlayerPoint>();
        //}

    }



}
