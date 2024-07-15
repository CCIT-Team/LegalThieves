using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.KCC;
using Fusion.Menu;
using Fusion.Sockets;
using MultiClimb.Menu;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace LegalThieves
{
    public enum EInputButton
    {
        Jump,
        Interaction,
        ThrowItem,
        Sprint,
        Crouch,
        Excavate
    }

    public struct NetInput : INetworkInput
    {
        public NetworkButtons   Buttons;
        public Vector2          Direction;
        public Vector2          LookDelta;
    }

    public class InputManager : SimulationBehaviour, IBeforeUpdate, INetworkRunnerCallbacks
    {
        public TempPlayer localTempPlayer;
        public Vector2    AccumulatedMouseDelta => _mouseDeltaAccumulator.AccumulatedValue;
    
        private NetInput _accumulateInput;
        private bool     _resetInput;
        
        private readonly Vector2Accumulator _mouseDeltaAccumulator = new() { SmoothingWindow = 0.025f };

        void IBeforeUpdate.BeforeUpdate()
        {
            if (_resetInput)
            {
                _resetInput      = false;
                _accumulateInput = default;
            }
        
            var keyboard = Keyboard.current;
        
            if (keyboard != null && (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame || keyboard.escapeKey.wasPressedThisFrame))
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible   = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible   = false;
                }
            }
        
            if(Cursor.lockState != CursorLockMode.Locked)
                return;
        
            NetworkButtons buttons = default;
        
            var mouse = Mouse.current;
            if (mouse != null)
            {
                var mouseDelta = mouse.delta.ReadValue();
                var lookRotationDelta = new Vector2(-mouseDelta.y, mouseDelta.x);
                
                _mouseDeltaAccumulator.Accumulate(lookRotationDelta);
                //buttons.Set(EInputButton.Interaction, keyboard.eKey.isPressed);
            }
            if (keyboard != null)
            {
                if (keyboard.rKey.wasPressedThisFrame && localTempPlayer != null)
                    localTempPlayer.RPC_SetReady();
            
                var moveDirection = Vector2.zero;

                if (keyboard.wKey.isPressed) moveDirection += Vector2.up;
                if (keyboard.sKey.isPressed) moveDirection += Vector2.down;
                if (keyboard.aKey.isPressed) moveDirection += Vector2.left;
                if (keyboard.dKey.isPressed) moveDirection += Vector2.right;

                _accumulateInput.Direction += moveDirection;
                buttons.Set(EInputButton.Jump, keyboard.spaceKey.isPressed);
                buttons.Set(EInputButton.ThrowItem, keyboard.gKey.isPressed);
                buttons.Set(EInputButton.Sprint, keyboard.leftShiftKey.isPressed);
                buttons.Set(EInputButton.Crouch, keyboard.leftCtrlKey.isPressed);
                buttons.Set(EInputButton.Interaction, keyboard.eKey.isPressed);
                buttons.Set(EInputButton.Excavate, keyboard.fKey.isPressed);
            }

            _accumulateInput.Buttons = new NetworkButtons(_accumulateInput.Buttons.Bits | buttons.Bits);
        }

        void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (player != runner.LocalPlayer) 
                return;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }

        void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (player != runner.LocalPlayer) 
                return;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }

        void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
        {
            _accumulateInput.Direction.Normalize();
            _accumulateInput.LookDelta = _mouseDeltaAccumulator.ConsumeTickAligned(runner);
            input.Set(_accumulateInput);
            _resetInput = true;
        }
    
        async void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;

            if (shutdownReason != ShutdownReason.DisconnectedByPluginLogic) return;
            await FindFirstObjectByType<MenuConnectionBehaviour>(FindObjectsInactive.Include)
                .DisconnectAsync(ConnectFailReason.Disconnect);
            FindFirstObjectByType<FusionMenuUIGameplay>(FindObjectsInactive.Include).Controller.Show<FusionMenuUIMain>();
        }

        void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner){}
    
        void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input){}

        void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}

        void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}

        void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason){}

        void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token){}

        void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason){}

        void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message){}

        void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList){}

        void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data){}

        void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken){}

        void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){}

        void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress){}

        void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner){}

        void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner){}
    }
}