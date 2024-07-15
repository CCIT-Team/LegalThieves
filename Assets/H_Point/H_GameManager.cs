using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class H_GameManager : MonoBehaviour
{

    public static GameObject instance;
    public enum GoldOrRenown { Gold, Renown }
    public H_PlayerPoint[] playerScript; // 각 플레이어의 정보를 미리 할당

    private void Start()
    {
        //게임매니저
        if (instance == null) {
        instance = this.gameObject;
        DontDestroyOnLoad(instance);
        }
        else { 
        Destroy(instance);
        }

        // 플레이어 스크립트 담는 초식!!!
        playerScript = new H_PlayerPoint[4];
        GameObject[] playerArray = GameObject.FindGameObjectsWithTag("Player");

        for(int i = 0; i< playerArray.Length; i++) { 
            playerScript[i] = playerArray[i].GetComponent<H_PlayerPoint>();
        }
       
    }


    public void SetRank()
    {
      
        int[] RankArray = new int[4];
        for (int i = 0; i< playerScript.Length;)
        {
            RankArray[i] = playerScript[i].WinPoint;
        }
        Array.Sort(RankArray);// 인덱스 0부터 최고점수 즉, 1등

        foreach (var rank in RankArray)
        {
            Debug.Log(rank);
            //순위 로직
        }

    }
}
