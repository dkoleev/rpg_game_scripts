using System;
using Darkness.Runtime.Gameplay.Player;
using Unity.Cinemachine;
using UnityEngine;

namespace Darkness.Runtime.Gameplay.Camera {
    [ExecuteAlways]
    public class PlatformerCamera : CinemachineCameraManagerBase {
        [Space]
        [SerializeField] private float fallSpeedThreshold = -15f;
        [Header("State Cameras")]
        [ChildCameraProperty] public CinemachineVirtualCameraBase defaultCamera;
        [ChildCameraProperty] public CinemachineVirtualCameraBase fallCamera;

        private CinemachineConfiner2D _confiner;
        private PlayerPlatformerMovement _platformerMovement;

        private void Awake() {
            _confiner = GetComponent<CinemachineConfiner2D>();
        }

        public void SetBounds(Collider2D bounds) {
            _confiner.InvalidateBoundingShapeCache();
            _confiner.BoundingShape2D = bounds;
        }

        protected override CinemachineVirtualCameraBase ChooseCurrentCamera(Vector3 worldUp, float deltaTime) {
            SetupPlayer();
            if (_platformerMovement is null) {
                return defaultCamera;
            }
            
            return  _platformerMovement.RB.linearVelocityY < fallSpeedThreshold ? fallCamera : defaultCamera;
        }

        private void SetupPlayer() {
            if (_platformerMovement is not null) {
                return;
            }
            
            var player = GameObject.FindWithTag("Player");
            if (player is null) {
                return;
            }
            
            _platformerMovement = player.GetComponent<PlayerPlatformerMovement>();
        }
    }
}