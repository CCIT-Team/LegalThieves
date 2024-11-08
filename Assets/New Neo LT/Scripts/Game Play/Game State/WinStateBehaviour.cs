using Fusion.Addons.FSM;
using UnityEngine;

namespace New_Neo_LT.Scripts.Game_Play.Game_State
{
    public class WinStateBehaviour : StateBehaviour
    {
        protected override void OnEnterState()
        {
            if (!HasStateAuthority)
                return;
            PlayerRegistry.ForEach(pc => pc.Teleport(NewGameManager.Instance.winMapData.GetSpawnPosition(pc.Index)));
            PlayerRegistry.ForEach(pc =>
            {
                for (var i = 0; i < 10; i++)
                {
                    pc.Inventory.Set(i, -1);
                }
            });
            LegalThieves.RelicManager.Instance.DespawnAllRelics();
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