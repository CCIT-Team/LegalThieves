using Fusion;
using LegalThieves;
using System;

using UnityEngine;
using static H_PlayerPoint;


public class H_GameManager : NetworkBehaviour
{


    //GameLogic에 넣어야하는것
    //public RelicCreation relicCreationScript;
    //public TempPlayer[] tempPlayer;

    //public override void Spawned()
    //{
    //    tempPlayer = GameObject.FindObjectsOfType<TempPlayer>();
    //}

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.V)){
    //        EndRoundSumPoint();
    //        SetRank();
    //    }
    //}
    //public void SetRank()
    //{
    //    int i = 0;
    //    int[] RankArray = new int[4];

    //    foreach (TempPlayer player in playerScript)
    //    {
            
    //        switch (player.EPlayerWinPoint)
    //        {
    //            case GoldOrRenown.Gold:
    //                RankArray[i++] = player.goldPoint;
    //                break;

    //            case GoldOrRenown.Renown:
    //                RankArray[i++] = player.renownPoint;
    //                break;
    //        }
          
    //    }
    //    Array.Sort(RankArray);
    //    Array.Reverse(RankArray);// 인덱스 0부터 최고점수 즉, 1등
    //    foreach (var rank in RankArray)
    //    {
    //        Debug.Log(rank);
    //        //순위 로직
    //    }
    //}

    //void EndRoundSumPoint()
    //{
    //    foreach (TempPlayer player in playerScript)
    //    {
    //        foreach (int invNum in player.inventory)
    //        {
               
    //            if (invNum == 0) continue;  // 0이면 기본값이므로 아무것도 없는 칸이라 다음 칸으로
    //            player.goldPoint += relicCreationScript.createdRelicList[invNum].goldPoint;
    //            player.renownPoint += relicCreationScript.createdRelicList[invNum].renownPoint;
               
    //        }

    //    }
    //}

}





