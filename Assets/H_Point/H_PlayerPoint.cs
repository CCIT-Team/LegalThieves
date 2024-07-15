using Fusion;
using System;

using UnityEngine;

public class H_PlayerPoint : NetworkBehaviour
{
    //GameLogic에 넣어야하는것

    public H_PlayerPoint[] playerScript;
    public void SetRank()
    {
        int[] RankArray = new int[4];
        for (int i = 0; i < playerScript.Length;)
        {
            RankArray[i] = playerScript[i].winPoint;
        }
        Array.Sort(RankArray);// 인덱스 0부터 최고점수 즉, 1등
        
        foreach (var rank in RankArray)
        {
            Debug.Log(rank);
            //순위 로직
        }

    }

    //player가 가지고 있어야할것
    public enum GoldOrRenown { Gold, Renown }
    GoldOrRenown EPlayerWinPoint;
    public int[] inventory;

    //임시로 유물 리스트 구현
    private Relic[] relics;


    private void Start()
    {
        inventory = new int[10];
        remainPoint = maxPoint;
    }

    Relic relic;


    [Networked] private int goldPoint { get; set; }
    [Networked] private int renownPoint { get; set; }
    [Networked] private int buyPoint { get; set; }
    [Networked] private int remainPoint { get; set; }
    [Networked] private int maxPoint { get; set; }
    [Networked] private int winPoint { get; set; }

    public int GoldPoint { get { return goldPoint; } }
    public int RenownPoint { get { return renownPoint; } }
    public int BuyPoint { get { return buyPoint; } }
    public int RemainPoint { get { return remainPoint; } }
    public int MaxPoint { get { return maxPoint; } }
    public int WinPoint { get { return winPoint; } }
    void SetRemain()// 주울수 있는 양을 설정
    {
        remainPoint = remainPoint - (goldPoint + renownPoint);
        //UI에 바로 표현
    }

    void GetRelic(Relic relic) // 렐릭을 주웠을 때
    {
        if (remainPoint - relic.goldPoint - relic.renownPoint < 0) // 못주울 때
        { PointOver(); return; }

        goldPoint += relic.goldPoint;
        renownPoint += relic.renownPoint;

        SetRemain();
    }

    void ThrowRelic(Relic relic)
    {
        goldPoint -= relic.goldPoint;
        renownPoint -= relic.renownPoint;

        SetRemain();
    }

    void PointOver() // 더 못주울 때
    {
        Debug.Log("더이상 주울 수 없습니다");
    }

    void EndRound(GoldOrRenown playerWinPoint)
    {
        //Relic temp;
        //foreach (int i in inventory)
        //{
        //    // temp = Relic[i].goldPoint     인벤토리의 인덱스에 따라 렐릭의 데이터를 가져옴
        //}

        switch (playerWinPoint)
        {
            case GoldOrRenown.Gold:

                winPoint = goldPoint; // 기존 
                buyPoint = renownPoint;
                break;

            case GoldOrRenown.Renown:
                winPoint = renownPoint;
                buyPoint = goldPoint;
                break;
        }


        SetRemain();   //남은 스코어 
    }


}