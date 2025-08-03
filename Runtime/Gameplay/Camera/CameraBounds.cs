using UnityEngine;

namespace Darkness.Runtime.Gameplay.Camera {
    public class CameraBounds : MonoBehaviour {
        private void Awake() {
            var cameraManager = FindAnyObjectByType<PlatformerCamera>();
            if (cameraManager is null) {
                Debug.LogWarning("Platformer Camera not found");
                return;
            }
            cameraManager.SetBounds(GetComponent<CompositeCollider2D>());
        }
    }
}