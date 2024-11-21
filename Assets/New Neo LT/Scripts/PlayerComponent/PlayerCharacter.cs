using System;
using System.Linq;
using Fusion;
using Fusion.Addons.KCC;
using LegalThieves;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.Player_Input;
using New_Neo_LT.Scripts.Relic;
using TMPro;
using UnityEngine;
using EInputButton = New_Neo_LT.Scripts.Player_Input.EInputButton;
using NetInput = New_Neo_LT.Scripts.Player_Input.NetInput;
using RelicManager = LegalThieves.RelicManager;
using UIManager = New_Neo_LT.Scripts.UI.UIManager;

public enum Job { Archaeologist, Linguist , BusinessCultist , Shamanist }

namespace New_Neo_LT.Scripts.PlayerComponent
{
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerCharacter : Character
    {
        [Header("Player Components")]
        [SerializeField] private Transform              camTarget;
        [SerializeField] private TMP_Text               playerNickname;
        [SerializeField] private PlayerInteraction      playerInteraction;
        [SerializeField] private SkinnedMeshRenderer    skinnedMeshRenderer;
       
        [Space, Header("Player Setup")]
        [Range(-90, 90)]
        [SerializeField] private float                  maxPitch         = 85f; 
        [SerializeField] private float                  lookSensitivity  = 0.15f;
        [SerializeField] private Vector3                jumpImpulse      = new(0f, 5f, 0f);
        [SerializeField] private float                  interactionRange = 5f;
        
        [Space, Header("Player Models")]
        [SerializeField] private GameObject[]           playerModels;
        
        [Networked, OnChangedRender(nameof(OnRefChanged))] 
        public PlayerRef          Ref        { get; set; }
        [Networked] 
        public byte               Index      { get; set; }
        
        [Networked, OnChangedRender(nameof(OnColorChanged))]
        private int               PlayerColor { get; set; }
        [Networked, OnChangedRender(nameof(OnCurrentPlayerModelIndexChanged))] 
        private int               CurrentPlayerModelIndex { get; set; }
        
        [Networked, OnChangedRender(nameof(OnPlayerJobChanged))]
        public bool               IsScholar { get; set; }
        [Networked, OnChangedRender(nameof(OnPointChanged))]
        private int               RenownPoint { get; set; }
        [Networked, OnChangedRender(nameof(OnPointChanged))]
        private int               GoldPoint { get; set; }
        
        [Networked, OnChangedRender(nameof(NicknameChanged))] 
        public NetworkString<_16> Nickname   { get; set; }

        [Networked, Capacity(10), OnChangedRender(nameof(OnInventoryChanged))]
        public NetworkArray<int> Inventory => default;

        [Networked]
        Job job { get; set; }

        [Networked] 
        private float             CrouchSync { get; set; } = 1f;
        
     
        private NetworkButtons    _previousButtons;
        private Vector2           _accumulatedMouseDelta;

        private bool              _isSprinting;
        
        private bool              CanJump   => kcc.FixedData.IsGrounded;
        private bool              CanSprint => kcc.FixedData.IsGrounded && characterStats.CurrentStamina > 0;

        public int GetGoldPoint => GoldPoint;
        public int GetRenownPoint => RenownPoint;

      
        [SerializeField] private int slotIndex = 0;

        private RaycastHit      _rayCastHit;
        
        public static PlayerCharacter Local { get; set; }

        #region Animation Hashes...

        private static readonly int AnimMoveDirX      = Animator.StringToHash("MoveDirX");
        private static readonly int AnimMoveDirY      = Animator.StringToHash("MoveDirY");
        private static readonly int AnimIsCrouchSync  = Animator.StringToHash("IsCrouchSync");
        private static readonly int AnimLookPit       = Animator.StringToHash("LookPit");
        private static readonly int AnimJumpTrigger   = Animator.StringToHash("Jump");
        private static readonly int AnimSnapGround    = Animator.StringToHash("SnapGround");
        
        #endregion
        
        /*------------------------------------------------------------------------------------------------------------*/

        #region NetworkBehaviour Events...

        private void Start()
        {
            Client_InitPlayerModle(CurrentPlayerModelIndex);
            
        }

        public override void Spawned()
        {
            base.Spawned();

            if (Object.HasStateAuthority)
            {
                PlayerRegistry.Server_Add(Runner, Object.InputAuthority, this);

                var playerIndex = Object.InputAuthority.AsIndex - 1;
                PlayerColor = playerIndex;
                CurrentPlayerModelIndex = playerIndex;
                IsScholar = playerIndex % 2 != 0;
            

                for (var i = 0; i < Inventory.Length;i++)
                {
                    Inventory.Set(i, -1);
                }
            }

            if (Object.HasInputAuthority)
            {
                Local = this;
                InitializePlayerComponents();
                UIManager.Instance.InitializeInGameUI();
                InitModels();
            }
            
            UIManager.Instance.playerListController.PlayerJoined(this);
            
            InitializeCharacterComponents();
            InitializePlayerNetworkedProperties();
            
            NicknameChanged();
            SetPlayerTag(job.ToString());
        }
        void SetPlayerTag(string tag)
        {
            playerNickname.text = tag;
        }
        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            
            // Process Player Input
            if(GetInput(out NetInput playerInput))
                SetPlayerInput(playerInput);
        }
        
