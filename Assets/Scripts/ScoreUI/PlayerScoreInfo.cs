
using New_Neo_LT.Scripts.Game_Play;
using System;
using TMPro;
using UnityEngine;

[Serializable]
public class PlayerScoreInfo 
{

    string _pointType;
    public string pointType { get { return _pointType; } }
    string _name;
    public string name { get { return _name; } }
    int _score;
    public int score { get { return _score; } }
    int _playerRef;
    public int playerRef { get { return _playerRef; } }

    Color _playerColor;

    public PlayerScoreInfo(string pointType, string name, int score, int playerRef, Color playerColor)
    {
        _pointType = pointType;
        _name = name;
        _score = score;
        _playerRef = playerRef;
        _playerColor = playerColor;
    }

  
    public void ScoreSet(int score)
    {
        _score = score;
    }
    
    public void SetPointType(bool IsScholar)
    {
        _pointType = IsScholar ? "Renown" : "Gold";
    }

    public void SlotRankSet(ScoreComponents com)
    {
        com.PlayerImage.color = _playerColor;
        com.PointTypeText.text = _pointType;
        com.PointAmountText.text = score.ToString() ;

    }
}
