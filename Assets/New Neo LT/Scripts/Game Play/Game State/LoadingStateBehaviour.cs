using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fusion.Addons.FSM;
using New_Neo_LT.Scripts.UI;

namespace New_Neo_LT.Scripts.Game_Play.Game_State
{
    public class LoadingStateBehaviour : StateBehaviour
    {
        protected override void OnEnterState()
        {
            if (HasStateAuthority)
            {
                var players = PlayerRegistry.Where(pc => pc.GetJobIndex() == (int)Job.Null);
                var availableJobs = NewGameManager.Instance.GetAvailableJobIndices().ToArray();
                // 직업이 없는 플레이어들에게 랜덤으로 겹치지 않는 직업을 부여
                foreach (var player in players)
                {
                    var jobIndex = availableJobs[UnityEngine.Random.Range(0, availableJobs.Length)];
                    player.ChangeJob((Job)jobIndex);
                    availableJobs = availableJobs.Where(i => i != jobIndex).ToArray();
                }
            }
                
        }
        
        protected override void OnEnterStateRender()
        {
            UIManager.Instance.timerController.SetRound(NewGameManager.Instance.GetCurrentRound());
            UIManager.Instance.stateLoadingUI.ChangeState(ELoadType.Toggle);
        }

        protected override void OnRender()
        {

        }

        protected override void OnExitState()
        {

        }

        protected override void OnExitStateRender()
        {

        }
    }
}
