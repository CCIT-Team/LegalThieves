using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using ExitGames.Client.Photon.StructWrapping;

public class RelicDisplayer : NetworkBehaviour
{
    int currentPlayer = -1; //-1 : �̿� x 0~3 = 1P~4P
    [Networked, Capacity(120),OnChangedRender(nameof(CallChangeRelicList))]
    NetworkArray<int> DisplayedRelics { get;}
    //0~29, 30~59, 60~89, 90~119

    void CallChangeRelicList()
    {
        //UI �ﰢ ���� ���� ����
    }

    public int AddRelics(int relicID)
    {
        if (currentPlayer == -1)
            return -1;
        int playerID = currentPlayer; //���� ���� �� ���� ����
        for (int i = 0; i < 30;i++)
        {
            if(DisplayedRelics.Get(i) != -1)
            {
                DisplayedRelics.Set(i + playerID, relicID);
                return -1;
            }
        }
        return relicID; //������ �ʰ� �� ��ȯ
    }

    public List<int> SellRelics()
    {
        if (currentPlayer == -1)
            return null;
        int playerID = currentPlayer;

        List<int> relicList = new List<int>();
        for (int i = 0; i < 30; i++)
        {
            if (DisplayedRelics.Get(i + playerID) == -1)
                return relicList;
            relicList.Add(DisplayedRelics.Get(i + playerID));
            DisplayedRelics.Set(i + playerID, -1);
        }

        return relicList;
    }


    public void ContectPlayer(int playerNumber)
    {
        if (currentPlayer == playerNumber)
            currentPlayer = -1;
        else
            currentPlayer = playerNumber;
    }
}
