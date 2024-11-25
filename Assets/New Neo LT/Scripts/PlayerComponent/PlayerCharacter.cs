using System;
using System.Linq;
using Fusion;
using Fusion.Addons.KCC;
using System.Collections;
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
using UnityEngine.InputSystem;
using System.Runtime.CompilerServices;

public enum Job { Null = -1, Archaeologist, Linguist , BusinessCultist , Shamanist, max }

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
        [SerializeField] private Item_Torch_Temp[] TorchScript;
        [SerializeField] private Item_Flash_Temp[] FlashScript;


                [Networked, OnChangedRender(nameof(OnRefChanged))] 
        public PlayerRef          Ref        { get; set; }
        [Networked] 
        public byte               Index      { get; set; }
        
        [Networked, OnChangedRender(nameof(OnColorChanged))]
        private int               PlayerColor { get; set; }
        [Networked, OnChangedRender(nameof(OnCurrentPlayerModelIndexChanged))] 
        private int               CurrentPlayerModelIndex { get; set; }

        [Networked, OnChangedRender(nameof(OnPlayerJobChanged))]
        Job job { get; set; }
        [Networked]
        public bool               IsScholar { get; set; }
        [Networked, OnChangedRender(nameof(OnPointChanged))]
        private int               RenownPoint { get; set; }
        [Networked, OnChangedRender(nameof(OnPointChanged))]
        private int               GoldPoint { get; set; }

        [Networked, Capacity(10), OnChangedRender(nameof(OnInventoryChanged))]
        public NetworkArray<int> Inventory => default;

        [Networked, OnChangedRender(nameof(OnTorchChanged))]
        private bool _isPikedTorch { get; set; }


        [Networked, OnChangedRender(nameof(OnTorchStateChanged))]
        private bool IsTorchVisibility { get; set; }

        [Networked, OnChangedRender(nameof(OnFlashChanged))]
        private bool _isPikedFlash { get; set; }


        [Networked, OnChangedRender(nameof(OnFlashStateChanged))]
        private bool IsFlashVisibility { get; set; }

        private bool canPickItem;
    
        [Networked] 
        private float             CrouchSync { get; set; } = 1f;
        
     
        private NetworkButtons    _previousButtons;
        private Vector2           _accumulatedMouseDelta;
        
        [Networked, Capacity(24), OnChangedRender(nameof(OnNicknameChanged))]
        private string            Nickname { get => default; set { }}

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
        private static readonly int AnimPickTorch     = Animator.StringToHash("pickTorch");
        private static readonly int AnimPickFlash     = Animator.StringToHash("pickFlash");
        #endregion

        /*------------------------------------------------------------------------------------------------------------*/

        #region NetworkBehaviour Events...

        private void Start()
        {
            Client_InitPlayerModel(CurrentPlayerModelIndex);
            
            if(Nickname != null)
                SetPlayerTag(Nickname);
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
                job = Job.Null;

                for (var i = 0; i < Inventory.Length;i++)
                {
                    Inventory.Set(i, -1);
                }

                if (PlayerRegistry.Instance != null && PlayerRegistry.Count >= 4)
                {
                    for (int i = 0; i < NewGameManager.Instance.ButtonStateArray.Length; i++)
                    {
                        NewGameManager.Instance.EnableJobButton(i);
                    }
                }
            }

            if (Object.HasInputAuthority)
            {
                Local = this;
                InitializePlayerComponents();
                UIManager.Instance.InitializeInGameUI();
                InitModels();

                RPC_SetPlayerNickname(Runner.LocalPlayer, PlayerPrefs.GetString("Photon.Menu.Username"));
            }
            
            UIManager.Instance.playerListController.PlayerJoined(this);


            InitializeCharacterComponents();
            InitializePlayerNetworkedProperties();
            
            if (Object.HasInputAuthority)
            {
                UIManager.Instance.compassRotate.SetPlayerTransform(transform);
                UIManager.Instance.stateLoadingUI.SetYPos();
                UIManager.Instance.jobChangerUI.gameObject.SetActive(true);
                UIManager.Instance.jobChangerUI.JobChangerOpen(Object.InputAuthority,NewGameManager.Instance.ButtonStateArray.ToArray());
            }
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
            Nickname = playerName;
        }
        
        public string GetPlayerName()
        {
            return Nickname;
        }
        
        private void OnNicknameChanged()
        {
            SetPlayerTag(Nickname);
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
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.WheelUp))
                OnMouseWheelUp();
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.WheellDown))
                OnMouseWheelDown();

            
            // Set behavior by Keyboard input
            // Sprint
            SprintToggle(playerInput);

            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Interaction3))
                TorchToggle(playerInput);
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Interaction4))
                FlashToggle(playerInput);
            //CrouchToggle(playerInput);
            // if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Sprint) && CanSprint)
            //     kcc.FixedData.KinematicSpeed = characterStats.SprintSpeed;
            // if (playerInput.Buttons.WasReleased(_previousButtons, EInputButton.Sprint))
            //     kcc.FixedData.KinematicSpeed = characterStats.MoveSpeed;

            // Jump
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Jump) && CanJump)
                OnJumpButtonPressed();
            // Crouch
            //getRelic
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Interaction2))
                CheckInteraction();
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Inventory))
                ToggleInventory();
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

        private void TorchToggle(NetInput input)
        {
            if (IsFlashVisibility && _isPikedFlash) return;

            if (!IsTorchVisibility)
            {
                _isPikedTorch = true;
                IsTorchVisibility = true;
                TorchScript[CurrentPlayerModelIndex].gameObject.SetActive(IsTorchVisibility);
              //  AudioManager.instance.PlayLoop(ESoundType.TorchIdle);
                TorchScript[CurrentPlayerModelIndex].TurnOnLight();
            }
            else
            {
             //   AudioManager.instance.Stop(ESoundType.TorchIdle) ;
                _isPikedTorch = false;
                StartCoroutine(TorchTurnOff());
                TorchScript[CurrentPlayerModelIndex].TurnOffLight();
            }
        }

        public IEnumerator TorchTurnOff()
        {
            yield return 1f; // 이거 이상함 너무 빨리 꺼짐;
            IsTorchVisibility = false;
        }
        private void FlashToggle(NetInput input)
        {
            if (IsTorchVisibility && _isPikedTorch) return;

            if (!IsFlashVisibility)
            {
             //   AudioManager.instance.PlaySound(ESoundType.FlashOn);

                _isPikedFlash = true;
                IsFlashVisibility = true;
                FlashScript[CurrentPlayerModelIndex].gameObject.SetActive(IsFlashVisibility);
                FlashScript[CurrentPlayerModelIndex].TurnOnLight();
            }
            else
            {
                _isPikedFlash = false;
                StartCoroutine(FlashTurnOff());
                FlashScript[CurrentPlayerModelIndex].TurnOffLight();
            }
        }
        public IEnumerator FlashTurnOff()
        {
        //   AudioManager.instance.PlaySound(ESoundType.FlashOff);
            yield return 1f; // 이거 이상함 너무 빨리 꺼짐;
            IsFlashVisibility = false;
        }



        //private void CrouchToggle(NetInput input)
        //{

        //    if (input.Buttons.WasPressed(_previousButtons, EInputButton.Crouch))
        //    {

        //        kcc.AddModifier(kccProcessors[2]);
        //        CrouchSync = 0;
        //    }
        //    if (input.Buttons.WasReleased(_previousButtons, EInputButton.Crouch))
        //    {
        //        kcc.AddModifier(kccProcessors[2]);
        //        CrouchSync = 1;
        //    }

        //}

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

        private void OnMouseWheelUp()
        {
            Debug.Log("1");
            SelectSlot(slotIndex - 1 < 0 ? 0 : slotIndex - 1);
            Debug.Log("2");
        }
        private void OnMouseWheelDown()
        {
            Debug.Log("3");
            SelectSlot(slotIndex + 1 > 9 ? 9 : slotIndex + 1);
            Debug.Log("4");
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

            UIManager.Instance.inventorySlotController.SetSlotPoint(Inventory[index]);
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

        public void ToggleInventory()
        {
            if (HasInputAuthority)
            {
                ToggleInventory1(Object.InputAuthority);
            }
        }

        public void ToggleInventory1(PlayerRef player)
        {
            UIManager.Instance.inventorySlotController.OnToggleInventory();
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
            UIManager.Instance.inventorySlotController.SetSlotPoint(Inventory[slotIndex]);
            UIManager.Instance.relicPriceUI.SetTotalPoint(Inventory.ToArray());
            UIManager.Instance.RelicScanUI.SetUIPoint(-1);
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
            ChangePlayerModel(((int)job));
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
        void OnTorchChanged()
        {
            animator.SetBool(AnimPickTorch, _isPikedTorch);
           
        }
        private void OnTorchStateChanged()
        {
            TorchScript[CurrentPlayerModelIndex].gameObject.SetActive(IsTorchVisibility);
        }
        void OnFlashChanged()
        {
            animator.SetBool(AnimPickFlash, _isPikedFlash);
        }
        private void OnFlashStateChanged()
        {
            FlashScript[CurrentPlayerModelIndex].gameObject.SetActive(IsFlashVisibility);
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

        private void Client_InitPlayerModel(int index)
        {
            var prev = playerModels[0];
            var curr = playerModels[index];
            
            prev.SetActive(false);
            curr.SetActive(true);
            
            animator = curr.GetComponent<Animator>();
            
            CurrentPlayerModelIndex = index;
        }
        
        public void SetPlayerTag(string pTag)
        {
            playerNickname.text = pTag;
        }
        
        public int GetJobIndex()
        {
            return (int)job;
        }

        public void ResetPoints()
        {
            GoldPoint = 0;
            RenownPoint = 0;
        }

        [Networked]
        private NetworkBool Ready { get; set; } = false;
        public bool IsReady => Ready;
        
        public void SetReady(bool ready)
        {
            Ready = ready;
            
            if(!HasStateAuthority)
                return;
            
            NewGameManager.Instance.StartGame();
        }

        #region RPC Methods...
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
        public void RPC_SetPlayerNickname(PlayerRef player, string nickname)
        {
            var playerCharacter = PlayerRegistry.GetPlayer(player);
            playerCharacter.SetPlayerName(nickname);
        }

        #endregion
    }
}
