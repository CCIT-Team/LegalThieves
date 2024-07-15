using Fusion;
using System;

using UnityEngine;

public class H_PlayerPoint : NetworkBehaviour
{
    //GameLogic�� �־���ϴ°�

    public H_PlayerPoint[] playerScript;
    public void SetRank()
    {
        int[] RankArray = new int[4];
        for (int i = 0; i < playerScript.Length;)
        {
            RankArray[i] = playerScript[i].winPoint;
        }
        Array.Sort(RankArray);// �ε��� 0���� �ְ����� ��, 1��
        
        foreach (var rank in RankArray)
        {
            Debug.Log(rank);
            //���� ����
        }

    }

    //player�� ������ �־���Ұ�
    public enum GoldOrRenown { Gold, Renown }
    GoldOrRenown EPlayerWinPoint;
    public int[] inventory;

    //�ӽ÷� ���� ����Ʈ ����
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
    void SetRemain()// �ֿ�� �ִ� ���� ����
    {
        remainPoint = remainPoint - (goldPoint + renownPoint);
        //UI�� �ٷ� ǥ��
    }

    void GetRelic(Relic relic) // ������ �ֿ��� ��
    {
        if (remainPoint - relic.goldPoint - relic.renownPoint < 0) // ���ֿ� ��
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

    void PointOver() // �� ���ֿ� ��
    {
        Debug.Log("���̻� �ֿ� �� �����ϴ�");
    }

    void EndRound(GoldOrRenown playerWinPoint)
    {
        //Relic temp;
        //foreach (int i in inventory)
        //{
        //    // temp = Relic[i].goldPoint     �κ��丮�� �ε����� ���� ������ �����͸� ������
        //}

        switch (playerWinPoint)
        {
            case GoldOrRenown.Gold:

                winPoint = goldPoint; // ���� 
                buyPoint = renownPoint;
                break;

            case GoldOrRenown.Renown:
                winPoint = renownPoint;
                buyPoint = goldPoint;
                break;
        }


        SetRemain();   //���� ���ھ� 
    }


}