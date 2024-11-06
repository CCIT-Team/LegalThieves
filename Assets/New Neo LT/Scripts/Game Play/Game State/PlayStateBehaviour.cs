using Fusion.Addons.FSM;
using LegalThieves;
using UnityEngine;

namespace New_Neo_LT.Scripts.Game_Play.Game_State
{
    public class PlayStateBehaviour : StateBehaviour
    {
        
        
        
        protected override void OnEnterState()
        {
            if(HasStateAuthority)
                PlayerRegistry.ForEach(pc => pc.Teleport(NewGameManager.Instance.playMapData.GetSpawnPosition(pc.Index)));
                
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
            
            UIManager.Singleton.SetTimer((int)NewGameManager.State.GetRemainingTime());
        }
        
        protected override void OnExitStateRender()
        {
            // UI 변경
            UIManager.Singleton.SetTimer("Waiting");
        }
    }
}