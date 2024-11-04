using Fusion.Addons.FSM;
using UnityEngine;

namespace New_Neo_LT.Scripts.Game_Play.Game_State
{
    public class PlayStateBehaviour : StateBehaviour
    {
        
        
        
        protected override void OnEnterState()
        {
            PlayerRegistry.ForEach(pc => pc.Teleport(NewGameManager.Instance.playMapData.GetSpawnPosition(pc.Index)));
            NewGameManager.State.Server_DelaySetState<WinStateBehaviour>(NewGameManager.Playtime);
        }

        protected override void OnEnterStateRender()
        {
#if UNITY_EDITOR
            Debug.Log("게임 플레이 상태 진입");
#endif
            // UI 변경
        }
    }
}