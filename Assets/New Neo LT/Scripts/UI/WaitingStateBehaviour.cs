using Fusion.Addons.FSM;

namespace New_Neo_LT.Scripts.UI
{
    public class WaitingStateBehaviour : StateBehaviour
    {
        protected override void OnEnterState()
        {
            base.OnEnterState();
        }

        protected override void OnEnterStateRender()
        {
            base.OnEnterStateRender();
            UIManager.Instance.EnterWaitingState();
            
#if UNITY_EDITOR
            UnityEngine.Debug.Log("대기 상태 진입");
#endif
        }

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
        }

        protected override void OnRender()
        {
            base.OnRender();
        }

        protected override void OnExitState()
        {
            base.OnExitState();
        }

        protected override void OnExitStateRender()
        {
            base.OnExitStateRender();
        }
    }
}