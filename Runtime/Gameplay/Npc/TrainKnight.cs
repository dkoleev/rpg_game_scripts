using UnityEngine;

namespace Darkness.Runtime.Gameplay.Npc {
    public class TrainKnight : MonoBehaviour, IHittable, IPhysicsObject {
        public Rigidbody2D Rigidbody2D => _rb;
        public Transform Transform => transform;
        
        private Animator _animator;
        private Rigidbody2D _rb;
        
        private void Awake() {
            _animator = GetComponentInChildren<Animator>();
            _rb = GetComponent<Rigidbody2D>();
        }
        
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                Debug.Log("Player hit");
            }
        }
        
        public void TakeHit(int damage) {
            _animator.Play("Hit");
        }

        public void Stun(float duration) {
            
        }
    }
}