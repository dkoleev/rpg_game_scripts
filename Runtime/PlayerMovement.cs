using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Darkness.Runtime
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;

        private Rigidbody2D _rb;

        private InputAction _moveAction;
        private InputAction _jumpAction;

        private Vector2 _moveInput;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();

            _moveAction = InputSystem.actions.FindAction("Move");
            _jumpAction = InputSystem.actions.FindAction("Jump");
        }

        void Update()
        {
            _moveInput = _moveAction.ReadValue<Vector2>();
            if (_jumpAction.IsPressed())
            {
                // your jump code here
            }
        }

        private void FixedUpdate()
        {
            _rb.linearVelocity = _moveInput * moveSpeed;
        }
    }
}
