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
            UIManager.Instance.EnterEndGameState();
            
            
            if(!HasStateAuthority)
                return;
            
            NewGameManager.State.Server_DelaySetState<PregameStateBehaviour>(20);
        }
        
        protected override void OnExitState()
        {
            
        }
    
        protected override void OnExitStateRender()
        {
            //결과화면 끄기
            UIManager.Instance.SetActiveUI(UIType.ResultUIController, false);
            
            NewGameManager.Instance.Server_Shutdown();
            AudioManager.instance.SetSoundPack(AudioManager.instance.themes[(int)EField.Main]);
        }
    }
}

