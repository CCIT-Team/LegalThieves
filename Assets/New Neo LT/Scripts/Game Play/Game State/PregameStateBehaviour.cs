using System.Linq;
using Fusion.Addons.FSM;
using New_Neo_LT.Scripts.UI;
using UnityEngine;

namespace New_Neo_LT.Scripts.Game_Play.Game_State
{
    public class PregameStateBehaviour : StateBehaviour
    {

        protected override void OnEnterState()
        {
            UIManager.Instance.stateLoadingUI.SetYPos();
        }

        protected override void OnEnterStateRender()
        {
#if UNITY_EDITOR
            Debug.Log("게임 시작 전 상태 진입");
#endif
            UIManager.Instance.EnterPreGameState();
            
            // UI 변경
            // UIManager.Instance.stateLoadingUI.SetSubPos(0);
            
            //직업선택 켜기
            //UI.UIManager.Instance.stateLoadingUI.ChangeState(false);
            
            
            if(HasStateAuthority)
            {
                NewGameManager.State.Server_DelaySetState<PlayStateBehaviour>(NewGameManager.Picktime);
            }
        }


        protected override void OnExitStateRender()
        {
            
            UIManager.Instance.stateLoadingUI.SetLoadingText( "Round " + NewGameManager.Instance.GetCurrentRound());

            if (HasStateAuthority)
            {
                var players = PlayerRegistry.Where(pc => pc.GetJobIndex() == (int)Job.Null);
                var availableJobs = NewGameManager.Instance.GetAvailableJobIndices().ToArray();
                // 직업이 없는 플레이어들에게 랜덤으로 겹치지 않는 직업을 부여
                foreach (var player in players)
                {
                    var jobIndex = availableJobs[Random.Range(0, availableJobs.Length - 1)];
                    player.ChangeJob((Job)jobIndex);
                    availableJobs = availableJobs.Where(i => i != jobIndex).ToArray();
                }
                
                NewGameManager.State.Server_DelaySetState<PlayStateBehaviour>(NewGameManager.Loadtime * 3);
            }
        }
    }
}