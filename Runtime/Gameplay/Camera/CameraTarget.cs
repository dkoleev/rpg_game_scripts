using UnityEngine;

namespace Darkness.Runtime.Gameplay.Camera {
    public class CameraTarget : MonoBehaviour {
        public Transform Target => _target;
        private Transform _target;
        
        public void SetTarget(Transform target) {
            _target = target;
        }

        private void Update() {
            if (_target is null) {
                return;
            }
            
            transform.position = _target.position;
            transform.rotation = _target.rotation;
        }
    }
}