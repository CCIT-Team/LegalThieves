using Fusion;
using LegalThieves;
using System;
using UnityEngine;
using static H_PlayerPoint;


public class H_GameManager : NetworkBehaviour
{
    public RelicCreation relicCreationScript;
    public TempPlayer[] tempPlayer;
    int bonuspoint; // 진상규명 포인트
    int[] SameRoomCheckList = new int[10]; // 10을 roomID의 최댓값으로 변경,  처음 규명하는 사람만이 점수를 얻어야 하기 때문에 전역으로 선언하여 계속 가지고 있음. 새로 맵 생성할 때 초기화 해주면 될듯합니다. 

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
        Array.Reverse(RankArray);// 인덱스 0부터 최고점수 즉, 1등
        foreach (var rank in RankArray)
        {
            Debug.Log(rank);
            //순위 로직
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
                    //roomID에 따른 용도 규명 텍스트가 필요합니다.
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
                if (invNum == 0) continue;  // 0이면 기본값이므로 아무것도 없는 칸이라 다음 칸으로

                player.goldPoint += relicCreationScript.createdRelicList[invNum].goldPoint;
                player.renownPoint += relicCreationScript.createdRelicList[invNum].renownPoint;
            }
        }
    }
}







