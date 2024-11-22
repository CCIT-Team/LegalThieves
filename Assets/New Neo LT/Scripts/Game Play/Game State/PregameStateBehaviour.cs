using Fusion.Addons.FSM;
using UnityEngine;

namespace New_Neo_LT.Scripts.Game_Play.Game_State
{
    public class PregameStateBehaviour : StateBehaviour
    {

        protected override void OnEnterState()
        {
            
        }

        protected override void OnEnterStateRender()
        {
#if UNITY_EDITOR
            Debug.Log("게임 시작 전 상태 진입");
#endif
            // UI 변경

            //직업선택 켜기
            //UI.UIManager.Instance.stateLoadingUI.ChangeState(false);
        }


        protected override void OnExitStateRender()
        {
            //직업선택 끄기

            if (HasStateAuthority)
            {
                NewGameManager.State.Server_DelaySetState<PlayStateBehaviour>(NewGameManager.Loadtime);
            }
        }
    }
}