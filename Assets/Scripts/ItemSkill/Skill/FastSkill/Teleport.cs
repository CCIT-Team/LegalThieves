using Fusion;
using New_Neo_LT.Scripts.Game_Play;
using UnityEngine;

namespace ItemSkill.Skill.FastSkill
{
    public class Teleport : SkillBehaviour
    {
        [SerializeField] private Transform destination;
        [SerializeField] private float     delay;
        [SerializeField] private string    teleportAnimation;
        
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
            if (HasInputAuthority)
            {
                
            }
            Debug.Log($"{Player.name} Teleporting");
        }
        
        private void OnTeleportingEnd()
        {
            if (HasInputAuthority)
            {
                
            }
            Debug.Log($"{Player.name} Teleporting End");
        }
    }
}