using UnityEngine;

namespace Darkness.Runtime.Gameplay.Player {
    public class PlayerAttack : MonoBehaviour {
        public void SetActive(bool active) {
            gameObject.SetActive(active);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.TryGetComponent<IHittable>(out var hittable)) {
                hittable.TakeHit(0); //TODO: get damage from config
            }
            else {
                var parentComponent = other.GetComponentInParent<IHittable>();
                if (parentComponent is not null) {
                    parentComponent.TakeHit(0);
                }
            }
        }
    }
}
