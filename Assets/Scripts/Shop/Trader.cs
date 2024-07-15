using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;
using System;

public class Trader : NetworkBehaviour
{
    int currentPlayer = -1; //-1 : ÀÌ¿כ x
    [Networked, Capacity(30), OnChangedRender(nameof(CallChangeRelicList))]
    NetworkLinkedList<int> relics => default ;

    void CallChangeRelicList()
    {
        
    }

    public void ContectPlayer(int playerNumber)
    {
        currentPlayer = playerNumber;
    }

    public void DisconnectPlayer()
    {
        currentPlayer = -1;
    }

    #region Relic
    void TradePoint(DummyRelic relic)
    {
        GetComponent<RoomVerifier>().SubmitStack(relic.roomID, relic.goldPoint);
        GameManager.Instance.GetScore(0, relic.goldPoint, relic.renownPoint);
    }
    #endregion

    #region Skill
    void BuySkill()
    {
        if(GameManager.Instance.UseScore(0, 1000, 0))
        {
            Debug.Log("BuySkill");
        }
        else
        {
            Debug.Log("Not Enought Point");
        }
    }
    #endregion

    #region Tools
    void BuyItem()
    {
        if (GameManager.Instance.UseScore(0, 1000, 0))
        {
            Debug.Log("BuyItem");
        }
        else
        {
            Debug.Log("Not Enought Point");
        }
    }
    #endregion
}
