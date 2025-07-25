using UnityEngine;
using VContainer;

namespace Darkness.Runtime.Experimental {
    public class PlayerAnimator : MonoBehaviour {
        private static readonly int VelocityY = Animator.StringToHash("VelocityY");
        private static readonly int VelocityX = Animator.StringToHash("VelocityX");
        private static readonly int Land = Animator.StringToHash("Land");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int SlowAttack = Animator.StringToHash("SlowAttack");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Dash = Animator.StringToHash("Dash");
        private static readonly int Block = Animator.StringToHash("Blocking");

        public bool StartedJumping { private get; set; }
        public bool JustLanded { private get; set; }
        public bool StartDashing { private get; set; }

        private PlayerPlatformerMovement _mov;
        private Animator _anim;
        private SpriteRenderer _spriteRend;
        private PlayerPlatformerAttack _attack;

        [Inject]
        public void Construct(PlayerPlatformerAttack attack) {
            _attack = attack;
            _attack.OnPerformAttack += PlayAttack;
            _attack.OnPerformSlowAttack += PlaySlowAttack;
            _attack.OnBlocking += SetBlock;
        }

        private void Start() {
            _mov = GetComponent<PlayerPlatformerMovement>();
            _spriteRend = GetComponentInChildren<SpriteRenderer>();
            _anim = _spriteRend.GetComponent<Animator>();
        }

        private void LateUpdate() {
            CheckAnimationState();
        }

        private void CheckAnimationState() {
            if (StartedJumping) {
                _anim.SetTrigger(Jump);
                StartedJumping = false;
                return;
            }
            
            if (StartDashing) {
                _anim.SetTrigger(Dash);
                StartDashing = false;
                return;
            }

            if (JustLanded) {
                _anim.SetTrigger(Land);
                JustLanded = false;
                return;
            }

            _anim.SetFloat(VelocityY, _mov.RB.linearVelocity.y);
            _anim.SetFloat(VelocityX, Mathf.Abs(_mov.RB.linearVelocity.x));
        }

        private void PlayAttack() {
            _anim.SetTrigger(Attack);
        }

        private void PlaySlowAttack() {
            _anim.SetTrigger(SlowAttack);
        }

        private void SetBlock(bool isBlocking) {
            _anim.CrossFadeInFixedTime(isBlocking ? "ToBlock" : "Idle", 0f);
        }
    }
}