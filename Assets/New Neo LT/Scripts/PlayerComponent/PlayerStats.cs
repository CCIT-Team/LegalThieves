namespace New_Neo_LT.Scripts
{
    public class PlayerStats : CharacterStats
    {
        
        
        /*------------------------------------------------------------------------------------------------------------*/
        
        #region Stat Change Callbacks

        protected override void OnMaxHealthChanged(int value)
        {
            // 최대체력 변경시 처리할 내용
        }

        protected override void OnMaxStaminaChanged(int value)
        {
            // 최대스태미나 변경시 처리할 내용
        }

        protected override void OnCurrentHealthChanged(int value)
        {
            // 현재체력 변경시 처리할 내용
        }

        protected override void OnCurrentStaminaChanged(int value)
        {
            // 현재스태미나 변경시 처리할 내용
        }

        protected override void OnMoveSpeedChanged(float value)
        {
            // 이동속도 변경시 처리할 내용
        }

        protected override void OnSprintSpeedChanged(float value)
        {
            // 달리기속도 변경시 처리할 내용
        }

        protected override void OnCrouchMoveSpeedChanged(float value)
        {
            // 앉았을 때 이동속도 변경시 처리할 내용
        }

        protected override void OnCrouchingSpeedChanged(float value)
        {
            // 앉았을 때 이동속도 변경시 처리할 내용
        }

        #endregion
    }
}
