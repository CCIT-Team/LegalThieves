using System;
using Fusion;
using ItemSkill.Skill;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.PlayerComponent;
using New_Neo_LT.Scripts.UI;
using UnityEngine;

namespace New_Neo_LT.Scripts.PlayerComponent
{
    #region Debug

    [Serializable]
    public struct DebugSkillHolder
    {
        [SerializeField] public int          skillID;
        [SerializeField] public ESkillState  skillState;
        [SerializeField] public float        timer;
    }

    #endregion
    
    public class PlayerSkillController : NetworkBehaviour
    {
        private const int MaxSkillCount = 3;

        [SerializeField] private DebugSkillHolder[] debugSkillHolders;
        
        
        [Networked, Capacity(MaxSkillCount)]
        [OnChangedRender(nameof(OnSkillHolderChanged))]
        private NetworkArray<SkillHolder> SkillHolders => default;

        // [Networked, Capacity(MaxSkillCount)]
        // [OnChangedRender(nameof(OnSkillHolderChanged))]
        // private NetworkDictionary<SkillHolder, TickTimer> SkillHolders => default;
        
        public override void Spawned()
        {
            base.Spawned();

            if (HasStateAuthority)
            {
                Server_InitHolders();                
            }
        }

        private void Server_InitHolders()
        {
            for(var i = 0; i < MaxSkillCount; i++)
            {
                var sHolder = SkillHolder.Defaults;
                
                SkillHolders.Set(i, sHolder);
            }
        }
        
        private void OnSkillHolderChanged(NetworkBehaviourBuffer previous)
        {
            for (var i = 0; i < MaxSkillCount; i++)
            {
                var sHolder = SkillHolders[i];
                
#if UNITY_EDITOR
                debugSkillHolders[i].skillID = sHolder.SkillID;
                debugSkillHolders[i].skillState = sHolder.State;
                debugSkillHolders[i].timer = sHolder.Timer.RemainingTime(Runner) ?? 0;
#endif
                
                if (!sHolder.IsSkillChanged)
                    continue;
                sHolder.IsSkillChanged = false;
                
                // UI Update 추가
                var sBase = GetSkill(i);
                var sSprite = sBase.GetSkillData().skillIcon;
                UIManager.Instance.itemSkillInventoryUI.SetSlotImage(i + 3, sSprite);
            }

            // 

            // UI Update 추가
        }
        
        public void Server_ReadySkill(int inventoryIndex)
        {
            if (inventoryIndex is < 0 or >= MaxSkillCount)
                return;
            
            if(!SkillHolders[inventoryIndex].TryReady(Object.GetComponent<PlayerCharacter>()))
                return;
            
            // 사용 시
        }
        
        public void Server_EquipSkill(int inventoryIndex)
        {
            if (inventoryIndex is < 0 or >= MaxSkillCount)
                return;
            
            if(!SkillHolders[inventoryIndex].TryEquip(Object.GetComponent<PlayerCharacter>()))
                return;
            
            // 사용 시
        }
        
        public void Server_UnequipSkill(int inventoryIndex)
        {
            if (inventoryIndex is < 0 or >= MaxSkillCount)
                return;
            
            if(!SkillHolders[inventoryIndex].TryUnequip(Object.GetComponent<PlayerCharacter>()))
                return;
            
            // 사용 시
        }
        
        public void Server_ActiveSkill(int inventoryIndex)
        {
            if (inventoryIndex is < 0 or >= MaxSkillCount)
                return;
            
            if(!SkillHolders[inventoryIndex].TryActive(Object.GetComponent<PlayerCharacter>()))
                return;
            
            // 사용 시
        }
        
        public void Server_AddSkill(int inventoryIndex, int skillID)
        {
            if (inventoryIndex is < 0 or >= MaxSkillCount)
                return;
            
            if(SkillHolders[inventoryIndex].SkillID != 0)
                return;
            
            if(!TryGetSkillBase(skillID, out var sBase))
                return;

            var sHolder = SkillHolders[inventoryIndex];
            
            sHolder.SkillID = skillID;
            
            if(sHolder.Timer.IsRunning)
                sHolder.Timer = TickTimer.None;
            
            sBase.OnSkillAdd(Object.GetComponent<PlayerCharacter>());
        }

