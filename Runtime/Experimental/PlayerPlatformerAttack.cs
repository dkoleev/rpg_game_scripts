using System;
using Darkness.Runtime.Messages;
using MessagePipe;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using VContainer.Unity;
using PlayerInput = Darkness.Runtime.Input.PlayerInput;

namespace Darkness.Runtime.Experimental {
    public class PlayerPlatformerAttack : IStartable, IDisposable {
        public event Action OnPerformAttack;
        public event Action OnPerformSlowAttack;
        public event Action<bool> OnBlocking;
        public bool IsBlocking { get; private set; }
        
        private PlayerInput _playerInput;

        public PlayerPlatformerAttack(IPublisher<PlayerAttackMessage> attackPublisher) {
            
        }

        public void Start() {
            _playerInput = new PlayerInput();
            _playerInput.Enable();
            _playerInput.Player.Attack.started += AttackStarted;
            _playerInput.Player.Attack.performed += AttackPerformed;
            _playerInput.Player.Attack.canceled += AttackCancelled;
            
            _playerInput.Player.Block.started += BlockStarted;
            _playerInput.Player.Block.performed += BlockPerformed;
            _playerInput.Player.Block.canceled += BlockCancelled;
        }
        
        public void Dispose() {
            _playerInput.Player.Attack.started -= AttackStarted;
            _playerInput.Player.Attack.performed -= AttackPerformed;
            _playerInput.Player.Attack.canceled -= AttackCancelled;

            _playerInput.Player.Block.started -= BlockStarted;
            _playerInput.Player.Block.performed -= BlockPerformed;
            _playerInput.Player.Block.canceled -= BlockCancelled;
            
            _playerInput.Disable();
            _playerInput.Dispose();
        }

        private void AttackStarted(InputAction.CallbackContext context) {
            
        }

        private void AttackPerformed(InputAction.CallbackContext context) {
            if (context.interaction is SlowTapInteraction) {
                OnPerformSlowAttack?.Invoke();
            }
            else {
                OnPerformAttack?.Invoke();
            }
        }
        
        private void AttackCancelled(InputAction.CallbackContext context) {
            
        }
        
        private void BlockStarted(InputAction.CallbackContext context) {
        }

        private void BlockPerformed(InputAction.CallbackContext context) {
            IsBlocking = true;
            OnBlocking?.Invoke(true);
        }

        private void BlockCancelled(InputAction.CallbackContext context) {
            IsBlocking = false;
            OnBlocking?.Invoke(false);
        }
    }
}