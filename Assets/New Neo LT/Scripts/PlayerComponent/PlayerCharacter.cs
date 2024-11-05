using Fusion;
using Fusion.Addons.KCC;
using LegalThieves;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.Player_Input;
using TMPro;
using UnityEngine;
using EInputButton = New_Neo_LT.Scripts.Player_Input.EInputButton;
using NetInput = New_Neo_LT.Scripts.Player_Input.NetInput;

namespace New_Neo_LT.Scripts.PlayerComponent
{
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerCharacter : Character
    {
        [Header("Player Components")]
        [SerializeField] private Transform             camTarget;
        [SerializeField] private TMP_Text              playerNickname;

        [Header("Player Setup")]
        [Range(-90, 90)]
        [SerializeField] private float                 maxPitch         = 85f; 
        [SerializeField] private float                 lookSensitivity  = 0.15f;
        [SerializeField] private Vector3               jumpImpulse      = new(0f, 5f, 0f);
        [SerializeField] private float                 interactionRange = 5f;
        
        [Networked] 
        public PlayerRef          Ref        { get; set; }
        [Networked] 
        public byte               Index      { get; set; }
        [Networked, OnChangedRender(nameof(NicknameChanged))] 
        public NetworkString<_16> Nickname   { get; set; }
        [Networked] 
        private float             CrouchSync { get; set; } = 1f;
        

        private NetworkButtons  _previousButtons;
        private Vector2         _accumulatedMouseDelta;
        private string          _playerName;
        
        private bool            _isSprinting;
        private bool            CanJump   => kcc.FixedData.IsGrounded;
        private bool            CanSprint => kcc.FixedData.IsGrounded && characterStats.CurrentStamina > 0;



       

        [SerializeField] private int[] inventory = new int[10];
        [SerializeField] private int slotIndex =0;




        private RaycastHit      _rayCastHit;
        [SerializeField]
        private PlayerInteraction PlayerInteraction;
        public static PlayerCharacter Local { get; set; }

        #region Animation Hashes...

        private static readonly int AnimMoveDirX      = Animator.StringToHash("MoveDirX");
        private static readonly int AnimMoveDirY      = Animator.StringToHash("MoveDirY");
        private static readonly int AnimIsCrouchSync  = Animator.StringToHash("IsCrouchSync");
        private static readonly int AnimLookPit       = Animator.StringToHash("LookPit");
        private static readonly int AnimJumpTrigger   = Animator.StringToHash("Jump");
        
        #endregion
        
        /*------------------------------------------------------------------------------------------------------------*/

        #region NetworkBehaviour Events...

        public override void Spawned()
        {
            base.Spawned();

            if (Object.HasStateAuthority)
            {
                PlayerRegistry.Server_Add(Runner, Object.InputAuthority, this);
            }

            if (Object.HasInputAuthority)
            {
                Local = this;
                InitializeCharacterComponents();
                InitializePlayerComponents();
                InitializePlayerNetworkedProperties();
            }
            
            
            NicknameChanged();
        }
        
        public override void FixedUpdateNetwork()
        {
            // Process Player Input
            if(GetInput(out NetInput playerInput))
                SetPlayerInput(playerInput);
        }
        
        public override void Render()
        {
            // Update Player facing direction
            if(kcc.Settings.ForcePredictedLookRotation)
                kcc.SetLookRotation(kcc.GetLookRotation() + _accumulatedMouseDelta * lookSensitivity);
            camTarget.localRotation = Quaternion.Euler(kcc.GetLookRotation().x, 0f, 0f);
            
            var moveVelocity = GetAnimationMoveVelocity();
            animator.SetFloat(AnimMoveDirX    , moveVelocity.x, 0.05f, Time.deltaTime);
            animator.SetFloat(AnimMoveDirY    , moveVelocity.z, 0.05f, Time.deltaTime);
            animator.SetFloat(AnimIsCrouchSync, CrouchSync);
            animator.SetFloat(AnimLookPit     , kcc.FixedData.LookPitch);
        }

        

        #endregion

        private void InitializePlayerComponents()
        {
            camTarget ??= transform.Find("CamTarget");
            CameraFollow.Singleton.SetTarget(camTarget);
            
            kcc.Settings.ForcePredictedLookRotation = true;

            _accumulatedMouseDelta = Runner.GetComponent<InputController>().AccumulatedMouseDelta;
        }

        private void InitializePlayerNetworkedProperties()
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                inventory[i] = -1;
            }
        }
        
        public void SetPlayerName(string playerName)
        {
            _playerName = playerName;
        }
        
        private void NicknameChanged()
        {
            // playerNickname.text = Nickname.Value;
        }
        
        public void Server_Init(PlayerRef pRef, byte index)
        {
            Debug.Assert(Runner.IsServer);

            Ref = pRef;
            Index = index;
        }
        
        #region Player Input Methods...

        // Player Input to player character state
        private void SetPlayerInput(NetInput playerInput)
        {
            // Set Movement (WASD)
            kcc.SetInputDirection(kcc.FixedData.TransformRotation * playerInput.Direction.X0Y());
            
            // Set face direction by mouse pointer position delta
            kcc.AddLookRotation(playerInput.LookDelta * lookSensitivity, -maxPitch, maxPitch);
            
            // Set Camera target Rotation
            //camTarget.rotation = Quaternion.Euler(kcc.GetLookRotation().x, 0f, 0f);
            
            // Set behavior by mouse click input
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Attack1))
                OnMouseLeftClick();
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Attack2))
                OnMouseRightClick();
            
            // Set behavior by Keyboard input
            // Sprint
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Sprint) && CanSprint)
                kcc.FixedData.KinematicSpeed = characterStats.SprintSpeed;
            if (playerInput.Buttons.WasReleased(_previousButtons, EInputButton.Sprint))
                kcc.FixedData.KinematicSpeed = characterStats.MoveSpeed;
            
            // Jump
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Jump))
                OnJumpButtonPressed();
            // Crouch
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Crouch))
                CrouchSync = 0;
            if(playerInput.Buttons.WasReleased(_previousButtons, EInputButton.Crouch))
                CrouchSync = 1;

            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Interaction2))
                PlayerInteraction.CheckInteraction();

            if(playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Slot1))
                slotIndex = 0;
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Slot2))
                slotIndex = 1;
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Slot3))
                slotIndex = 2;
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Slot4))
                slotIndex = 3;
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Slot5))
                slotIndex = 4;
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Slot6))
                slotIndex = 5;
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Slot7))
                slotIndex = 6;
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Slot8))
                slotIndex = 7;
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Slot9))
                slotIndex = 8;
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Slot10))
                slotIndex = 9;

           

            // Previous Buttons for comparison
            _previousButtons = playerInput.Buttons;
        }

        #endregion

        #region Movement Methods...

        private void OnJumpButtonPressed()
        {
            kcc.Jump(jumpImpulse);
            animator.SetTrigger(AnimJumpTrigger);
        }
        
        private Vector3 GetAnimationMoveVelocity()
        {
            if (kcc.Data.RealSpeed < 0.01f)
                return Vector3.zero;

            var velocity = kcc.Data.RealVelocity;

            velocity.y = 0f;

            if (velocity.sqrMagnitude > 1f)
            {
                velocity.Normalize();
            }

            return transform.InverseTransformVector(velocity);
        }

        #endregion
        
        #region Interaction Methods...

        private void OnMouseLeftClick()
        {
            // 현재 들고있는 아이템에 따라 바뀜 아이템 클래스 구현 후 추가 예정
            
            // Interaction Raycast
            if (Physics.Raycast(camTarget.position, camTarget.forward, out _rayCastHit, interactionRange))
            {
                if (_rayCastHit.collider.TryGetComponent(out Scripts.Elements.Relic.Relic relic))
                {
                    relic.TryInteraction(this);
                }
            }
        }
        
        private void OnMouseRightClick()
        {
            // 현재 들고있는 아이템에 따라 바뀜 아이템 클래스 구현 후 추가 예정
        }

        #endregion

        #region Inventory
        
        //public int GetSlot(int relicId)
        //{
        //    inventory[slotIndex]
        //}
        public bool SetSlot(int relicId)
        {
            if (inventory[slotIndex] == -1 )
            {
                inventory[slotIndex] = relicId;

                return true;
            }
            else
            {
                for(int i =0; i< inventory.Length; i++)
                {
                    if (inventory[i] == -1)
                    {
                        inventory[i] = relicId;

                        return true;
                    }
                }
             
                Debug.Log("인벤토리 빈공간 없음");
            
            }
           
            return false;

        }
        public TMP_Text a;
        public void SellRelic()
        {

        }

        #endregion

        #region RPC Methods...



        #endregion
    }
}
