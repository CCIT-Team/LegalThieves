using Fusion;
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;

namespace ItemSkill.Skill.FastSkill
{
    public abstract class SkillBehaviour : NetworkBehaviour
    {
        [SerializeField] private int cooldown;
        
        [Networked]
        private TickTimer CooldownTimer { get; set; }
        
        private bool CanUseSkill => CooldownTimer.ExpiredOrNotRunning(Runner);
        
        protected PlayerCharacter Player => transform.root.GetComponent<PlayerCharacter>();


        public void UseSkill()
        {
            if (!CanUseSkill)
                return;
            
            OnSkillUsed();

            CooldownTimer = TickTimer.CreateFromSeconds(Runner, cooldown);
        }

        protected abstract void OnSkillUsed();
    }
}