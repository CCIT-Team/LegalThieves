using Fusion;
using LegalThieves;
using System;
using UnityEngine;
using static H_PlayerPoint;


public class H_GameManager : NetworkBehaviour
{
    public RelicCreation relicCreationScript;
    public TempPlayer[] tempPlayer;
    int bonuspoint; // ����Ը� ����Ʈ
    int[] SameRoomCheckList = new int[10]; // 10�� roomID�� �ִ����� ����,  ó�� �Ը��ϴ� ������� ������ ���� �ϱ� ������ �������� �����Ͽ� ��� ������ ����. ���� �� ������ �� �ʱ�ȭ ���ָ� �ɵ��մϴ�. 

    public override void Spawned()
    {
        tempPlayer = GameObject.FindObjectsOfType<TempPlayer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            EndRoundSumPoint();
            SetRank();
        }
    }
    public void SetRank()
    {
        int i = 0;
        int[] RankArray = new int[4];

        foreach (TempPlayer player in tempPlayer)
        {
            switch (player.EPlayerWinPoint)
            {
                case TempPlayer.GoldOrRenown.Gold:
                    RankArray[i++] = player.goldPoint;
                    break;

                case TempPlayer.GoldOrRenown.Renown:
                    RankArray[i++] = player.renownPoint;
                    break;
            }
        }
        Array.Sort(RankArray);
        Array.Reverse(RankArray);// �ε��� 0���� �ְ����� ��, 1��
        foreach (var rank in RankArray)
        {
            Debug.Log(rank);
            //���� ����
        }
    }

  
    void EndRoundSumPoint()
    {
        foreach (TempPlayer player in tempPlayer)
        {
            foreach (int invNum in player._inventoryItems)
            {
                SameRoomCheckList[relicCreationScript.createdRelicList[invNum].roomID]++;
                if (SameRoomCheckList[relicCreationScript.createdRelicList[invNum].roomID] == 3)
                {
                    //roomID�� ���� �뵵 �Ը� �ؽ�Ʈ�� �ʿ��մϴ�.
                    switch (player.EPlayerWinPoint)
                    {
                        case TempPlayer.GoldOrRenown.Gold:
                            player.goldPoint += bonuspoint;
                            break;

                        case TempPlayer.GoldOrRenown.Renown:
                            player.renownPoint += bonuspoint;
                            break;
                    }
                }
                if (invNum == 0) continue;  // 0�̸� �⺻���̹Ƿ� �ƹ��͵� ���� ĭ�̶� ���� ĭ����

                player.goldPoint += relicCreationScript.createdRelicList[invNum].goldPoint;
                player.renownPoint += relicCreationScript.createdRelicList[invNum].renownPoint;
            }
        }
    }
}







