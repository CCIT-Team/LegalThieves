using Fusion;
using LegalThieves;
using System;

using UnityEngine;

public class H_PlayerPoint : NetworkBehaviour
{
    //player�� ������ �־���Ұ�
    public enum GoldOrRenown { Gold, Renown }
    public GoldOrRenown EPlayerWinPoint;

    

    //�ӽ÷� ���� ����Ʈ ����

    public override void Spawned()
    {
        //�κ��丮 ����
        //inventory = new int[10]; �׽�Ʈ�� ���� �ּ�ó��
      
        remainPoint = maxPoint;
    }

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

   


}