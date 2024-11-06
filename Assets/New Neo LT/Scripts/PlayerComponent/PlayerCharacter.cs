using Fusion;
using Fusion.Addons.KCC;
using LegalThieves;
using New_Neo_LT.Scripts.Elements.Relic;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.Player_Input;
using System;
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
        [SerializeField] private int slotIndex = 0;


        private void Start()
        {
            for (int i = 0; i < 10; i++)
            {
                inventory[i] = -1;
            }
            // 로컬 플레이어인지 확인
            if (!Object.HasInputAuthority) // Photon Fusion에서는 HasInputAuthority로 로컬인지 확인
            {
                // 로컬 플레이어가 아닌 경우 AudioListener 비활성화
                AudioListener listener = GetComponentInChildren<AudioListener>();
                if (listener != null)
                {
                    listener.enabled = false;
                }
            }
        }


        [Networked]
        private int renownPoint { get; set; }
        [Networked]
        private int goldPoint { get; set; }

        private Transform camera;

        private RaycastHit      _rayCastHit;
        [SerializeField]
        private PlayerInteraction PlayerInteraction;
        [SerializeField]
        
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
            camera = camTarget;
            kcc.Settings.ForcePredictedLookRotation = true;

            _accumulatedMouseDelta = Runner.GetComponent<InputController>().AccumulatedMouseDelta;
        }

        private void InitializePlayerNetworkedProperties()
        {
           
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
            //getRelic
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Interaction2))
                CheckInteraction();
            //throwRelic
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Interaction5))
                ThrowRelic();
                //slot
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Slot1))
                SelectSlot(0);
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Slot2))
                SelectSlot(1);
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Slot3))
                SelectSlot(2);
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Slot4))
                SelectSlot(3);
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Slot5))
                SelectSlot(4);
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Slot6))
                SelectSlot(5);
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Slot7))
                SelectSlot(6);
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Slot8))
                SelectSlot(7);
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Slot9))
                SelectSlot(8);
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Slot10))
                SelectSlot(9);



            // Previous Buttons for comparison
            _previousButtons = playerInput.Buttons;
        }

        #endregion

        #region Movement Methods...

        private void OnJumpButtonPressed()
        {
            kcc.Jump(jumpImpulse);
            animator.SetTrigger(AnimJumpTrigger);
            if (Object.HasStateAuthority) // 서버 권한을 가진 경우에만 RPC 호출
            {
                RPC_PlayJumpSound(transform.position);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_PlayJumpSound(Vector3 position)
        {
            // 점프 소리 재생
            AudioManager.instance.PlayJumpSfx(position, true);
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

        public void CheckInteraction()
        {
            if (Object.HasInputAuthority)
            {
                PlayerInteraction.CheckInteraction(camera);

            }
    
        }

        #endregion


        #region Inventory
        public void SelectSlot(int index)
        {
            if (Object.HasInputAuthority) // 로컬 플레이어만 UI 업데이트
            {
                slotIndex = index;
                NewUiManager.Instance.SelectTogle(index);
            }
        }

        public bool SetSlot(int relicId)
        {
            if (Object.HasStateAuthority) // 서버 권한 확인
            {
                // 현재 선택된 슬롯이 비어있으면 해당 슬롯에 아이템을 추가
                if (inventory[slotIndex] == -1)
                {
                    inventory[slotIndex] = relicId;
                    UpdateInventoryUI(slotIndex, relicId, true);
                    return true;
                }
                else
                {
                    // 빈 슬롯을 찾아 아이템 추가
                    for (int i = 0; i < inventory.Length; i++)
                    {
                        if (inventory[i] == -1)
                        {
                            inventory[i] = relicId;
                            UpdateInventoryUI(i, relicId, true);
                            return true;
                        }
                    }
                    Debug.Log("인벤토리 빈공간 없음");
                }
            }
            return false;
        }

        private void UpdateInventoryUI(int index, int relicId, bool hasItem)
        {
            if (Object.HasInputAuthority) // 로컬 플레이어 UI만 업데이트
            {
                NewUiManager.Instance.SetRelicSprite(index, relicId, hasItem);
            }
        }
        

        public void ThrowRelic()
        {
            if (Object.HasStateAuthority && inventory[slotIndex] != -1) // 서버에서만 실행
            {
                // 현재 선택된 슬롯의 아이템을 버림
                //렐릭 풀에서 인덱스 검사해서 가져와 오브젝트에 할당
                Debug.Log($"Drop {inventory[slotIndex]}");
                RelicManager.instance.SpawnRelic(inventory[slotIndex], camera.position + transform.forward * 2);
                //Runner.Spawn(오브젝트, camera.position + transform.forward * 2);
                inventory[slotIndex] =  -1; // 인벤토리에서 제거
                UpdateInventoryUI(slotIndex, -1, false); // 로컬 UI 업데이트
            }
            else if (Object.HasInputAuthority && inventory[slotIndex] == null)
            {
                Debug.Log("아이템이 없습니다.");
            }
        }

        #endregion

        #region PointSystem
        public void SellRelic()
        {
            NewGameManager.Instance.SellRelic(Runner.LocalPlayer, slotIndex);
        }
        public void GetPoint()
        {

        }
        #endregion
        #region RPC Methods...

        #endregion
    }
}
