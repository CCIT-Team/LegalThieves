﻿using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
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
        [SerializeField] private Transform[] rankUIList;
        
        private PlayerListSlot[] _playerList = new PlayerListSlot[4];

        // private Texture2D[] _playerImages = new Texture2D[4];
        
        private void Start()
        {
            for(var i = 0; i < 4; i++)
            {
                var pl = Instantiate(playerListElementPrefab, pool).GetComponent<PlayerListSlot>();
                pl.SetPlayerSlot(-1);
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
                
                // var pColor = pc.Runner.LocalPlayer == pc.Ref ? Color.white : NewGameManager.Instance.playerHairMaterials[pc.GetPlayerColor()].color;
                // var pIndex = pc.Ref.AsIndex - 1;
                // var pPointType = pc.IsScholar ? "Renown" : "Gold";
                // var pScore = pc.IsScholar ? pc. GetRenownPoint : pc.GetGoldPoint;
                    
                _playerList[i].SetPlayerSlot(pc);
                
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
                _playerList[i].SetPlayerSlot(-1);
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

                var player = PlayerRegistry.Where(pc => pc.Ref.AsIndex == pIndex + 1).FirstOrDefault();
                
                if(player == null)
                    return;
                
                var jobIndex = player.GetJobIndex();
                
                if(jobIndex == -1)
                    return;

                var renderTexture = UIManager.Instance.resultUIController.GetCamera(jobIndex).targetTexture;
                
                var texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

                RenderTexture.active = renderTexture;
                
                texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                texture.Apply();
                
                RenderTexture.active = null;

                // _playerImages ??= new Texture2D[4];
                //
                // _playerImages[i] = texture;
                
                _playerList[i].SetPlayerImage(texture);
                
                return;
            }
        }

        public void InitPlayersName()
        {
            foreach (var slot in _playerList)
            {
                var player = PlayerRegistry.Where(p => p.Index == slot.GetPlayerIndex()).FirstOrDefault();
                
                if(player == null)
                    continue;
                
                slot.SetPlayerName(player.GetPlayerName());
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

            for (var i = 0; i < _playerList.Length; i++)
            {
                int score = _playerList[i].GetPlayerScore();
                if (!rankUIList[i].gameObject.activeInHierarchy && score != 0)
                {
                    rankUIList[i].gameObject.SetActive(true);
                }
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

        public Transform[] GetRankUIList()
        {
            return rankUIList;
        }
    }
}