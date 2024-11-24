using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace New_Neo_LT.Scripts.UI
{
    public class PlayerListSlot : MonoBehaviour
    {
        [SerializeField] private Image    playerIcon;
        [SerializeField] private TMP_Text playerScore;
        [SerializeField] private GameObject playerPointType;

        private int _playerIndex = -1;

        private void Start()
        {
            playerIcon     ??= transform.GetChild(0).GetComponent<Image>();
            playerScore     ??= transform.GetChild(1).GetComponent<TMP_Text>();
            playerPointType ??= transform.GetChild(2).gameObject;
        }

        public void SetPlayerSlot(int pIndex, Color color , int score, string pointType)
        {
            _playerIndex = pIndex;
            playerIcon.color = color;
            playerScore.text = score.ToString();
            if(pointType == "Gold")
            {
                playerPointType.transform.GetChild(0).gameObject.SetActive(true);
                playerPointType.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                playerPointType.transform.GetChild(0).gameObject.SetActive(false);
                playerPointType.transform.GetChild(1).gameObject.SetActive(true);
            }
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
            if (pointType == "Gold")
            {
                playerPointType.transform.GetChild(0).gameObject.SetActive(true);
                playerPointType.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                playerPointType.transform.GetChild(0).gameObject.SetActive(false);
                playerPointType.transform.GetChild(1).gameObject.SetActive(true);
            }
        }

        public int GetPlayerScore()
        {
            return playerScore.text == "" ? 0 : Convert.ToInt32(playerScore.text);
        }
    }
}
