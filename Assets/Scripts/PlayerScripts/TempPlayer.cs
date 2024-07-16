using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;

namespace LegalThieves
{
    public class TempPlayer : NetworkBehaviour
    {
        [Header("Components")]
        [SerializeField] private SkinnedMeshRenderer[] modelParts;
        [SerializeField] private KCC                   kcc;
        [SerializeField] private KCCProcessor          sprintProcessor;
        [SerializeField] private KCCProcessor          crouchProcessor;
            [SerializeField] private Transform             camTarget;
            [SerializeField] private AudioSource           source;                            //점프 사운드 - 제거 or 변경 예정
        [SerializeField] public static Animator              animator;

        [Header("Setup")]
        [SerializeField] private float                 maxPitch        = 85f;                   //현재 최대 피치에서 싱크가 맞지않음
        [SerializeField] private float                 lookSensitivity = 0.15f;
        [SerializeField] private Vector3               jumpImpulse     = new(0f, 5f, 0f);
        [SerializeField] private float                 maxHealth       = 100f;
        [SerializeField] private float                 maxStemina      = 100f;
        [field: SerializeField] public float           AbilityRange { get; private set; } = 5f;
        
        public double  Score => Math.Round(transform.position.y, 1);        //스코어 제거 or 변경 예정

        public GoldOrRenown EPlayerWinPoint;
        [Networked] public int goldPoint { get; set; }
        [Networked] public int renownPoint { get; set; }
        [Networked] public int remainPoint { get; set; }
        [Networked] public int maxPoint { get; set; }

        public bool    isReady;                                             //준비 기준 변경 예정 (GameLogic)
        public int[]   playerBoxItems = Enumerable.Repeat(-1, 30).ToArray();
        
        private bool CanSprint => kcc.FixedData.IsGrounded;
    
        private InputManager  _inputManager;
        private Vector2       _baseLookRotation;
        private int[]         _inventoryItems = Enumerable.Repeat(-1, 10).ToArray();
        
        private static readonly int AnimMoveDirX     = Animator.StringToHash("MoveDirX");
        private static readonly int AnimMoveDirY     = Animator.StringToHash("MoveDirY");
        private static readonly int AnimIsCrouching  = Animator.StringToHash("IsCrouching");
        RaycastHit hit;
        [Networked] public string  Name           { get; private set; }
        [Networked] public bool    IsSprinting    { get; private set; }
        [Networked] public bool    IsCrouching    { get; private set; }
        //[Networked] public float   CurrentHealth  { get; private set; }
        //[Networked] public float   CurrentStamina { get; private set; }
        
        //fusion 홈페이지 Network Tick <<< 이거 보면됨
        
        [Networked] private NetworkButtons  PreviousButtons  { get; set; }
        [Networked, OnChangedRender(nameof(Jumped))] private int JumpSync { get; set; }
    
        #region Overrided user callback functions in NetworkBehaviour

        public override void Spawned()
        {

            animator ??= GetComponentInChildren<Animator>();
        
            if(HasInputAuthority)
            {
                //입력된 스킨드메쉬를 안보이게 하는 부분.
                foreach (var skinnedMeshRenderer in modelParts)
                    skinnedMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                
                _inputManager = Runner.GetComponent<InputManager>();
                _inputManager.localTempPlayer = this;
                
                Name = PlayerPrefs.GetString("Photon.Menu.Username");
                RPC_PlayerName(Name);
                
                CameraFollow.Singleton.SetTarget(camTarget);
                UIManager.Singleton.localTempPlayer = this;
                kcc.Settings.ForcePredictedLookRotation = true;
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if(HasInputAuthority)
                UIManager.Singleton.localTempPlayer = null;
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetInput input))
            {
                CheckSprint(input);
                CheckJump(input);
                CheckCrouch(input);
                TryInteraction(input);
                CheckThrowItem(input);
                
                if(IsSprinting && !CanSprint)
                    ToggleSprint(false);

                _baseLookRotation = kcc.GetLookRotation();
                kcc.AddLookRotation(input.LookDelta * lookSensitivity, -maxPitch, maxPitch);
                UpdateCamTarget();
            
                SetInputDirection(input);
            
                PreviousButtons = input.Buttons;
            }
        }

        public override void Render()
        {
            if (kcc.Settings.ForcePredictedLookRotation)
            {
                var predictedLookRotation = _baseLookRotation + _inputManager.AccumulatedMouseDelta * lookSensitivity;
                kcc.SetLookRotation(predictedLookRotation);
            }
            UpdateCamTarget();

            var moveVelocity = GetAnimationMoveVelocity();
            animator.SetFloat(AnimMoveDirX, moveVelocity.x, 0.05f, Time.deltaTime);
            animator.SetFloat(AnimMoveDirY, moveVelocity.z, 0.05f, Time.deltaTime);

            animator.SetBool(AnimIsCrouching, IsCrouching);
        }

        #endregion
    
        #region Player Movement
        
        public void Teleport(Vector3 position, Quaternion rotation)
        {
            kcc.SetPosition(position);
            kcc.SetLookRotation(rotation);
        }
        
        private void SetInputDirection(NetInput input)
        {
            var worldDirection = kcc.FixedData.TransformRotation * input.Direction.X0Y();
        
            kcc.SetInputDirection(worldDirection);
        }

        private void CheckJump(NetInput input)
        {
                
            if (input.Buttons.WasPressed(PreviousButtons, EInputButton.Jump) && kcc.FixedData.IsGrounded)
            {
                kcc.Jump(jumpImpulse);
                JumpSync++;
                animator.SetBool("isJump", true);
            }          
            else if (GetAnimationMoveVelocity().y < 0)
            {
                Physics.Raycast(transform.position, Vector3.down, out hit, 5.5f);
                if (hit.collider != null)
                {
                    animator.SetBool("isJump", false); ;
                }
            }           
        }

