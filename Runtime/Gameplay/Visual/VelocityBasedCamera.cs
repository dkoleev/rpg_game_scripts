using System;
using Darkness.Runtime.Gameplay.Player;
using UnityEngine;

namespace Darkness.Runtime.Gameplay.Visual {
    public class VelocityBasedCamera : MonoBehaviour {
        private PlayerPlatformerMovement _playerMovement;

        private void Update() {
            Setup();
            if (_playerMovement is null) {
                return;
            }
            
            Debug.Log(_playerMovement.RB.linearVelocity);
        }

        private void Setup() {
            if (_playerMovement is not null) {
                return;
            }
            
            var player = GameObject.FindWithTag("Player");
            if (player is null) {
                return;
            }
            
            _playerMovement = player.GetComponent<PlayerPlatformerMovement>();
        }
    }
}