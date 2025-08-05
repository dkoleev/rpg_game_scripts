using UnityEngine;

namespace Darkness.Runtime.Gameplay.Npc {
    public class TrainKnight : MonoBehaviour, IHittable {
        private Animator _animator;
        
        private void Awake() {
            _animator = GetComponentInChildren<Animator>();
        }

        public void TakeHit(int damage) {
            _animator.CrossFadeInFixedTime("Hit", 0f);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                Debug.Log("Player hit");
            }
        }
    }
}