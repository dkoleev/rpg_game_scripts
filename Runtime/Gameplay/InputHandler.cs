using System;
using Darkness.Runtime.Messages;
using JetBrains.Annotations;
using MessagePipe;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using VContainer.Unity;
using PlayerInput = Darkness.Runtime.Input.PlayerInput;

namespace Darkness.Runtime.Gameplay {
    [UsedImplicitly]
    public class InputHandler : IStartable, IDisposable {
        private IPublisher<InputMessage> _inputPublisher;
        private PlayerInput _playerInput;

        void IStartable.Start() {
            _inputPublisher = GlobalMessagePipe.GetPublisher<InputMessage>();
            
            _playerInput = new PlayerInput();
            _playerInput.Enable();
            _playerInput.Player.Move.performed += MovePreformed;
            _playerInput.Player.Move.canceled += MoveCanceled;
            
            _playerInput.Player.Attack.started += AttackStarted;
            _playerInput.Player.Attack.performed += AttackPerformed;
            _playerInput.Player.Attack.canceled += AttackCancelled;
            
            _playerInput.Player.Block.started += BlockStarted;
            _playerInput.Player.Block.performed += BlockPerformed;
            _playerInput.Player.Block.canceled += BlockCancelled;

            _playerInput.Player.Roll.performed += RollPerformed;
        }

        public void Dispose() {
            _playerInput.Player.Move.performed -= MovePreformed;
            _playerInput.Player.Move.canceled -= MoveCanceled;
            _playerInput.Player.Attack.started -= AttackStarted;
            _playerInput.Player.Attack.performed -= AttackPerformed;
            _playerInput.Player.Attack.canceled -= AttackCancelled;
            
            _playerInput.Player.Roll.performed -= RollPerformed;
            
            _playerInput.Disable();
        }
        
        private void RollPerformed(InputAction.CallbackContext context) {
                        
        }
        
        private void AttackStarted(InputAction.CallbackContext context) {
            
        }

        private void AttackPerformed(InputAction.CallbackContext context) {
            _inputPublisher.Publish(context.interaction is SlowTapInteraction
                ? new InputMessage {
                    Type = InputMessage.InputType.Attack, 
                    Phase  = InputMessage.InputPhase.Performed,
                    IsSlowAttack = true
                }
                : new InputMessage {
                    Type = InputMessage.InputType.Attack,
                    Phase = InputMessage.InputPhase.Performed,
                    IsSlowAttack = false
                });
        }
        
        private void AttackCancelled(InputAction.CallbackContext context) {
        }

        private void MoveCanceled(InputAction.CallbackContext obj) {
        }

        private void MovePreformed(InputAction.CallbackContext context) {
        }
        
        private void BlockStarted(InputAction.CallbackContext context) {
            
        }

        private void BlockPerformed(InputAction.CallbackContext context) {
            _inputPublisher.Publish(new InputMessage {
                Type = InputMessage.InputType.Block,
                Phase = InputMessage.InputPhase.Performed
            });
        }

        private void BlockCancelled(InputAction.CallbackContext context) {
            _inputPublisher.Publish(new InputMessage {
                Type = InputMessage.InputType.Block,
                Phase = InputMessage.InputPhase.Cancelled
            });
        }
    }
}