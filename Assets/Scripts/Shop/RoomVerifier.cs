using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomVerifier : MonoBehaviour
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


    List<VerifyCount> verifyCounts = new List<VerifyCount>();

    public void SetStacksCount(int max)
    {
        for (int i = 0; i < max; i++)
            verifyCounts.Add(new VerifyCount());
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
            GameManager.Instance.CallVerify();
        }
    }
}

