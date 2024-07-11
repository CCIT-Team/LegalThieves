using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trader : MonoBehaviour
{
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
