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
    public class InputHandler : IStartable, IDisposable, ITickable {
        private EntityManager _entityManager;
        private PlayerInput _playerInput;
        private Entity _playerEntity;
        private EntityQuery _playerInputQuery;

        void IStartable.Start() {
            _playerInput = new PlayerInput();
            _playerInput.Enable();
            _playerInput.Player.Move.performed += MovePreformed;
            _playerInput.Player.Move.canceled += MoveCanceled;
            _playerInput.Player.Attack.started += AttackPerformed;
            _playerInput.Player.Attack.canceled += AttackCancelled;
            
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _playerInputQuery = _entityManager.CreateEntityQuery(typeof(InputData));
        }
        
        private void AttackPerformed(InputAction.CallbackContext context) {
            if (!_entityManager.Exists(_playerEntity)) {
                return;
            }

            var inputData = _entityManager.GetComponentData<InputData>(_playerEntity);
            inputData.Attack = true;
            _entityManager.SetComponentData(_playerEntity, inputData);
        }
        
        private void AttackCancelled(InputAction.CallbackContext context) {
            if (!_entityManager.Exists(_playerEntity)) {
                return;
            }

            var inputData = _entityManager.GetComponentData<InputData>(_playerEntity);
            inputData.Attack = false;
            _entityManager.SetComponentData(_playerEntity, inputData);
        }


        private void MoveCanceled(InputAction.CallbackContext obj) {
            if (!_entityManager.Exists(_playerEntity)) {
                return;
            }

            var inputData = _entityManager.GetComponentData<InputData>(_playerEntity);
            inputData.MoveValue = float2.zero;
            
            _entityManager.SetComponentData(_playerEntity, inputData);
        }

        private void MovePreformed(InputAction.CallbackContext context) {
            if (!_entityManager.Exists(_playerEntity)) {
                return;
            }

            var inputValue = context.ReadValue<Vector2>();
            var inputData = _entityManager.GetComponentData<InputData>(_playerEntity);
            inputData.MoveValue = new float2(inputValue.x, inputValue.y);
            _entityManager.SetComponentData(_playerEntity, inputData);
        }

        public void Dispose() {
            _playerInput.Player.Move.performed -= MovePreformed;
            _playerInput.Player.Move.canceled -= MoveCanceled;
            _playerInput.Player.Attack.performed -= AttackPerformed;
            _playerInput.Player.Attack.canceled -= AttackCancelled;
            _playerInput.Disable();        
        }

        public void Tick() {
            if (_playerEntity == Entity.Null) {
                if (_playerInputQuery.IsEmpty) {
                    return;
                }
            
                _playerEntity = _playerInputQuery.GetSingletonEntity();
            }
            
            CheckForInteract();
        }
        
        private void CheckForInteract() {
            var playerEntity = _playerInputQuery.GetSingletonEntity();
            var inputData = _entityManager.GetComponentData<InputData>(playerEntity);
            if (_playerInput.Player.Interact.WasPressedThisFrame()) {
                inputData.InteractPressed = true;
                _entityManager.SetComponentData(playerEntity, inputData);
            }
            else if(inputData.InteractPressed) {
                inputData.InteractPressed = false;
                _entityManager.SetComponentData(playerEntity, inputData);
            }
        }
    }
}