using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.Addons.FSM;


namespace New_Neo_LT.Scripts.Game_Play.Game_State
{
    public class EndStateBehaviour : StateBehaviour
    {
        protected override void OnEnterState()
        {
            Debug.Log("EndGame");
        }

        protected override void OnEnterStateRender()
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

