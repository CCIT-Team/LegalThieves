using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.Addons.FSM;
using New_Neo_LT.Scripts.UI;

namespace New_Neo_LT.Scripts.Game_Play.Game_State
{
    public class LoadingStateBehaviour : StateBehaviour
    {
        protected override void OnEnterStateRender()
        {
            UIManager.Instance.timerController.SetRound(NewGameManager.Instance.GetCurrentRound());
            UIManager.Instance.stateLoadingUI.ChangeState(true);
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
