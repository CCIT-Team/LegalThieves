using System;
using UnityEngine;

namespace ItemSkill.Skill
{
    public class SkillManager : MonoBehaviour
    {
        [SerializeField] private SkillBase emptySkill;
        [SerializeField, Space] private SkillBaseContainer skillDataContainer;
        
        public SkillBaseContainer SkillDataContainer => skillDataContainer;
        
        
    }
    
    [Serializable]
    public struct SkillBaseContainer
    {
        [SerializeField] public SkillBase[] skillBase;
        
        public bool TryGetSkillBase(int skillID, out SkillBase sBase)
        {
            foreach (var sd in skillBase)
            {
                if (sd.GetSkillData().skillID != skillID) 
                    continue;
                
                sBase =  sd;
                return true;
            }

            sBase = default;
            return false;
        }
    }
}