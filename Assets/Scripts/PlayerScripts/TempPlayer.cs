using System;
using System.Collections.Generic;
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
        [SerializeField] private Rigidbody  rigid;
        [Header("Setup")]
        [SerializeField] private float                 maxPitch        = 85f;                   //현재 최대 피치에서 싱크가 맞지않음
        [SerializeField] private float                 lookSensitivity = 0.15f;
        [SerializeField] private Vector3               jumpImpulse     = new(0f, 5f, 0f);
        [SerializeField] private float                 maxHealth       = 100f;
        [SerializeField] private float                 maxStemina      = 100f;
        [field: SerializeField] public float           AbilityRange { get; private set; } = 25f;
        
        public double  Score => Math.Round(transform.position.y, 1);        //스코어 제거 or 변경 예정
        public bool    isReady;                                             //준비 기준 변경 예정 (GameLogic)
        private bool CanSprint => kcc.FixedData.IsGrounded;
        private Vector3 MoveVelocity;
        private InputManager  _inputManager;
        private Vector2       _baseLookRotation;
        private List<uint>    _inventoryItems = new();
        
        private static readonly int AnimMoveDirX     = Animator.StringToHash("MoveDirX");
        private static readonly int AnimMoveDirY     = Animator.StringToHash("MoveDirY");

        private static readonly int AnimIsJumping = Animator.StringToHash("IsJumping");
        private static readonly int AnimIsCrouching  = Animator.StringToHash("IsCrouching");

        [Networked] public string  Name           { get; private set; }
        [Networked] public bool    IsSprinting    { get; private set; }
        [Networked] public bool    IsCrouching    { get; private set; }
        [Networked] public bool IsJumping { get; private set; }
        //[Networked] public float   CurrentHealth  { get; private set; }
        //[Networked] public float   CurrentStamina { get; private set; }

        [Networked] private NetworkButtons  PreviousButtons  { get; set; }
        
        [Networked, OnChangedRender(nameof(Jumped))] private int JumpSync { get; set; }
        
    
        #region Overrided user callback functions in NetworkBehaviour

        public override void Spawned()
        {

            animator ??= GetComponentInChildren<Animator>();
        
            if(HasInputAuthority)
            {
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
                kcc.AddLookRotation(input.LookDelta * lookSensitivity, -maxPitch, maxPitch);
                UpdateCamTarget();
            
                if(input.Buttons.WasPressed(PreviousButtons, EInputButton.Interaction))
                    TryInteraction(camTarget.forward);

                if (!CanSprint)
                {
                    if(IsSprinting)
                        ToggleSprint(false);
                }
                    
            
                SetInputDirection(input);
            
                PreviousButtons = input.Buttons;
                _baseLookRotation = kcc.GetLookRotation();
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
            
            MoveVelocity = GetAnimationMoveVelocity();            
            animator.SetFloat(AnimMoveDirX, MoveVelocity.x, 0.05f, Time.deltaTime);
            animator.SetFloat(AnimMoveDirY, MoveVelocity.z, 0.05f, Time.deltaTime);

            animator.SetBool(AnimIsJumping, IsJumping);
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
            if (input.Buttons.WasPressed(PreviousButtons, EInputButton.Jump) && kcc.Data.IsGrounded)
            {
                kcc.Jump(jumpImpulse);
                JumpSync++;
                animator.SetBool("isJump", true);

            }
            else if (MoveVelocity.y < 0.5f)
            {
                animator.SetBool("isJump", false);
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
        
        private void TryInteraction(Vector3 lookDirection)
        {
            if (Physics.Raycast(camTarget.position, lookDirection, out RaycastHit hitInfo, AbilityRange))
            {
                if (hitInfo.collider.TryGetComponent(out TempRelic relic))
                {
                    _inventoryItems.Add(relic.relicNumber);
                    relic.GetRelic(this);
                }
            }
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

        private void CheckThrowItem(Vector3 lookDirection)
        {
            if(_inventoryItems.Count == 0) return;
            
        }
        
        //private Vector3
        
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