using UnityEngine;
using Fusion.Addons.FSM;
using New_Neo_LT.Scripts.UI;


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
            //결과화면 켜기
            UI.UIManager.Instance.SetActiveUI(UIType.ResultUIController, true);
        }

        protected override void OnExitState()
        {
            
        }

        protected override void OnExitStateRender()
        {
            //결과화면 끄기
            UI.UIManager.Instance.SetActiveUI(UIType.ResultUIController, false);
        }
    }
}

