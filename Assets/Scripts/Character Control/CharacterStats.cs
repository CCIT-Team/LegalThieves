using System;
using System.Reflection;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

namespace New_Neo_LT.Scripts
{
    public enum EStatType
    {
        MaxHealth,
        MaxStamina,
        CurrentHealth,
        CurrentStamina,
        MoveSpeed,
        SprintSpeed,
        CrouchMoveSpeed,
        CrouchingSpeed,
        SpeedMultiplier
    }
    
    public class CharacterStats : NetworkBehaviour
    {
        [Networked] public int     MaxHealth        { get; private set; }
        [Networked] public int     MaxStamina       { get; private set; }
        [Networked] public int     CurrentHealth    { get; private set; }
        [Networked] public int     CurrentStamina   { get; private set; }
        [Networked] public float   MoveSpeed        { get; private set; }
        [Networked] public float   SprintSpeed      { get; private set; }
        [Networked] public float   CrouchMoveSpeed  { get; private set; }
        [Networked] public float   CrouchingSpeed   { get; private set; }
        [Networked] public float   SpeedMultiplier  { get; private set; } = 1f;
         
        [SerializeField] public StatConfig statConfig;
        
        /*------------------------------------------------------------------------------------------------------------*/

        #region NetworkBehaviour Events
        
        // public override void Spawned()
        // {
        //     
        // }
        //
        // public override void FixedUpdateNetwork()
        // {
        //     
        // }
        //
        // public override void Render()
        // {
        //     
        // }
        
        #endregion

        #region Stat Control Methods

        public virtual void InitializeStats()
        {
            if(statConfig == null)
                return;
            MaxHealth       = CurrentHealth  = statConfig.maxHealth;
            MaxStamina      = CurrentStamina = statConfig.maxStamina;
            MoveSpeed       = statConfig.moveSpeed;
            SprintSpeed     = statConfig.sprintSpeed;
            CrouchMoveSpeed = statConfig.crouchMoveSpeed;
            CrouchingSpeed  = statConfig.crouchingSpeed;
            SpeedMultiplier = statConfig.speedMultiplier;
        }

        public void ResetStat(EStatType statType)
        {
            switch (statType)
            {
                case EStatType.CurrentHealth:
                    CurrentHealth = MaxHealth;
                    break;
                case EStatType.CurrentStamina:
                    CurrentStamina = MaxStamina;
                    break;
                case EStatType.SpeedMultiplier:
                    SpeedMultiplier = 1f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statType), statType, "그런 스탯은 없습니다...");
            }
        }
        
        public void SetStat(EStatType eStatType, int value)
        {
            switch (eStatType)
            {
                case EStatType.MaxHealth:
                    MaxHealth = value;
                    break;
                case EStatType.MaxStamina:
                    MaxStamina = value;
                    break;
                case EStatType.CurrentHealth:
                    MaxHealth = value;
                    break;
                case EStatType.CurrentStamina:
                    MaxStamina = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eStatType), eStatType, "그런 스탯은 없습니다...");
            }
            OnStatChanged(eStatType, value);
        }
        
        public void SetStat(EStatType eStatType, float value)
        {
            switch (eStatType)
            {
                case EStatType.MoveSpeed:
                    MoveSpeed = value;
                    break;
                case EStatType.SprintSpeed:
                    SprintSpeed = value;
                    break;
                case EStatType.CrouchMoveSpeed:
                    CrouchMoveSpeed = value;
                    break;
                case EStatType.CrouchingSpeed:
                    CrouchingSpeed = value;
                    break;
                case EStatType.SpeedMultiplier:
                    SpeedMultiplier = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eStatType), eStatType, "그런 스탯은 없습니다...");
            }
            OnStatChanged(eStatType, value);
        }
        
        public void AddValueToCurrentStat(EStatType eStatType, int value)
        {
            switch (eStatType)
            {
                case EStatType.CurrentHealth:
                    CurrentHealth += value;
                    break;
                case EStatType.CurrentStamina:
                    CurrentStamina += value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eStatType), eStatType, "그런 스탯은 없습니다...");
            }
            OnStatChanged(eStatType, value);
        }
        
        #endregion

        #region Stat Change Callback

        //UI 갱신 혹은 다른 처리를 위한 콜백 함수
        private void OnStatChanged(EStatType statType, int value)
        {
            Type type = typeof(This);
            MethodInfo method = type.GetMethod("On" + statType + "Changed");
            if(method == null)
                throw new NullReferenceException("그런 스탯은 없습니다...");
            method.Invoke(method, new object[] {value});
            
            // switch (statType)
            // {
            //     case EStatType.MaxHealth:
            //         OnMaxHealthChanged(value);
            //         break;
            //     case EStatType.MaxStamina:
            //         OnMaxStaminaChanged(value);
            //         break;
            //     case EStatType.CurrentHealth:
            //         OnCurrentHealthChanged(value);
            //         break;
            //     case EStatType.CurrentStamina:
            //         OnCurrentStaminaChanged(value);
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException(nameof(statType), statType, "그런 스탯은 없습니다...");
            // }
        }
        
        private void OnStatChanged(EStatType statType, float value)
        {
            Type type = typeof(This);
            MethodInfo method = type.GetMethod("On" + statType + "Changed");
            if(method == null)
                throw new NullReferenceException("그런 스탯은 없습니다...");
            method.Invoke(method, new object[] {value});
            
            // switch (statType)
            // {
            //     case EStatType.MoveSpeed:
            //         OnMoveSpeedChanged(value);
            //         break;
            //     case EStatType.SprintSpeed:
            //         OnSprintSpeedChanged(value);
            //         break;
            //     case EStatType.CrouchMoveSpeed:
            //         OnCrouchMoveSpeedChanged(value);
            //         break;
            //     case EStatType.CrouchingSpeed:
            //         OnCrouchingSpeedChanged(value);
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException(nameof(statType), statType, "그런 스탯은 없습니다...");
            // }
        }
        
        protected virtual void OnMaxHealthChanged(int value) { }
        protected virtual void OnMaxStaminaChanged(int value) { }
        protected virtual void OnCurrentHealthChanged(int value) { }
        protected virtual void OnCurrentStaminaChanged(int value) { }
        protected virtual void OnMoveSpeedChanged(float value) { }
        protected virtual void OnSprintSpeedChanged(float value) { }
        protected virtual void OnCrouchMoveSpeedChanged(float value) { }
        protected virtual void OnCrouchingSpeedChanged(float value) { }

        #endregion
    }
}