        public override void Render()
        {
            // Update Player facing direction
            // if(kcc.Settings.ForcePredictedLookRotation) // 화면 끊김 발생해서 뺌
            kcc.SetLookRotation(kcc.GetLookRotation() + _accumulatedMouseDelta * lookSensitivity);
            camTarget.localRotation = Quaternion.Euler(kcc.GetLookRotation().x, 0f, 0f);
            
            if(kcc.FixedData.IsSnappingToGround)
                animator.SetTrigger(AnimSnapGround);
            
            var moveVelocity = GetAnimationMoveVelocity();
            animator.SetFloat(AnimMoveDirX    , moveVelocity.x, 0.05f, Time.deltaTime);
            animator.SetFloat(AnimMoveDirY    , moveVelocity.z * 2, 0.05f, Time.deltaTime);
            animator.SetFloat(AnimIsCrouchSync, CrouchSync);
            animator.SetFloat(AnimLookPit     , kcc.FixedData.LookPitch * -0.01f);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            UIManager.Instance.playerListController.PlayerLeft(this);
        }

        #endregion

        private void InitializePlayerComponents()
        {
            camTarget ??= transform.Find("CamTarget");
            CameraFollow.Singleton.SetTarget(camTarget);
            // kcc.Settings.ForcePredictedLookRotation = true; // 화면 끊김 발생해서 뺌

            _accumulatedMouseDelta = Runner.GetComponent<InputController>().AccumulatedMouseDelta;
        }

        private void InitializePlayerNetworkedProperties()
        {
            skinnedMeshRenderer ??= GetComponentInChildren<SkinnedMeshRenderer>();
        }
        
