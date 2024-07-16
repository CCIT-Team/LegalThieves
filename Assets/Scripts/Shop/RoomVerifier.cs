using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using LegalThieves;

public class RoomVerifier : NetworkBehaviour
{
    class VerifyCount
    {
        int[] stack = new int[4];
        int count = 0;
        bool isVertified = false;
        public int Set(int playerNumber)
        {
            count++;
            if (isVertified)
            {
                return -1;
            }
            else
            {
                stack[playerNumber - 1] += 1;
                if (stack[playerNumber - 1] >= 3)
                {
                    isVertified = true;
                    return playerNumber;
                }
                else
                    return 0;
            }
        }
    }


    VerifyCount[] verifyCounts;
    [Networked] int explainCount {  get; set; }

    public void SetStacksCount(int max) //방 수만큼 규명확인 설정, 방 생성 후 호출 필요
    {
        verifyCounts = new VerifyCount[max];
        explainCount = 0;
    }
    public void SubmitStack(int roomId, int playerNumber)
    {
        //checker :  -1 = 이미 방 규명됨,0 = 규명중,1~4 = 규명한 플레이어 반환
        int checker = verifyCounts[roomId].Set(playerNumber);
        if (checker < 0)
        {
            Debug.Log("Room" + roomId + " is already vertified");
        }
        if (checker > 0)
        {
            Debug.Log("Room" + roomId + " is vertified");
            //GameLogic.ExplainRoom();
            GameManager.Instance.CallVerify();
        }
    }
}

