using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace New_Neo_LT.Scripts.UI
{
    public class PlayerListSlot : MonoBehaviour
    {
        [SerializeField] private Image    playerColor;
        [SerializeField] private TMP_Text playerScore;
        [SerializeField] private TMP_Text playerPointType;

        private int _playerIndex = -1;

        private void Start()
        {
            playerColor     ??= transform.GetChild(0).GetComponent<Image>();
            playerScore     ??= transform.GetChild(1).GetComponent<TMP_Text>();
            playerPointType ??= transform.GetChild(2).GetComponent<TMP_Text>();
        }

        public void SetPlayerSlot(int pIndex, Color color , int score, string pointType)
        {
            _playerIndex = pIndex;
            playerColor.color = color;
            playerScore.text = score.ToString();
            playerPointType.text = pointType;
        }
        
        public void SetPlayerIndex(int pIndex)
        {
            _playerIndex = pIndex;
        }
        
        public int GetPlayerIndex()
        {
            return _playerIndex;
        }
        
        public void SetPlayerScore(int score)
        {
            playerScore.text = score.ToString();
        }
        
        public void SetPlayerPointType(string pointType)
        {
            playerPointType.text = pointType;
        }

        public int GetPlayerScore()
        {
            return playerScore.text == "" ? 0 : Convert.ToInt32(playerScore.text);
        }
    }
}
