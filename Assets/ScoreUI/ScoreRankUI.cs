using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Fusion;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.PlayerComponent;

public class ScoreRankUI : MonoBehaviour {

    [SerializeField]
    private GameObject PlayerScoreBase;

    [SerializeField]
    private List<ScoreComponents>  ScoreComponentsList = new List<ScoreComponents>();
    [SerializeField]
    private List<PlayerScoreInfo> ScoreInfo = new List<PlayerScoreInfo>();

    Color[] PlayerColors = { Color.red, Color.green, Color.blue, Color.yellow };

    int PlayerIndex = 0;
    public void JoinedPlayer(int player)
    {
        
        
        GameObject slot =Instantiate(PlayerScoreBase,Vector3.zero,Quaternion.identity,transform.GetChild(0));
        ScoreComponentsList.Add(slot.GetComponent<ScoreComponents>());
        
        //playerRef 값 이용해서 속성들 채워넣을 예정
        ScoreInfo.Add(new PlayerScoreInfo("Gold", "Player", 1000, player, PlayerColors[PlayerIndex]));
        RankSet();
    }

    public void LeftPlayer(int player)
    {
        ScoreComponentsList.RemoveAt(ScoreComponentsList.Count-1);
        ScoreInfo.Remove(ScoreInfo.FirstOrDefault(Info => Info.playerRef == player));
        PlayerIndex--;
        RankSet();
    }
  
  

    public void PlayerScoreSet(int player,int score)
    {
        for (int i = 0; i < ScoreInfo.Count; i++)
        {
            if (ScoreInfo[i].playerRef == player)
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

    public void SetJoinedPlayer(NetworkDictionary<PlayerRef,PlayerCharacter> players)
    {
        foreach (var player in players)
        {
            var pc = player.Value;
            //JoinedPlayer();
        }
    }
}
