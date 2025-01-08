using Fusion;
using UnityEngine;

namespace ItemSkill.Skill.FastSkill
{
    public class SkillController : NetworkBehaviour
    {
        private const int Capacity = 3;
        
        [SerializeField] private SkillBehaviour[] skills;

        public void UseSkill(int index)
        {
            if (index is < 0 or >= Capacity) return;
            skills[index].UseSkill();
        }
    }
}
