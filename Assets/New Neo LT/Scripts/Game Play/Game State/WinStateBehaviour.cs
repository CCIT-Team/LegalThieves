using Fusion.Addons.FSM;
using UnityEngine;

namespace New_Neo_LT.Scripts.Game_Play.Game_State
{
    public class WinStateBehaviour : StateBehaviour
    {
        protected override void OnEnterState()
        {
            PlayerRegistry.ForEach(pc => pc.Teleport(NewGameManager.Instance.winMapData.GetSpawnPosition(pc.Index)));
        }

        protected override void OnEnterStateRender()
        {
#if UNITY_EDITOR
            Debug.Log("게임 승리 상태 진입");
#endif
            // UI 변경
        }
    }
}