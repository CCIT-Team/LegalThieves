using Fusion.Addons.FSM;
using LegalThieves;
using UnityEngine;
using UIManager = New_Neo_LT.Scripts.UI.UIManager;

namespace New_Neo_LT.Scripts.Game_Play.Game_State
{
    public class PlayStateBehaviour : StateBehaviour
    {
        protected override void OnEnterState()
        {
            if(!HasStateAuthority)
                return; 
            
            PlayerRegistry.ForEach(pc =>
            {
                pc.Teleport(NewGameManager.Instance.playMapData.GetSpawnPosition(pc.Index));
                pc.ResetPoints();
            });
            LegalThieves.RelicManager.Instance.SpawnAllRelics();
        }

        protected override void OnEnterStateRender()
        {
#if UNITY_EDITOR
            Debug.Log("게임 플레이 상태 진입");
#endif
            // WinState로 게임 상태 전환 예약
            // 딜레이 시간은 게임 메니저 인스펙터로 관리
            if (HasStateAuthority)
            {
                NewGameManager.State.Server_DelaySetState<WinStateBehaviour>(NewGameManager.Playtime);
            }
        }

        protected override void OnRender()
        {
            UIManager.Instance.timerController.SetTimer(NewGameManager.State.GetRemainingTime());
        }
        
        protected override void OnExitStateRender()
        {
            // UI 변경
            UIManager.Instance.timerController.SetTimer("Waiting");
        }
    }
}