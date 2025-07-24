using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using PlayerInput = Darkness.Runtime.Input.PlayerInput;

namespace Darkness.Runtime.Experimental {
    public class PlayerPlatformerAttack : MonoBehaviour {
        private PlayerInput _playerInput;
        private PlayerAnimator _playerAnimator;

        private void Awake() {
            _playerInput = new PlayerInput();
            _playerAnimator = GetComponent<PlayerAnimator>();
        }

        private void OnEnable() {
            _playerInput.Enable();
            _playerInput.Player.Attack.started += AttackStarted;
            _playerInput.Player.Attack.performed += AttackPerformed;
            _playerInput.Player.Attack.canceled += AttackCancelled;
        }

        private void OnDisable() {
            _playerInput.Player.Attack.started -= AttackStarted;
            _playerInput.Player.Attack.performed -= AttackPerformed;
            _playerInput.Player.Attack.canceled -= AttackCancelled;
            
            _playerInput.Disable();
        }
        
        private void AttackStarted(InputAction.CallbackContext context) {
            
        }

        private void AttackPerformed(InputAction.CallbackContext context) {
            if (context.interaction is SlowTapInteraction) {
                _playerAnimator.PlaySlowAttack();
            }
            else {
                _playerAnimator.PlayAttack();
            }
        }
        
        private void AttackCancelled(InputAction.CallbackContext context) {
            
        }
    }
}