        private void CheckSprint(NetInput input)
        {
            if (input.Buttons.WasPressed(PreviousButtons, EInputButton.Sprint) && CanSprint)
                ToggleSprint(true);
            else if(input.Buttons.WasReleased(PreviousButtons, EInputButton.Sprint) && IsSprinting)
                ToggleSprint(false);
        }

        private void CheckCrouch(NetInput input)
        {
            if (input.Buttons.WasPressed(PreviousButtons, EInputButton.Crouch) && kcc.Data.IsGrounded)
            {
                ToggleCrouch(true);
                ToggleSprint(false);
            }
            else if(input.Buttons.WasReleased(PreviousButtons, EInputButton.Crouch) && IsCrouching)
            {
                ToggleCrouch(false);
            }
        }

        private void ToggleSprint(bool isSprinting)
        {
            if (IsSprinting == isSprinting)
                return;

            if (isSprinting)
            {
                kcc.AddModifier(sprintProcessor);
                var velocity = kcc.Data.DynamicVelocity;
                velocity.y *= 0.25f;
                kcc.SetDynamicVelocity(velocity);
            }
            else
            {
                kcc.RemoveModifier(sprintProcessor);
            }

            IsSprinting = isSprinting;
        }

        private void ToggleCrouch(bool isCrouching)
        {
            if (IsCrouching == isCrouching)
                return;

            if (isCrouching)
            {
                kcc.SetHeight(1f);
                kcc.AddModifier(crouchProcessor);
                camTarget.localPosition = new Vector3(0f, 1f, 0f);  //러프 적용해아됨
                IsCrouching = true;
            }
            else
            {
                kcc.SetHeight(1.8f);
                kcc.RemoveModifier(crouchProcessor);
                camTarget.localPosition = new Vector3(0, 1.65f, 0f);
                IsCrouching = false;
            }
        }
        
        #endregion
        
        public void ResetCooldown()
        {
            
        }
        
        //플레이어 상호작용 확인. NetInput Interaction F키를 눌러 호출됨.
        private void TryInteraction(NetInput input)
        {
            if(!input.Buttons.WasPressed(PreviousButtons, EInputButton.Interaction))
                return;

            
            if (!Physics.Raycast(camTarget.position, camTarget.forward, out var hitInfo, AbilityRange)) return;
            if (hitInfo.collider.TryGetComponent(out RelicDisplayer Table))
            {
                var index = Table.AddRelics(_inventoryItems[UIManager.Singleton.currentSlotIndex], this);
                if (index == -1) return;
                var selectedItemIndex = _inventoryItems[UIManager.Singleton.currentSlotIndex];
                var tempRelic = RelicManager.Singleton.GetTempRelicWithIndex(selectedItemIndex);
                _inventoryItems[UIManager.Singleton.currentSlotIndex] = -1;
                UIManager.Singleton.SetSlotImage(false);
                tempRelic.SpawnRelic(Table.GetRelicPosition(index), Quaternion.Euler(Vector3.zero), Vector3.zero);
                return;
            }
            if (_inventoryItems[UIManager.Singleton.currentSlotIndex] != -1) return;
            if (hitInfo.collider.TryGetComponent(out TempRelic relic))
            {
                _inventoryItems[UIManager.Singleton.currentSlotIndex] = relic.relicNumber;
                UIManager.Singleton.SetSlotImage(true, relic.relicSprite);
                relic.GetRelic(this);
            }
        }
        
        //아이템 버리기 체크 현재 G키를 눌러 _inventoryItems배열 마지막 요소를 버리게 되어있음.
        //NetInput의 ThrowItem을 통해 TempPlayer의 FixedUpdateNetwork함수에서 호출됨.
        private void CheckThrowItem(NetInput input)
        {
            if(!input.Buttons.WasPressed(PreviousButtons, EInputButton.ThrowItem) ||
               _inventoryItems[UIManager.Singleton.currentSlotIndex] == -1) 
                return;

            var selectedItemIndex = _inventoryItems[UIManager.Singleton.currentSlotIndex];
            var tempRelic = RelicManager.Singleton.GetTempRelicWithIndex(selectedItemIndex);
            _inventoryItems[UIManager.Singleton.currentSlotIndex] = -1;
            UIManager.Singleton.SetSlotImage(false);
            tempRelic.SpawnRelic(camTarget.position, camTarget.rotation, camTarget.forward);
        }

        private void UpdateCamTarget()
        {
            camTarget.localRotation = Quaternion.Euler(kcc.GetLookRotation().x, 0f, 0f);
        }

        private void Jumped()
        {
            source.Play();
        }
        
        private Vector3 GetAnimationMoveVelocity()
        {
            if (kcc.Data.RealSpeed < 0.01f)
                return default;

            var velocity = kcc.Data.RealVelocity;

            if (velocity.sqrMagnitude > 1f)
            {
                velocity.Normalize();
            }

            return transform.InverseTransformVector(velocity);
        }
        
        #region RPC Callback
    
        [Rpc(RpcSources.InputAuthority, RpcTargets.InputAuthority | RpcTargets.StateAuthority)]
        public void RPC_SetReady()
        {
            isReady = true;
            if(HasInputAuthority) UIManager.Singleton.DidSetReady();
        }
    
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_PlayerName(string playerName)
        {
            Name = playerName;
        }
    
        #endregion
    }
}