        public SkillBase GetSkill(int inventoryIndex)
        {
            if (inventoryIndex is < 0 or >= MaxSkillCount)
            {
                return null;
            }
            
            var sHolder = SkillHolders[inventoryIndex];

            if (TryGetSkillBase(sHolder.SkillID, out var sBase))
                return null;
            
            return sBase;
        }
        
        private static bool TryGetSkillBase(int skillID, out SkillBase skillBase)
        {
            if (NewGameManager.Instance.SkillManager)
                return NewGameManager.Instance.SkillManager.SkillDataContainer.TryGetSkillBase(skillID, out skillBase);
            
            skillBase = default;
            return false;
        }
    }
    
    public struct SkillHolder : INetworkStruct
    {
        private int ID { get; set; }

        public int SkillID
        {
            get => ID;
            set
            {
                ID = value;
                IsSkillChanged = true;
            }
        }
        
        public NetworkBool IsSkillChanged { get; set; }
        
        private int SkillState { get; set; }
        
        public TickTimer Timer { get; set; }

        public ESkillState State => (ESkillState)SkillState;

        public static SkillHolder Defaults
        {
            get
            {
                var result = new SkillHolder
                {
                    SkillID = 0,
                    SkillState = (int)ESkillState.None,
                    Timer = TickTimer.None
                };
                return result;
            }
        }

        public void Reset()
        {
            SkillID = 0;
            SkillState = (int)ESkillState.None;
            Timer = TickTimer.None;
        }
        
        public void Set(int skillID)
        {
            SkillID = skillID;
            SkillState = (int)ESkillState.Ready;
            Timer = TickTimer.None;
        }

        public bool TryTransition(ESkillState nextState)
        {
            return true;
        }

        public bool TryReady(PlayerCharacter playerCharacter)
        {
            if (!Timer.ExpiredOrNotRunning(NewGameManager.LocalRunner))
                return false;
            
            if(!NewGameManager.Instance.SkillManager.SkillDataContainer.TryGetSkillBase(SkillID, out var sBase))
                return false;

            SkillState = (int)ESkillState.Ready;
            
            sBase.OnSkillReady(playerCharacter);
            
            return true;
        }
        
        public bool TryActive(PlayerCharacter playerCharacter)
        {
            if (!CanActive)
                return false;
            
            if(!NewGameManager.Instance.SkillManager.SkillDataContainer.TryGetSkillBase(SkillID, out var sBase))
                return false;

            SkillState = (int)ESkillState.Active;
            
            sBase.OnSkillActive(playerCharacter);
            
            return true;
        }
        
        public bool TryEquip(PlayerCharacter playerCharacter)
        {
            if (!CanEquip)
                return false;
            
            if(!NewGameManager.Instance.SkillManager.SkillDataContainer.TryGetSkillBase(SkillID, out var sBase))
                return false;

            SkillState = (int)ESkillState.Equipped;
            
            sBase.OnSkillEquip(playerCharacter);
            
            return true;
        }
        
        public bool TryUnequip(PlayerCharacter playerCharacter)
        {
            if (!CanUnequip)
                return false;
            
            if(!NewGameManager.Instance.SkillManager.SkillDataContainer.TryGetSkillBase(SkillID, out var sBase))
                return false;

            SkillState = (int)ESkillState.Ready;
            
            sBase.OnSkillUnequip(playerCharacter);
            
            return true;
        }
        
        public bool CanReady => true;
        
        public bool CanActive => State is ESkillState.Equipped;
        
        public bool CanEquip => State is ESkillState.Ready;
        
        public bool CanUnequip => State is ESkillState.Active or ESkillState.Equipped;
    }
    
    public enum ESkillState
    {
        None = -1,
        Ready,
        Equipped,
        Active,
        Count
    }
}