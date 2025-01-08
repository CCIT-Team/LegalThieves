using Fusion;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.UI;
using UnityEngine;

namespace ItemSkill.Skill.FastSkill
{
    public class Teleport : SkillBehaviour
    {
        [SerializeField] private Transform destination;
        [SerializeField] private float     delay;
        [SerializeField] private string    teleportAnimation;
        
        [Space, SerializeField] private GameObject teleportEffect;
        
        [Networked] private TickTimer DelayTimer { get; set; }
        
        [Networked] 
        [OnChangedRender(nameof(OnTeleport))]
        private bool IsTeleporting { get; set; }

        public void OnTeleport()
        {
            if (IsTeleporting)
            {
                OnTeleportingStart();
            }
            else
            {
                OnTeleportingEnd();
            }
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            
            if(!IsTeleporting)
                return;
            if(!DelayTimer.ExpiredOrNotRunning(Runner))
                return;
            
            IsTeleporting = false;
            
            TeleportToDestination();
        }

        protected override void OnSkillUsed()
        {
            DelayTimer = TickTimer.CreateFromSeconds(Runner, delay);
            IsTeleporting = true;
        }
        
        private void TeleportToDestination()
        {
            var pc = Player;
            var dest = NewGameManager.Instance.pregameMapData.GetSpawnPosition(Random.Range(0, 3));
            
            pc.Teleport(dest);
        }
        
        private void OnTeleportingStart()
        {
            var effect = Instantiate(teleportEffect).GetComponent<ReturnEffectController>();
            effect.Setup(delay, Player.transform.position);
            
            if (HasInputAuthority)
            {
                UIManager.Instance.skillUIController.WriteToConsole("내가 텔레포트 사용!");
            }
            else
            {
                UIManager.Instance.skillUIController.WriteToConsole($"{Player.GetPlayerName()} 텔레포트 사용!");
            }
            
            
        }
        
        private void OnTeleportingEnd()
        {
            if (HasInputAuthority)
            {
                UIManager.Instance.skillUIController.WriteToConsole("내가 텔레포트 완료!");
            }
            else
            {
                UIManager.Instance.skillUIController.WriteToConsole($"{Player.GetPlayerName()} 텔레포트 완료!");
            }
            
            
        }
    }
}