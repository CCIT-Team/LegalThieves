using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;
using UnityEngine.UIElements;

namespace New_Neo_LT.Scripts.Game_Play.Game_State
{
    public class WinStateBehaviour : StateBehaviour
    {
        [SerializeField] private NetworkPrefabRef barricadeTransform;
        [SerializeField] private Transform winStateTransform;

        private NetworkObject spawnedBarricade;

        protected override void OnEnterState()
        {
            if (!HasStateAuthority)
                return;
            spawnedBarricade = Runner.Spawn(barricadeTransform);
            spawnedBarricade.transform.SetParent(winStateTransform);
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
            if (HasStateAuthority)
            {
                //UIManager.Instance.stateLoadingUI.ChangeState(false)
                NewGameManager.State.Server_DelaySetState<LoadingStateBehaviour>(NewGameManager.Resttime);
            }
            // UI 변경
        }

        protected override void OnRender()
        {
            UI.UIManager.Instance.timerController.SetTimer(NewGameManager.State.GetRemainingTime());
        }

        protected override void OnExitStateRender()
        {
            UI.UIManager.Instance.stateLoadingUI.SetLoadingText("Round " + NewGameManager.Instance.GetCurrentRound());
            if (HasStateAuthority)
            {
                NewGameManager.State.Server_DelaySetState<PlayStateBehaviour>(NewGameManager.Loadtime * 3);
            }
            Runner.Despawn(spawnedBarricade);
        }
    }
}