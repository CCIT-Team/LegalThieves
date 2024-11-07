using New_Neo_LT.Scripts.Game_Play;
using UnityEngine;

namespace New_Neo_LT.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        public CompassRotate           compassRotate;
        public InventorySlotController inventorySlotController;
        public TimerController         timerController;
        public PlayerListController    playerListController;

        public static UIManager Instance;
        
        private Transform _localPlayerTransform;

        private void Start()
        {
            if(Instance == null)
                Instance = this;
            else if(Instance != this)
                Destroy(gameObject);
        }

        private void LateUpdate()
        {
            // 서버 접속 확인
            if (!_localPlayerTransform)
                return;
            
            compassRotate.RotateCompass(_localPlayerTransform);
            
        }

        // 게임 접속 시 UI 초기화
        public void InitializeInGameUI()
        {
            
        }
        
        public void SetLocalPlayerTransform(Transform localPlayerTransform)
        {
            _localPlayerTransform = localPlayerTransform;
        }
    }
}