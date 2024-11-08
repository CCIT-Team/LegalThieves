using Fusion;
using Fusion.Addons.KCC;
using LegalThieves;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.Player_Input;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;
using System.Collections;
using EInputButton = New_Neo_LT.Scripts.Player_Input.EInputButton;
using NetInput = New_Neo_LT.Scripts.Player_Input.NetInput;
using RelicManager = LegalThieves.RelicManager;
using UIManager = New_Neo_LT.Scripts.UI.UIManager;

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
        
        [Networked, OnChangedRender(nameof(OnColorChanged))]
        private int               PlayerColor { get; set; }
        
        [Networked, OnChangedRender(nameof(OnPlayerJobChanged))]
        public bool IsScholar { get; set; }
        [Networked, OnChangedRender(nameof(OnPointChanged))]
        private int               RenownPoint { get; set; }
        [Networked, OnChangedRender(nameof(OnPointChanged))]
        private int               GoldPoint { get; set; }
        
        [Networked, OnChangedRender(nameof(NicknameChanged))] 
        public NetworkString<_16> Nickname   { get; set; }
        
        [Networked, Capacity(10), OnChangedRender(nameof(OnInventoryChanged))] 
        private NetworkArray<int> Inventory => default;
        
        private int[]             _prevInventory = new int[10];

        [Networked] 
        private float             CrouchSync { get; set; } = 1f;

       
        private bool              IsPickTorch { get; set; }

        private NetworkButtons    _previousButtons;
        private Vector2           _accumulatedMouseDelta;
        
        private bool              _isSprinting;
        private bool              CanJump   => kcc.FixedData.IsGrounded;
        private bool              CanSprint => kcc.FixedData.IsGrounded && characterStats.CurrentStamina > 0;


        [SerializeField] private GameObject itemTorch;
        [SerializeField] private Item_Torch_Temp Torch;
        private Coroutine torchCoroutine = null;
        private bool isTorchProcessing = false; // 코루틴 실행 상태 플래그

        [SerializeField] private int slotIndex = 0;

        private RaycastHit      _rayCastHit;
        
        public static PlayerCharacter Local { get; set; }

        #region Animation Hashes...

        private static readonly int AnimMoveDirX      = Animator.StringToHash("MoveDirX");
        private static readonly int AnimMoveDirY      = Animator.StringToHash("MoveDirY");
        private static readonly int AnimIsCrouchSync  = Animator.StringToHash("IsCrouchSync");
        private static readonly int AnimLookPit       = Animator.StringToHash("LookPit");
        private static readonly int AnimJumpTrigger   = Animator.StringToHash("Jump");
        private static readonly int AnimTorch         = Animator.StringToHash("pickTorch");
        #endregion

        /*------------------------------------------------------------------------------------------------------------*/

        #region NetworkBehaviour Events...

        public override void Spawned()
        {
            base.Spawned();

            if (Object.HasStateAuthority)
            {
                PlayerRegistry.Server_Add(Runner, Object.InputAuthority, this);

                PlayerColor = 0;
                IsScholar = false;
                for(var i = 0; i < 10; i++)
                {
                    Inventory.Set(i, -1);
                }
            }

            if (Object.HasInputAuthority)
            {
                Local = this;
                UIManager.Instance.SetLocalPlayerTransform(transform);
                InitializePlayerComponents();
            }
            
            UIManager.Instance.scoreRankUI.JoinedPlayer(Ref);
            
            InitializeCharacterComponents();
            InitializePlayerNetworkedProperties();
            
            
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
            // if(kcc.Settings.ForcePredictedLookRotation) // 화면 끊김 발생해서 뺌
            kcc.SetLookRotation(kcc.GetLookRotation() + _accumulatedMouseDelta * lookSensitivity);
            camTarget.localRotation = Quaternion.Euler(kcc.GetLookRotation().x, 0f, 0f);
            
            var moveVelocity = GetAnimationMoveVelocity();
            animator.SetFloat(AnimMoveDirX    , moveVelocity.x, 0.05f, Time.deltaTime);
            animator.SetFloat(AnimMoveDirY    , moveVelocity.z * 2, 0.05f, Time.deltaTime);
            animator.SetFloat(AnimIsCrouchSync, CrouchSync);
            animator.SetFloat(AnimLookPit     , kcc.FixedData.LookPitch);
            animator.SetBool(AnimTorch        , IsPickTorch);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            
            UIManager.Instance.scoreRankUI.LeftPlayer(Ref);
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
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Sprint) && CanSprint)
                ToggleSprint(true);
            else if (playerInput.Buttons.WasReleased(_previousButtons, EInputButton.Sprint) && _isSprinting)
                ToggleSprint(false);

            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Crouch) && kcc.Data.IsGrounded)
            {
                ToggleCrouch(true);
                ToggleSprint(false);
            }
            else if (playerInput.Buttons.WasReleased(_previousButtons, EInputButton.Crouch) && _isSprinting)
            {
                ToggleCrouch(false);
            }

            //torch
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Interaction3))
                CheckInteractionQ();
            // Jump
            if (playerInput.Buttons.WasPressed(_previousButtons, EInputButton.Jump)&& CanJump)
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

            if (_isSprinting && !CanSprint)
                ToggleSprint(false);

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
  

        private void ToggleSprint(bool isSprinting)
        {
            if (_isSprinting == isSprinting)
                return;

            if (isSprinting)
            {
                kcc.AddModifier(kccProcessors[1]);
                var velocity = kcc.Data.DynamicVelocity;
                velocity.y *= 0.25f;
                kcc.SetDynamicVelocity(velocity);
            }
            else
            {
                kcc.RemoveModifier(kccProcessors[1]);
            }
            _isSprinting = isSprinting;
        }

        private void ToggleCrouch(bool isCrouching)
        {
            if (_isSprinting == isCrouching)
                return;

            if (isCrouching)
            {
                kcc.SetHeight(1f);
                kcc.AddModifier(kccProcessors[2]);
                _isSprinting = true;
            }
            else
            {
                kcc.SetHeight(1.6f);
                kcc.RemoveModifier(kccProcessors[2]);
                _isSprinting = false;
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

        private void CheckInteractionQ()
        {
            if (torchCoroutine != null)
            {
                StopCoroutine(torchCoroutine); // 실행 중인 코루틴이 있으면 중지
            }

            if (!IsPickTorch)
            {
                torchCoroutine = StartCoroutine(WaitOn());
            }
            else
            {
                torchCoroutine = StartCoroutine(WaitOff());
            }
        }

        private IEnumerator WaitOn()
        {
            Debug.Log("횃불 ON");
            RpcSetTorchState(true);

            yield return new WaitForSeconds(1f);
            Torch.TurnOnLight();
            IsPickTorch = true;
            torchCoroutine = null; // 코루틴 완료 후 null로 설정
        }

        private IEnumerator WaitOff()
        {
           
            IsPickTorch = false;
            yield return new WaitForSeconds(1.0f);
            Torch.TurnOffLight();
            yield return new WaitForSeconds(1.0f);
            itemTorch.SetActive(false);
            torchCoroutine = null; // 코루틴 완료 후 null로 설정
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
                SetPreviousInventory();
                return true;
            }

            // 빈 슬롯을 찾아 아이템 추가
            for (var i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] != -1) 
                    continue;
                
                Inventory.Set(i, relicId);
                SetPreviousInventory();
                return true;
            }
            
            return false;
        }

        private void SetPreviousInventory()
        {
            _prevInventory = Inventory.ToArray();
        }
        
        public void ThrowRelic()
        {
            if (HasStateAuthority && Inventory[slotIndex] != -1) // 서버에서만 실행
            {
                RelicManager.Instance.GetRelicData(Inventory[slotIndex]).OnThrowAway(Object.InputAuthority);
                Inventory.Set(slotIndex, -1); // 인벤토리에서 제거
                SetPreviousInventory();
            }
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
        }
        
        public int[] GetInventorys()
        {
            return Inventory.ToArray();
        }

        public void SetPlayerColor(int index) => PlayerColor = index;
        public int GetPlayerColor() => PlayerColor;

        private void OnColorChanged()
        {
            Material[] materials = skinnedMeshRenderer.materials; 
            materials[1] = NewGameManager.Instance.playerClothMaterials[PlayerColor];
            materials[4] = NewGameManager.Instance.playerHairMaterials[PlayerColor];
            skinnedMeshRenderer.materials = materials;
        }
        
        public void ChangeJob()
        {
            IsScholar = !IsScholar;
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

        private void OnPlayerJobChanged()
        {
            UIManager.Instance.scoreRankUI.SetPlayerJob(Ref.PlayerId, IsScholar);
        }
        
        private void OnPointChanged()
        {
            UIManager.Instance.scoreRankUI.PlayerScoreSet(Ref, IsScholar ? RenownPoint : GoldPoint);
        }

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
        
        
        #endregion
        #region RPC Methods...

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_AddPoint(int gold, int renown, PlayerRef player)
        {
            GoldPoint += gold;
            RenownPoint += renown;
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        private void RpcSetTorchState(bool state)
        {
            IsPickTorch = state;
            itemTorch.SetActive(state);
        }

        #endregion
    }
}
