using UnityEngine;

namespace Darkness.Runtime.Gameplay.Player {
    public class PlayerAttack : MonoBehaviour {
        [SerializeField] private PlayerPlatformerAttack.AttackType attackType;
        private Animator _playerAnimator;

        private void Awake() {
            _playerAnimator = transform.root.GetComponentInChildren<Animator>();
            gameObject.SetActive(false);
        }

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
                    
                    var currentTime = _playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                    switch (attackType) {
                        case PlayerPlatformerAttack.AttackType.Default:
                            _playerAnimator.Play("AttackWithEffect", 0, currentTime % 1f);
                            break;
                        case PlayerPlatformerAttack.AttackType.Slow:
                            _playerAnimator.Play("SlowAttackWithEffect", 0, currentTime % 1f);
                            break;
                    }
                }
            }
        }
    }
}
