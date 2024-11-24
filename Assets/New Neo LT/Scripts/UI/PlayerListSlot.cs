using System;
using System.Linq;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.PlayerComponent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace New_Neo_LT.Scripts.UI
{
    public class PlayerListSlot : MonoBehaviour
    {
        [SerializeField] private RawImage   playerIcon;
        [SerializeField] private TMP_Text   playerName;
        [SerializeField] private TMP_Text   playerScore;
        [SerializeField] private GameObject playerPointType;

        private int _playerIndex = -1;

        private void Start()
        {
            playerIcon      ??= transform.GetChild(0).GetComponent<RawImage>();
            playerName      ??= transform.GetChild(2).GetComponent<TMP_Text>();
            playerScore     ??= transform.GetChild(1).GetComponent<TMP_Text>();
            playerPointType ??= transform.GetChild(3).gameObject;
        }
        
        public void SetPlayerSlot(PlayerCharacter player)
        {
            _playerIndex = player.Ref.AsIndex - 1;
            
            playerName.text = player.GetPlayerName();
            
            var pointType = player.IsScholar ? "Renown" : "Gold";
            SetPlayerPointType(pointType);
        }

        public void SetPlayerSlot(int pIndex)
        {
            _playerIndex = pIndex;
            
            if (pIndex == -1)
            {
                playerName.text = "?";
                playerScore.text = "0";
                playerPointType.transform.GetChild(0).gameObject.SetActive(false);
                playerPointType.transform.GetChild(1).gameObject.SetActive(false);
                return;
            }
            
            var player = PlayerRegistry.Where(pc => pc.Ref.AsIndex == pIndex + 1).FirstOrDefault();
            
            if(player == null) 
                return;
            
            playerName.text = player.GetPlayerName();;
            
            var pointType = player.IsScholar ? "Renown" : "Gold";
            SetPlayerPointType(pointType);
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
        
        public void SetPlayerImage(RenderTexture renderTexture)
        {
            playerIcon.texture = renderTexture;
        }
        
        public void SetPlayerImage(Texture renderTexture)
        {
            playerIcon.texture = renderTexture;
        }
        
        public void SetPlayerName(string name)
        {
            playerName.text = name;
        }
    }
}
