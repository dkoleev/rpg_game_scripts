using UnityEngine;

namespace Darkness.Runtime.Gameplay.Visual {
    public class ParallaxBackground : MonoBehaviour {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float parallaxFactor = 0.5f; // Adjust for each layer (closer to 0 = less movement)

        private Vector3 _startPos;

        private void Start() {
            _startPos = transform.position;
        }

        private void LateUpdate() {
            var newPos = _startPos + (cameraTransform.position - _startPos) * parallaxFactor;
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        }
    }
}
