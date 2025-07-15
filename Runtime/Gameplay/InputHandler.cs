using System;
using Darkness.Runtime.ECS.Components;
using JetBrains.Annotations;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;
using PlayerInput = Darkness.Runtime.Input.PlayerInput;

namespace Darkness.Runtime.Gameplay {
    [UsedImplicitly]
    public class InputHandler : IStartable, IDisposable {
        private EntityManager _entityManager;
        private Entity _playerEntity;
        private PlayerInput _playerInput;

        void IStartable.Start() {
            _playerInput = new PlayerInput();
            _playerInput.Enable();
            _playerInput.Player.Move.performed += MovePreformed;
            _playerInput.Player.Move.canceled += MoveCanceled;
            
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _playerEntity = _entityManager.CreateEntityQuery(typeof(InputData)).GetSingletonEntity();
        }

        private void MoveCanceled(InputAction.CallbackContext obj) {
            if (!_entityManager.Exists(_playerEntity)) {
                return;
            }

            _entityManager.SetComponentData(_playerEntity,
                new InputData { MoveDirection = float3.zero });
            
        }

        private void MovePreformed(InputAction.CallbackContext context) {
            if (!_entityManager.Exists(_playerEntity)) {
                return;
            }

            var inputValue = context.ReadValue<Vector2>();
            _entityManager.SetComponentData(_playerEntity,
                new InputData { MoveDirection = new float3(inputValue.x, inputValue.y, 0) });
        }

        public void Dispose() {
            _playerInput.Player.Move.performed -= MovePreformed;
            _playerInput.Disable();        
        }
    }
}