using Fusion;
using LegalThieves;
using System;

using UnityEngine;

public class H_PlayerPoint : NetworkBehaviour
{
    //player가 가지고 있어야할것
    //public enum GoldOrRenown { Gold, Renown }
    //public GoldOrRenown EPlayerWinPoint;


    [Networked] public int goldPoint { get; set; }
    [Networked] public int renownPoint { get; set; }
    [Networked] public int buyPoint { get; set; }
    [Networked] public int remainPoint { get; set; }
    [Networked] public int maxPoint { get; set; }
    [Networked] public int winPoint { get; set; }


    //public int GoldPoint { get { return goldPoint; } }
    //public int RenownPoint { get { return renownPoint; } }
    //public int BuyPoint { get { return buyPoint; } }
    //public int RemainPoint { get { return remainPoint; } }
    //public int MaxPoint { get { return maxPoint; } }
    //public int WinPoint { get { return winPoint; } }
   
    void GetRelic(Relic relic) // 렐릭을 주웠을 때
    {
        if (remainPoint - relic.goldPoint - relic.renownPoint < 0) // 못주울 때
        { PointOver(); return; }

        goldPoint += relic.goldPoint;
        renownPoint += relic.renownPoint;
    }

    void ThrowRelic(Relic relic)
    {
        goldPoint -= relic.goldPoint;
        renownPoint -= relic.renownPoint;

    }

    void PointOver() // 더 못주울 때
    {
        Debug.Log("더이상 주울 수 없습니다");
    }

   


}