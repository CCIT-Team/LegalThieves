using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Fusion;
using New_Neo_LT.Scripts.Game_Play;

public class ScoreRankUI : MonoBehaviour {

    [SerializeField]
    private GameObject PlayerScoreBase;

    [SerializeField]
    private List<ScoreComponents>  ScoreComponentsList = new List<ScoreComponents>();
    [SerializeField]
    private List<PlayerScoreInfo> ScoreInfo = new List<PlayerScoreInfo>();

    Color[] PlayerColors = { Color.red, Color.green, Color.blue, Color.yellow };

    int PlayerIndex = 0;
    public void JoinedPlayer(PlayerRef player)
    {
        var playerId = player.PlayerId;
        var job = PlayerRegistry.GetPlayer(player).IsScholar;
       
        GameObject slot =Instantiate(PlayerScoreBase,Vector3.zero,Quaternion.identity,transform.GetChild(0));
        ScoreComponentsList.Add(slot.GetComponent<ScoreComponents>());
        
        //playerRef 값 이용해서 속성들 채워넣을 예정
        ScoreInfo.Add(new PlayerScoreInfo(job ? "Gold" : "Renown", "name", 1000, playerId, PlayerColors[PlayerIndex]));
        Debug.Log(ScoreInfo[PlayerIndex++].playerRef);
        RankSet();
    }

    public void LeftPlayer(PlayerRef player)
    {
        var pId = player.PlayerId;
        ScoreComponentsList.RemoveAt(ScoreComponentsList.Count-1);
        ScoreInfo.Remove(ScoreInfo.FirstOrDefault(Info => Info.playerRef == pId));
        PlayerIndex--;
        RankSet();
    }
  
  

    public void PlayerScoreSet(PlayerRef player,int score)
    {
        var playerId = player.PlayerId;
        for (int i = 0; i < ScoreInfo.Count; i++)
        {
            if (ScoreInfo[i].playerRef == playerId)
            {
                ScoreInfo[i].ScoreSet(score);
                RankSet();
                return;
            }
        }
     
        Debug.Log("플레이어 데이터를 찾을 수 없음");
    }
    
    public void SetPlayerJob(int player,bool isScholar)
    {
        for (var i = 0; i < ScoreInfo.Count; i++)
        {
            if (ScoreInfo[i].playerRef == player)
            {
                ScoreInfo[i].SetPointType(isScholar);
                RankSet();
                return;
            }
        }
     
        Debug.Log("플레이어 데이터를 찾을 수 없음");
    }

    public void RankSet()
    {
        ScoreInfo = ScoreInfo.OrderByDescending(player => player.score).ToList();

        for (int i = 0;i < ScoreComponentsList.Count; i++)
        {
            ScoreInfo[i].SlotRankSet(ScoreComponentsList[i]);
        }
    }
}
