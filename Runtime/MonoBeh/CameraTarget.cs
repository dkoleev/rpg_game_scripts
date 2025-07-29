using UnityEngine;

namespace Darkness.Runtime.MonoBeh {
    public class CameraTarget : MonoBehaviour {
        private Transform _target;
        
        private void Awake() {
            _target = GameObject.FindGameObjectWithTag("Player").transform;    
        }

        private void Update() {
            transform.position = _target.position;
        }
    }
}