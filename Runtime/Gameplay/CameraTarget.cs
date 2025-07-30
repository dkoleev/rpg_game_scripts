using UnityEngine;

namespace Darkness.Runtime.Gameplay {
    public class CameraTarget : MonoBehaviour {
        private Transform _target;
        
        public void SetTarget(Transform target) {
            _target = target;
        }

        private void Update() {
            if (_target is null) {
                return;
            }
            
            transform.position = _target.position;
        }
    }
}