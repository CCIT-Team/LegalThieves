using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class H_GameManager : MonoBehaviour
{

    public static GameObject instance;
    public enum GoldOrRenown { Gold, Renown }
    public H_Player[] playerScript; // �� �÷��̾��� ������ �̸� �Ҵ�

    private void Start()
    {
        //���ӸŴ���
        if (instance == null) {
        instance = this.gameObject;
        DontDestroyOnLoad(instance);
        }
        else { 
        Destroy(instance);
        }

        // �÷��̾� ��ũ��Ʈ ��� �ʽ�!!!
        playerScript = new H_Player[4];
        GameObject[] playerArray = GameObject.FindGameObjectsWithTag("Player");

        for(int i = 0; i< playerArray.Length; i++) { 
            playerScript[i] = playerArray[i].GetComponent<H_Player>();
        }
       
    }


    public void SetRank()
    {
      
        int[] RankArray = new int[4];
        for (int i = 0; i< playerScript.Length;)
        {
            RankArray[i] = playerScript[i].WinPoint;
        }
        Array.Sort(RankArray);// �ε��� 0���� �ְ����� ��, 1��

        foreach (var rank in RankArray)
        {
            Debug.Log(rank);
            //���� ����
        }

    }
}
