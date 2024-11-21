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
            UI.UIManager.Instance.readyStateUI.IsActive = !UI.UIManager.Instance.readyStateUI.IsActive;
        }
        
        
    }
}