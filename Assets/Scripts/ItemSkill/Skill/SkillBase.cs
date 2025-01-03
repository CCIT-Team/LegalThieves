using System;
using UnityEngine;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.PlayerComponent;

namespace ItemSkill.Skill
{
    public abstract class SkillBase : ScriptableObject
    {
        [SerializeField] protected SkillData skillData;

        public virtual void OnSkillAdd(PlayerCharacter playerCharacter) { }

        public virtual void OnSkillEquip(PlayerCharacter playerCharacter) { }
        
        public virtual void OnSkillUnequip(PlayerCharacter playerCharacter) { }
        
        public virtual void OnSkillActive(PlayerCharacter playerCharacter) { }
        
        public virtual void OnSkillReady(PlayerCharacter playerCharacter) { }
        
        
        protected virtual bool CanSkillActive(PlayerCharacter playerCharacter)
        {
            return true;
        }
        
        protected virtual bool CanSkillEquip(PlayerCharacter playerCharacter)
        {
            return true;
        }
        
        protected virtual bool CanSkillUnequip(PlayerCharacter playerCharacter)
        {
            return true;
        }
        
        protected virtual bool CanSkillAdd(PlayerCharacter playerCharacter)
        {
            return true;
        }
        
        
        
        public SkillData GetSkillData()
        {
            return skillData;
        }
    }
    
    [Serializable]
    [CreateAssetMenu(menuName = "Skill/SkillData")]
    public class SkillData : ScriptableObject
    {
        [Header("Skill Data")]
        [SerializeField] public int     skillID;
        [SerializeField] public string  skillName;
        [SerializeField] public string  skillDescription;
        [SerializeField] public Sprite  skillIcon;
    }
    
    public abstract class ActiveSkill : SkillBase
    {
        [Space, Header("Active Skill Data")]
        [SerializeField] private float skillCoolTime;

        public float CoolTime      => skillCoolTime;
        
        public override void OnSkillAdd(PlayerCharacter playerCharacter)
        {
            base.OnSkillAdd(playerCharacter);
            // 
        }

        public override void OnSkillEquip(PlayerCharacter playerCharacter)
        {
            base.OnSkillEquip(playerCharacter);
            // animation triger or something
        }

        public override void OnSkillUnequip(PlayerCharacter playerCharacter)
        {
            base.OnSkillUnequip(playerCharacter);
            // animation triger or something
        }

        public override void OnSkillActive(PlayerCharacter playerCharacter)
        {
            base.OnSkillActive(playerCharacter);
            
        }
    }
    
    [CreateAssetMenu(menuName = "Skill/ActiveSkill")]
    public class PassiveSkill : SkillBase
    {
        
        
        public override void OnSkillAdd(PlayerCharacter playerCharacter)
        {
            base.OnSkillAdd(playerCharacter);
        }

        public override void OnSkillEquip(PlayerCharacter playerCharacter)
        {
            
        }

        public override void OnSkillUnequip(PlayerCharacter playerCharacter)
        {
            
        }

        public override void OnSkillActive(PlayerCharacter playerCharacter)
        {
            
        }
    }
    
    [CreateAssetMenu(menuName = "Skill/EmptySkill")]
    public class EmptySkill : PassiveSkill
    {
        public void InvokeSkill()
        {
            
        }
    }
    
    [CreateAssetMenu(menuName = "Skill/ActiveSkill/Teleport")]
    public class TeleportSkill : ActiveSkill
    {
        public override void OnSkillActive(PlayerCharacter playerCharacter)
        {
            base.OnSkillActive(playerCharacter);
            
            var random = UnityEngine.Random.Range(0, 3);
            playerCharacter.Teleport(NewGameManager.Instance.pregameMapData.GetSpawnPosition(random));
        }
    }
}