        public void SetPlayerName(string playerName)
        {
            
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
            SprintToggle(playerInput);
            // if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Sprint) && CanSprint)
            //     kcc.FixedData.KinematicSpeed = characterStats.SprintSpeed;
            // if (playerInput.Buttons.WasReleased(_previousButtons, EInputButton.Sprint))
            //     kcc.FixedData.KinematicSpeed = characterStats.MoveSpeed;
            
            // Jump
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Jump) && CanJump)
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
        
        private void SprintToggle(NetInput input)
        {
            if (input.Buttons.WasPressed(_previousButtons, EInputButton.Sprint))
            {
                _isSprinting = true;
                kcc.AddModifier(kccProcessors[1]);
            }
            if(input.Buttons.WasReleased(_previousButtons, EInputButton.Sprint) && _isSprinting)
            {
                _isSprinting = false;
                kcc.RemoveModifier(kccProcessors[1]);
            }
        }

        #endregion
        
        #region Interaction Methods...

        private void OnMouseLeftClick()
        {
            // 현재 들고있는 아이템에 따라 바뀜 아이템 클래스 구현 후 추가 예정
            
            // Interaction Raycast
            if (!Physics.Raycast(camTarget.position, camTarget.forward, out _rayCastHit, interactionRange)) 
                return;
            
            if (_rayCastHit.collider.TryGetComponent(out Scripts.Elements.Relic.Relic relic))
            {
                //relic.Interact(Object);
            }
        }
        
        private void OnMouseRightClick()
        {
            // 현재 들고있는 아이템에 따라 바뀜 아이템 클래스 구현 후 추가 예정
        }

        public void CheckInteraction()
        {
            if (HasStateAuthority) 
                playerInteraction.Server_CheckInteraction();
            
            if (HasInputAuthority)
                playerInteraction.CheckInteraction();
        }

        #endregion


        #region Inventory

        private void SelectSlot(int index)
        {
            if (!HasStateAuthority && !HasInputAuthority) 
                return;
            
            slotIndex = index;
            if (!HasInputAuthority)
                return;
            
            UIManager.Instance.inventorySlotController.SelectToggle(index);

            UIManager.Instance.relicPriceUI.SetUIPoint(Inventory[index]);
        }

        public bool GetRelic(int relicId)
        {
            // 현재 선택된 슬롯이 비어있으면 해당 슬롯에 아이템을 추가
            if (Inventory[slotIndex] == -1)
            {
                Inventory.Set(slotIndex, relicId);
                return true;
            }

            // 빈 슬롯을 찾아 아이템 추가
            for (var i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] != -1) 
                    continue;
                
                Inventory.Set(i, relicId);
                return true;
            }
            
            return false;
        }
        
        public void ThrowRelic()
        {
            if (HasStateAuthority && Inventory[slotIndex] != -1) // 서버에서만 실행
            {
                RelicManager.Instance.GetRelicData(Inventory[slotIndex]).OnThrowAway(Object.InputAuthority);
                Inventory.Set(slotIndex, -1); // 인벤토리에서 제거
            }
        }
        
        public RelicObject RemoveRelicFromInventory(int index)
        {
            if (Inventory[index] == -1)
                return null;
            
            var relic = RelicManager.Instance.GetRelicData(Inventory[index]);
            Inventory.Set(index, -1);
            return relic;
        }

        private void UpdateInventoryUI(int index, int relicId)
        {
            if (HasInputAuthority) // 로컬 플레이어 UI만 업데이트
            {
                UIManager.Instance.inventorySlotController.SetRelicSprite(index, relicId);
            }
        }
        
        public Transform GetCamTarget()
        {
            return camTarget;
        }

        public void OnInventoryChanged(NetworkBehaviourBuffer previous)
        {
            if(!HasInputAuthority)
                return;
            for (var i = 0; i < 10; i++)
            {
                UIManager.Instance.inventorySlotController.SetRelicSprite(i, Inventory[i]);
            }
            UIManager.Instance.relicPriceUI.SetUIPoint(Inventory[slotIndex]);
            UIManager.Instance.shopController.SetLocalPlayerInventory(Inventory.ToArray());
        }

        public void SetPlayerColor(int index) => PlayerColor = index;
        public int GetPlayerColor() => PlayerColor;

        private void OnColorChanged()
        {
            skinnedMeshRenderer.materials[1] = NewGameManager.Instance.playerClothMaterials[PlayerColor];
            skinnedMeshRenderer.materials[4] = NewGameManager.Instance.playerHairMaterials[PlayerColor];
        }
        
       
        public void ChangeJob(Job newJob)
        {
          
            job = newJob;
            SetPlayerTag(job.ToString());
            switch (job)
            {
                case Job.Archaeologist:
                case Job.Linguist:
                    IsScholar = true;
                    break;

                case Job.BusinessCultist:
                case Job.Shamanist:
                    IsScholar = false;
                    break;
            }
            
        }

        #endregion

        #region Network Porperty Changed Events...
        
        private void OnPlayerJobChanged()
        {
            UIManager.Instance.playerListController.UpdatePlayerPointType(Index, IsScholar);
        }
        
        private void OnPointChanged()
        {
            UIManager.Instance.playerListController.UpdatePlayerScore(Index, IsScholar, GoldPoint, RenownPoint);
        }

        private void OnRefChanged()
        {
            
        }
        
        #endregion

        #region Gold, Renown Point Add...

        public void AddGoldPoint(int point)
        {
            GoldPoint += point;
        }
        
        public void AddRenownPoint(int point)
        {
            RenownPoint += point;
        }

        #endregion

        private void InitModels()
        {
            foreach (var model in playerModels)
            {
                var mesh = model.GetComponentInChildren<SkinnedMeshRenderer>();
                mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
        }
        
        private void OnCurrentPlayerModelIndexChanged(NetworkBehaviourBuffer previous)
        {
            // 변경 이전 모델 비활성화
            var prevValue = GetPropertyReader<int>(nameof(CurrentPlayerModelIndex)).Read(previous);
            playerModels[prevValue].SetActive(false);
            
            // 변경 이후 모델 활성화
            var newModel = playerModels[CurrentPlayerModelIndex];
            newModel.SetActive(true);
            
            // 애니메이터를 변경된 모델의 애니메이터로 변경
            animator = newModel.GetComponent<Animator>();
        }

        private void ChangePlayerModel(int index)
        {
            var prev = playerModels[CurrentPlayerModelIndex];
            var curr = playerModels[index];
            
            prev.SetActive(false);
            curr.SetActive(true);
            
            animator = curr.GetComponent<Animator>();
            
            CurrentPlayerModelIndex = index;
        }

        private void Client_InitPlayerModle(int index)
        {
            var prev = playerModels[0];
            var curr = playerModels[index];
            
            prev.SetActive(false);
            curr.SetActive(true);
            
            animator = curr.GetComponent<Animator>();
            
            CurrentPlayerModelIndex = index;
        }

        public void ResetPoints()
        {
            GoldPoint = 0;
            RenownPoint = 0;
        }
        
        private NetworkBool Ready { get; set; } = false;
        public bool IsReady => Ready;
        
        public void SetReady(bool ready)
        {
            Ready = ready;
        }
        
        #region RPC Methods...

        
        
        #endregion
    }
}
