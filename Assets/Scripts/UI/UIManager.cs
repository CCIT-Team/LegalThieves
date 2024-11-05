using System;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegalThieves
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Singleton
        {
            get => _singleton;
            private set
            {
                if      (value == null)        _singleton = null;
                else if (_singleton == null)   _singleton = value;
                else if (_singleton != value)
                {
                    Destroy(value);
                    Debug.LogError($"{nameof(UIManager)}는(은) 단 한번만 인스턴싱되어야 합니다!");
                }
            }
        }

        private static UIManager _singleton;

        [SerializeField] private Image[]            inventorySlotImages;
        [SerializeField] private GameObject[]       selectToggles;
        [Space]
        [SerializeField] private RectTransform compass;
        [SerializeField] private TMP_Text timer;
        [Space]
        [SerializeField] private TextMeshProUGUI    gameStateText;
        [SerializeField] private TextMeshProUGUI    instructionText;
        [SerializeField] private Image              sprintActive;
        [SerializeField] private LeaderboardItem[]  leaderboardItems;

        public TempPlayer  localTempPlayer;
        public int         currentSlotIndex;

        private void Awake()
        {
            Singleton = this;
            Init();
        }

        private void Update()
        {
            if (localTempPlayer == null)
                return;
            compass.eulerAngles = new Vector3(0, 0, localTempPlayer.transform.eulerAngles.y);
        }

        private void OnDestroy()
        {
            if (Singleton == this)
                Singleton = null;
        }

        private void Init()
        {
            selectToggles[currentSlotIndex].SetActive(true);
        }

        public void DidSetReady()
        {
            instructionText.text = "Waiting for other players to be ready...";
        }

        public void SetWaitUI(EGameState newState, TempPlayer winner)
        {
            if (newState == EGameState.Waiting)
            {
                if (winner == null)
                {
                    gameStateText.text   = "Waiting to Start";
                    instructionText.text = "Press R when you're ready to begin!";
                }
                else
                {
                    gameStateText.text   = $"{winner.Name} Wins!";
                    instructionText.text = "Press R when you're ready to begin!";
                }
            }

            gameStateText.enabled   = newState == EGameState.Waiting;
            instructionText.enabled = newState == EGameState.Waiting;
        }

        public void SetSlotImage(bool isActive, Sprite sprite = null)
        {
            inventorySlotImages[currentSlotIndex].sprite = sprite;
            inventorySlotImages[currentSlotIndex].enabled = isActive;
        }

        public void MoveCurrentSlot(bool isLeft)
        {
            switch (isLeft)
            {
                case true when currentSlotIndex > 0:
                    selectToggles[currentSlotIndex].SetActive(false);
                    currentSlotIndex -= 1;
                    selectToggles[currentSlotIndex].SetActive(true);
                    break;
                case false when currentSlotIndex < 9:
                    selectToggles[currentSlotIndex].SetActive(false);
                    currentSlotIndex += 1;
                    selectToggles[currentSlotIndex].SetActive(true);
                    break;
            }
        }

        public void UpdateLeaderboard(KeyValuePair<PlayerRef, TempPlayer>[] players)
        {
            for (var i = 0; i < leaderboardItems.Length; i++)
            {
                var item = leaderboardItems[i];

                if (i < players.Length)
                {
                    if (players[i].Key == localTempPlayer.Runner.LocalPlayer)
                    {

                    }
                    item.nameText.text = players[i].Value.Name;
                    item.heightText.text = $"{players[i].Value.Score}m";
                }
                else
                {
                    item.nameText.text = "";
                    item.heightText.text = "";
                }
            }
        }

        public void SetTimer(string text)
        {
            timer.text = text;
        }
        
        public void SetTimer(int time)
        {
            timer.text = (time / 60 < 10 ? "0" + time / 60 : time / 60) + " : " + (time % 60 < 10 ? "0" + time % 60 : time % 60);
        }

        public void ResetHUD()
        {
            ResetSlotToggle();
            ResetSlotImages();
            ResetTimerText();
        }

        private void ResetSlotImages()
        {
            foreach (var img in inventorySlotImages)
            {
                img.sprite = null;
                img.enabled = false;
            }
        }

        private void ResetSlotToggle()
        {
            foreach (var obj in selectToggles)
            {
                obj.SetActive(false);
            }
            currentSlotIndex = 0;
            selectToggles[0].SetActive(true);
        }

        private void ResetTimerText()
        {
            SetTimer("Waiting");
        }

        [Serializable]
        private struct LeaderboardItem
        {
            public TextMeshProUGUI nameText;
            public TextMeshProUGUI heightText;
        }
    }
}