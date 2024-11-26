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
                // 이전 라운드의 포인트 초기화
                // 누적 포인트를 초기화하려면 주석을 해제하세요.
                // pc.ResetPoints();
            });
            LegalThieves.RelicManager.Instance.SpawnAllRelics();
        }

        protected override void OnEnterStateRender()
        {
#if UNITY_EDITOR
            Debug.Log("게임 플레이 상태 진입");
#endif
            UIManager.Instance.EnterPlayState();
            
            UIManager.Instance.stateLoadingUI.SetSubPos(1000000);
            
            // WinState로 게임 상태 전환 예약
            // 딜레이 시간은 게임 메니저 인스펙터로 관리
            if (HasStateAuthority)
            {
                //UIManager.Instance.stateLoadingUI.ChangeState(false)
                NewGameManager.State.Server_DelaySetState<LoadingStateBehaviour>(NewGameManager.Playtime);
            }
        }

        protected override void OnRender()
        {
            UIManager.Instance.timerController.SetTimer(NewGameManager.State.GetRemainingTime());
        }
        
        protected override void OnExitStateRender()
        {
            // UI 변경

            if (NewGameManager.Instance.RoundOver())
            {
                UIManager.Instance.stateLoadingUI.SetLoadingText("Game Finish");
                if (HasStateAuthority)
                    NewGameManager.State.Server_DelaySetState<EndStateBehaviour>(NewGameManager.Loadtime * 3);
            }
            else
            {
                UIManager.Instance.stateLoadingUI.SetLoadingText("Round Finish");
                if (HasStateAuthority)
                    NewGameManager.State.Server_DelaySetState<WinStateBehaviour>(NewGameManager.Loadtime * 3);
            }
        }
    }
}