using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace LegalThieves
{
    public class TempPlayer : NetworkBehaviour
    {
        [Header("Components")]
        [SerializeField] private SkinnedMeshRenderer[] modelParts;
        [SerializeField] private Material[] clothMaterials;
        [SerializeField] private Material[] hairMaterials;
        [SerializeField] private KCC                   kcc;
        [SerializeField] private KCCProcessor          sprintProcessor;
        [SerializeField] private KCCProcessor          crouchProcessor;
        [SerializeField] private Transform             camTarget;
        [SerializeField] private Animator              _animator;

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
        public int[]         _inventoryItems = Enumerable.Repeat(-1, 10).ToArray();
        [SerializeField] private Item_Torch_Temp Torch;
     

        private static readonly int AnimMoveDirX     = Animator.StringToHash("MoveDirX");
        private static readonly int AnimMoveDirY     = Animator.StringToHash("MoveDirY");
        private static readonly int AnimIsCrouching  = Animator.StringToHash("IsCrouchSync");
        private static readonly int LookPit          = Animator.StringToHash("LookPit");
        private static readonly int Jump             = Animator.StringToHash("Jump");
        private static readonly int pickTorch        = Animator.StringToHash("pickTorch");
        private static readonly int Attack           = Animator.StringToHash("Attack");
        private static readonly int Victory = Animator.StringToHash("Victory");
        private static readonly int Defeat = Animator.StringToHash("Defeat");

        RaycastHit hit;
        [Networked] public string  Name           { get; private set; }
        [Networked] public bool    IsSprinting    { get; private set; }
        [Networked] private bool   IsCrouching    { get; set; }
        [Networked] public bool    IsHoldingItem  { get; private set; }
        [Networked] public bool    PickTorch  { get; private set; }
        
        //[Networked] public float   CurrentHealth  { get; private set; }
        //[Networked] public float   CurrentStamina { get; private set; }

        //fusion 홈페이지 Network Tick <<< 이거 보면됨

        [Networked] private NetworkButtons  PreviousButtons  { get; set; }
        [Networked, OnChangedRender(nameof(Jumped))] private int JumpSync { get; set; }
        [Networked, OnChangedRender(nameof(Attacked))] private int AttackSync { get; set; }
        [Networked] private float CrouchSync { get; set; }

        [SerializeField] private GameObject ItemTorch;
        #region Overrided user callback functions in NetworkBehaviour

        public override void Spawned()
        {

            _animator ??= GetComponentInChildren<Animator>();
            
            UIManager.Singleton.ResetHUD();
            IsHoldingItem = false;
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
                CheckHoldingItem(input);
                CheckAttack(input);
                CheckVictory(input);
                CheckDefeat(input);
                //CheckTorch(input);
                
                if(IsSprinting && !CanSprint)
                    ToggleSprint(false);

                _baseLookRotation = kcc.GetLookRotation();
                kcc.AddLookRotation(input.LookDelta * lookSensitivity, -maxPitch, maxPitch);
                UpdateCamTarget();
            
                SetInputDirection(input);

                PlayWalkingSounds(input);

                PreviousButtons = input.Buttons;
            }
        }
        bool walkingSounds = false; // 클래스 멤버로 선언
        


        private void PlayWalkingSounds(NetInput input)
        {
            if (IsSprinting)
                return;

            //// 걷는 소리가 이미 재생 중이고, 아무 키도 눌리지 않았을 경우 소리 중지
            //if (!walkingSounds && input.Direction.sqrMagnitude == 0)
            //{
            //    AudioManager.instance.PlayBreathSfx(false);
            //    AudioManager.instance.PlayHRGFSfx(false);
            //    AudioManager.instance.PlayHRDFSfx(false);
            //    AudioManager.instance.PlayDFSfx(false);
            //    AudioManager.instance.PlayGFSfx(false);
            //    walkingSounds = false; // 소리 재생 상태 해제
            //    return;
            //}

            // WASD 키 입력이 있는지 확인하고 소리를 재생
            if (input.Direction.sqrMagnitude > 0 && !walkingSounds)
            {
                walkingSounds = true; // 소리 재생 상태로 설정
                if (CaveJungleBGM.cavein == 1)
                {
                    // 동굴 안에서의 사운드
                    AudioManager.instance.PlayGFSfx(true);
                }
                else
                {
                    // 동굴 밖에서의 사운드
                    AudioManager.instance.PlayDFSfx(true);
                }
            }
            else 
            {
                if (walkingSounds && input.Direction.sqrMagnitude == 0)
                {
                    AudioManager.instance.PlayBreathSfx(false);
                    AudioManager.instance.PlayHRGFSfx(false);
                    AudioManager.instance.PlayHRDFSfx(false);
                    AudioManager.instance.PlayDFSfx(false);
                    AudioManager.instance.PlayGFSfx(false);
                    walkingSounds = false; // 소리 재생 상태 해제
                    return;
                }
            }
        }

        ///private void StopWalkingSounds()
        //{
        //    // 모든 소리 중지
        //    AudioManager.instance.PlayBreathSfx(false);
        //    AudioManager.instance.PlayHRGFSfx(false);
        //    AudioManager.instance.PlayHRDFSfx(false);
        //    AudioManager.instance.PlayDFSfx(false);
        //    AudioManager.instance.PlayGFSfx(false);
        //    walkingSounds = false; // 소리 재생 상태 해제
        //}

        //private void PlayCaveSounds()
        //{
        //    AudioManager.instance.PlayBreathSfx(true);
        //    AudioManager.instance.PlayHRGFSfx(false);
        //    AudioManager.instance.PlayHRDFSfx(false);
        //    AudioManager.instance.PlayDFSfx(false);
        //    AudioManager.instance.PlayGFSfx(true);
        //}

        //private void PlayJungleSounds()
        //{
        //    AudioManager.instance.PlayBreathSfx(true);
        //    AudioManager.instance.PlayHRGFSfx(false);
        //    AudioManager.instance.PlayHRDFSfx(false);
        //    AudioManager.instance.PlayDFSfx(true);
        //    AudioManager.instance.PlayGFSfx(false);
        //}
        
        
        public override void Render()
        {
            if (kcc.Settings.ForcePredictedLookRotation)
            {
                var predictedLookRotation = _baseLookRotation + _inputManager.AccumulatedMouseDelta * lookSensitivity;
                kcc.SetLookRotation(predictedLookRotation);
            }
            UpdateCamTarget();
            
            
            
            var tempCamLocalPosY = camTarget.localPosition.y;
            if (IsCrouching)
            {
                if(tempCamLocalPosY > 1f)
                    camTarget.localPosition = new Vector3(0f, tempCamLocalPosY - 5f * Runner.DeltaTime, 0f);
            }
            else
            {
                if(tempCamLocalPosY < 1.6f)
                    camTarget.localPosition = new Vector3(0f, tempCamLocalPosY + 5f * Runner.DeltaTime, 0f);
            }

            CrouchSync = (tempCamLocalPosY - 1) * 1.53f;

            var moveVelocity = GetAnimationMoveVelocity();
            moveVelocity = IsSprinting ? moveVelocity * 2f : moveVelocity;
            _animator.SetFloat(AnimMoveDirX, moveVelocity.x, 0.05f, Time.deltaTime);
            _animator.SetFloat(AnimMoveDirY, moveVelocity.z, 0.05f, Time.deltaTime);
            _animator.SetFloat(AnimIsCrouching, CrouchSync);
            _animator.SetFloat(LookPit, -kcc.GetLookRotation(true, false).x / 90f);
           //_animator.SetBool(pickTorch, IsHoldingItem);

            //animator.Animator.SetFloat(AnimMoveDirX, moveVelocity.x, 0.05f, Time.deltaTime);
            //animator.Animator.SetFloat(AnimMoveDirY, moveVelocity.z, 0.05f, Time.deltaTime);
            //animator.Animator.SetBool(AnimIsCrouching, IsCrouching);
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
            if (!input.Buttons.WasPressed(PreviousButtons, EInputButton.Jump) || !kcc.FixedData.IsGrounded) return;
            kcc.Jump(jumpImpulse);
            JumpSync++;
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

        private void CheckHoldingItem(NetInput input)
        {
            if (input.Buttons.WasPressed(PreviousButtons, EInputButton.Excavate))
            {
                IsHoldingItem = !IsHoldingItem;
            }
        }

        private void CheckAttack(NetInput input)
        {
            if(!IsHoldingItem || !input.Buttons.WasPressed(PreviousButtons, EInputButton.Attack))
                return;
            AttackSync++;
        }

        private void CheckVictory(NetInput input)
        {
            if (input.Buttons.WasPressed(PreviousButtons, EInputButton.Victory))             
                _animator.SetTrigger("Victory");
        }

        private void CheckDefeat(NetInput input)
        {
            if (input.Buttons.WasPressed(PreviousButtons, EInputButton.Defeat)) 
                _animator.SetTrigger("Defeat");
        }

        private void CheckTorch(NetInput input)
        {
            if (input.Buttons.WasPressed(PreviousButtons, EInputButton.Torch) && !IsHoldingItem)
            {
                StartCoroutine(WaitOn());
            }
            else
            {
                StartCoroutine(WaitOff());
            }
        }
        private IEnumerator WaitOn()
        {
            Debug.Log("횃불 ON");
            _animator.SetBool("pickTorch", true);
            yield return new WaitForSeconds(1f);
            ItemTorch.SetActive(true);
            Torch.TurnOnLight();
            IsHoldingItem = true;
            PickTorch = true;
        }
        private IEnumerator WaitOff()
        {
            Torch.TurnOffLight();
            yield return new WaitForSeconds(1.0f);
            _animator.SetBool("pickTorch", false);
            ItemTorch.SetActive(false);
            IsHoldingItem = false;
            PickTorch = false;
        }
        /*private IEnumerator useBandage()
        {
            ItemTorch.SetActive(true);
            _animator.SetTrigger("useBandage");
            yield return new WaitForSeconds(5.967f);
            ItemTorch.SetActive(false);
        }*/


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
                if (CaveJungleBGM.cavein == 1)
                {
                    AudioManager.instance.PlayBreathSfx(true);
                    AudioManager.instance.PlayHRGFSfx(true);
                    AudioManager.instance.PlayHRDFSfx(false);
                    AudioManager.instance.PlayDFSfx(false);
                    AudioManager.instance.PlayGFSfx(false);
                }
                else
                {
                    AudioManager.instance.PlayBreathSfx(true);
                    AudioManager.instance.PlayHRGFSfx(false);
                    AudioManager.instance.PlayHRDFSfx(true);
                    AudioManager.instance.PlayDFSfx(false);
                    AudioManager.instance.PlayGFSfx(false);
                }
            }
            else
            {
                kcc.RemoveModifier(sprintProcessor);
                AudioManager.instance.PlayBreathSfx(false);
                AudioManager.instance.PlayHRDFSfx(false);
                AudioManager.instance.PlayHRGFSfx(false);
               
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
                IsCrouching = true;
            }
            else
            {
                kcc.SetHeight(1.6f);
                kcc.RemoveModifier(crouchProcessor);
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
                //var tempRelic = RelicManager.instance.GetTempRelicWithIndex(selectedItemIndex);
                _inventoryItems[UIManager.Singleton.currentSlotIndex] = -1;
                UIManager.Singleton.SetSlotImage(false);
                //tempRelic.SpawnRelic(Table.GetRelicPosition(index), Quaternion.Euler(Vector3.zero), Vector3.zero);
                return;
            }
            if (_inventoryItems[UIManager.Singleton.currentSlotIndex] != -1) return;
            if (hitInfo.collider.TryGetComponent(out TempRelic relic))
            {
                //_inventoryItems[UIManager.Singleton.currentSlotIndex] = relic.relicNumber;
                //UIManager.Singleton.SetSlotImage(true, relic.relicSprites[relic.ChosenVisualIndex]);
                //relic.GetRelic(this);
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
            //var tempRelic = RelicManager.instance.GetTempRelicWithIndex(selectedItemIndex);
            _inventoryItems[UIManager.Singleton.currentSlotIndex] = -1;
            UIManager.Singleton.SetSlotImage(false);
            //tempRelic.SpawnRelic(camTarget.position, camTarget.rotation, camTarget.forward);
        }

        private void UpdateCamTarget()
        {
            camTarget.localRotation = Quaternion.Euler(kcc.GetLookRotation().x, 0f, 0f);
        }

        private void Jumped()
        {
            _animator.SetTrigger(Jump);
            //source.Play();
            AudioManager.instance.PlaySfx(AudioManager.Sfx.JUMP_01, transform.position, transform.position);
        }

        private void Attacked()
        {
            _animator.SetTrigger(Attack);
        }

        private void Crouched()
        {
            CrouchSync = IsCrouching ? 1 : 0;
        }
        
        private Vector3 GetAnimationMoveVelocity()
        {
            if (kcc.Data.RealSpeed < 0.01f)
                return default;

            var velocity = kcc.Data.RealVelocity;

            velocity.y = 0f;

            if (velocity.sqrMagnitude > 1f)
            {
                velocity.Normalize();
            }

            return transform.InverseTransformVector(velocity);
        }

        public void SetClothMaterial(int index)
        {
            Debug.Log("Function Call"+index);
            foreach (var skinnedMeshRenderer in modelParts)
            {
                Debug.Log("Function Run"+index);
                Material[] materials = skinnedMeshRenderer.materials;
                materials[1] = clothMaterials[index];
                materials[4] = hairMaterials[index];
                skinnedMeshRenderer.materials = materials;
            }
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