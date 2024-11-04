using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.KCC;
using Fusion.Menu;
using Fusion.Sockets;
using LegalThieves.Menu;
using UnityEngine;
using UnityEngine.InputSystem;

namespace New_Neo_LT.Scripts.Player_Input
{
    public enum EInputButton
    {
        Attack1,         // 마우스 왼쪽 클릭
        Attack2,         // 마우스 왼쪽 클릭
        Jump,            // 스페이스바
        Interaction1,    // F 
        Interaction2,    // E skill1?
        Interaction3,    // Q skill2?
        Interaction4,    // R skill3?
        Interaction5,    // G drop item
        Sprint,          // Shift
        Crouch,          // Ctrl
        Inventory,       // Tab
        InputButtonCount // 버튼 개수
    }

    public struct NetInput : INetworkInput
    {
        public NetworkButtons Buttons;
        public Vector2 Direction;
        public Vector2 LookDelta;
    }

    public class InputController : SimulationBehaviour, INetworkRunnerCallbacks
    {
        //public PlayerCharacter playerCharacter;
        public Vector2 AccumulatedMouseDelta => _mouseDeltaAccumulator.AccumulatedValue;

        private NetInput _accumulateInput;
        private bool _resetInput;

        private readonly Vector2Accumulator _mouseDeltaAccumulator = new() { SmoothingWindow = 0.025f };
        
        private PlayerInputActions _inputActions;
        
        /*------------------------------------------------------------------------------------------------------------*/

        #region Monobehaviour Events...

        private void Start()
        {
            // Initialize input actions
            _inputActions = new PlayerInputActions();
            _inputActions.Enable();
            SetPlayerInputActions();
        }
        
        private void OnDisable()
        {
            _inputActions.Disable();
        }

        #endregion
        
        #region Input Actions...

        private void SetPlayerInputActions()
        {
            _inputActions.Mouse.MouseDelta.performed += MouseDelta;
            _inputActions.Mouse.LeftClick.performed  += MouseLeftClick;
            _inputActions.Mouse.RightClick.performed += MouseRightClick;
            _inputActions.Movement.Move.performed    += MovementDirection;
            _inputActions.Movement.Jump.performed    += Jump;
            _inputActions.Movement.Sprint.performed  += Sprint;
            _inputActions.Movement.Crouch.performed  += Crouch;
            _inputActions.Skills.Q.performed         += InteractionQ;
            _inputActions.Skills.E.performed         += InteractionE;
            _inputActions.Skills.R.performed         += InteractionR;
            _inputActions.Skills.F.performed         += InteractionF;
            _inputActions.Skills.G.performed         += InteractionG;
            _inputActions.UI.Tab.performed           += Inventory;
        }

        private Vector2 _mouseDeltaVector;
        private void MouseDelta(InputAction.CallbackContext ctx)
        {
            _mouseDeltaVector.x = -ctx.ReadValue<Vector2>().y;
            _mouseDeltaVector.y = ctx.ReadValue<Vector2>().x;
            _mouseDeltaAccumulator.Accumulate(_mouseDeltaVector);
        }
        
        private void MouseLeftClick(InputAction.CallbackContext ctx)
        {
            _accumulateInput.Buttons.Set(EInputButton.Attack1, ctx.ReadValueAsButton());
        }
        
        private void MouseRightClick(InputAction.CallbackContext ctx)
        {
            _accumulateInput.Buttons.Set(EInputButton.Attack2, ctx.ReadValueAsButton());
        }
        
        private void MovementDirection(InputAction.CallbackContext ctx)
        {
            _accumulateInput.Direction = ctx.ReadValue<Vector2>();
        }
        
        private void Jump(InputAction.CallbackContext ctx)
        {
            _accumulateInput.Buttons.Set(EInputButton.Jump, ctx.ReadValueAsButton());
        }
        
        private void Sprint(InputAction.CallbackContext ctx)
        {
            _accumulateInput.Buttons.Set(EInputButton.Sprint, ctx.ReadValueAsButton());
        }
        
        private void Crouch(InputAction.CallbackContext ctx)
        {
            _accumulateInput.Buttons.Set(EInputButton.Crouch, ctx.ReadValueAsButton());
        }
        
        private void InteractionQ(InputAction.CallbackContext ctx)
        {
            _accumulateInput.Buttons.Set(EInputButton.Interaction3, ctx.ReadValueAsButton());
        }
        
        private void InteractionE(InputAction.CallbackContext ctx)
        {
            _accumulateInput.Buttons.Set(EInputButton.Interaction2, ctx.ReadValueAsButton());
        }
        
        private void InteractionR(InputAction.CallbackContext ctx)
        {
            _accumulateInput.Buttons.Set(EInputButton.Interaction4, ctx.ReadValueAsButton());
        }
        
        private void InteractionF(InputAction.CallbackContext ctx)
        {
            _accumulateInput.Buttons.Set(EInputButton.Interaction1, ctx.ReadValueAsButton());
        }
        
        private void InteractionG(InputAction.CallbackContext ctx)
        {
            _accumulateInput.Buttons.Set(EInputButton.Interaction5, ctx.ReadValueAsButton());
        }
        
        private void Inventory(InputAction.CallbackContext ctx)
        {
            _accumulateInput.Buttons.Set(EInputButton.Inventory, ctx.ReadValueAsButton());
        }
        
        #endregion

        #region INetworkRunnerCallbacks...
        
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                
            }
            
            
            if (player != runner.LocalPlayer)
                return;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        { 
            if (player != runner.LocalPlayer) 
                return;
            
            Cursor.lockState = CursorLockMode.None; 
            Cursor.visible = true;
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            _accumulateInput.LookDelta = _mouseDeltaAccumulator.ConsumeTickAligned(runner);
            input.Set(_accumulateInput);
            _accumulateInput.Buttons = default;
        }

        public async void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (shutdownReason != ShutdownReason.DisconnectedByPluginLogic) 
                return;
            
            await FindFirstObjectByType<MenuConnectionBehaviour>(FindObjectsInactive.Include).DisconnectAsync(ConnectFailReason.Disconnect);
            FindFirstObjectByType<FusionMenuUIGameplay>(FindObjectsInactive.Include).Controller.Show<FusionMenuUIMain>();
        }
        
        #endregion

        #region Unused INetworkRunnerCallbacks...

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

        public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
        
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }

        #endregion
    }
}
