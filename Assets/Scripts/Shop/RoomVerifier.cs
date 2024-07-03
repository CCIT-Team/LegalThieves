using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomVerifier : MonoBehaviour
{
    class VerifyCount
    {
        int[] stack = new int[4];
        bool isVertified = false;
        public int Set(int playerNumber)
        {
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
        int count = verifyCounts[roomId].Set(playerNumber);
        if (count < 0)
        {
            Debug.Log("Room" + roomId + " is already vertified");
        }
        if (n > 0)
        {
            Debug.Log("Room" + roomId + " is vertified by " + n);
        }
    }
}
}
