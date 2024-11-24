using System;
using System.Collections.Generic;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;

namespace New_Neo_LT.Scripts.UI
{
    public class PlayerListController : MonoBehaviour
    {
        [SerializeField] private Transform pool;
        [SerializeField] private Transform rankBordGrid;
        [SerializeField] private GameObject playerListElementPrefab;
        
        private PlayerListSlot[] _playerList = new PlayerListSlot[4];

        private void Start()
        {
            for(var i = 0; i < 4; i++)
            {
                var pl = Instantiate(playerListElementPrefab, pool).GetComponent<PlayerListSlot>();
                pl.SetPlayerSlot(-1, Color.white, 0, "Gold");
                _playerList[i] = pl;
                ReturnToPool(pl);
            }
        }

        public void PlayerJoined(PlayerCharacter pc)
        {
            for(var i = 0; i < 4; i++)
            {
                if (_playerList[i].GetPlayerIndex() != -1) 
                    continue;
                
                var pColor = pc.Runner.LocalPlayer == pc.Ref ? Color.white : NewGameManager.Instance.playerHairMaterials[pc.GetPlayerColor()].color;
                var pIndex = pc.Ref.AsIndex - 1;
                var pPointType = pc.IsScholar ? "Renown" : "Gold";
                var pScore = pc.IsScholar ? pc. GetRenownPoint : pc.GetGoldPoint;
                    
                _playerList[i].SetPlayerSlot(pIndex, pColor, pScore, pPointType);
                
                MoveToGrid(_playerList[i]);
                return;
            }
        }
        
        public void PlayerLeft(PlayerCharacter pc)
        {
            for(var i = 0; i < 4; i++)
            {
                var pIndex = _playerList[i].GetPlayerIndex();
                if (pIndex != pc.Index) 
                    continue;
                
                ReturnToPool(_playerList[i]);
                _playerList[i].SetPlayerSlot(pc.Index, Color.white, 0, "Gold");
                return;
            }
        }
        
        public void UpdatePlayerScore(int pIndex, bool isScholar, int gold, int renown)
        {
            for(var i = 0; i < 4; i++)
            {
                if (pIndex != _playerList[i].GetPlayerIndex()) 
                    continue;
                
                _playerList[i].SetPlayerScore(isScholar ? renown : gold);
                SortPlayerList();
                return;
            }

        }

        public void UpdatePlayerPointType(int pIndex, bool isScholar)
        {
            for(var i = 0; i < 4; i++)
            {
                if (pIndex != _playerList[i].GetPlayerIndex()) 
                    continue;
                
                var pointType = isScholar ? "Renown" : "Gold";
                _playerList[i].SetPlayerPointType(pointType);
                return;
            }
        }

        private void SortPlayerList()
        {
            Array.Sort(_playerList, (a, b) => b.GetPlayerScore().CompareTo(a.GetPlayerScore()));

            for (var i = 0; i < _playerList.Length; i++)
            {
                if(_playerList[i].GetPlayerIndex() == -1)
                    continue;
                _playerList[i].transform.SetSiblingIndex(i);
            }
        }
        
        private void MoveToGrid(PlayerListSlot playerListSlot)
        {
            playerListSlot.transform.SetParent(rankBordGrid);
        }
        
        
        private void ReturnToPool(PlayerListSlot playerListSlot)
        {
            playerListSlot.transform.SetParent(pool);
            playerListSlot.transform.localPosition = Vector3.zero;
        }
    }
}