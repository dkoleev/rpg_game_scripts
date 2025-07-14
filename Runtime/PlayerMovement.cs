using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Darkness.Runtime {
    public class PlayerMovement : MonoBehaviour {
        private static readonly int IsMovingAnimProperty = Animator.StringToHash("IsMoving");
        private static readonly int AttackAnimProperty = Animator.StringToHash("Attack");
        [SerializeField] private float moveSpeed = 5f;

        private Rigidbody2D _rb;
        private SpriteRenderer _characterSprite;
        private Animator _characterAnimator;

        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _attackAction;

        private Vector2 _moveInput;

        private void Start() {
            _rb = GetComponent<Rigidbody2D>();
            _characterSprite = GetComponentInChildren<SpriteRenderer>();
            _characterAnimator = GetComponentInChildren<Animator>();

            _moveAction = InputSystem.actions.FindAction("Move");
            _jumpAction = InputSystem.actions.FindAction("Jump");
            _attackAction = InputSystem.actions.FindAction("Attack");
        }

        private void Update() {
            _moveInput = _moveAction.ReadValue<Vector2>();
            if (_jumpAction.IsPressed()) {
                //TODO: jump logic if needed of course
            }

            if (_attackAction.WasPerformedThisFrame()) Attack();

            //TODO: swim logic

            //TODO: fly logic

            //just for test
        }

        private void FixedUpdate() {
            _rb.linearVelocity = _moveInput * moveSpeed;
            if (_moveInput != Vector2.zero) {
                _characterAnimator.SetBool(IsMovingAnimProperty, true);
                if (_moveInput.x != 0) _characterSprite.flipX = _moveInput.x < 0;
            }
            else {
                _characterAnimator.SetBool(IsMovingAnimProperty, false);
            }
        }

        private void Attack() {
            _characterAnimator.SetTrigger(AttackAnimProperty);
        }
    }
}