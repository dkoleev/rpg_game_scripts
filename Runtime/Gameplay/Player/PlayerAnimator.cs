using System;
using Darkness.Runtime.ScriptableObjects;
using UnityEngine;
using VContainer;

namespace Darkness.Runtime.Gameplay.Player {
    public class PlayerAnimator : MonoBehaviour {
        private static readonly int VelocityY = Animator.StringToHash("VelocityY");
        private static readonly int VelocityX = Animator.StringToHash("VelocityX");
        private static readonly int Land = Animator.StringToHash("Land");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int SlowAttack = Animator.StringToHash("SlowAttack");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int SitAttack = Animator.StringToHash("AttackSit");
        private static readonly int Dash = Animator.StringToHash("Dash");
        private static readonly int Slide = Animator.StringToHash("Slide");

        public bool StartedJumping { private get; set; }
        public bool JustLanded { private get; set; }
        public bool StartDashing { private get; set; }
        public bool StartSliding { private get; set; }

        private PlayerPlatformerMovement _mov;
        private Animator _anim;
        private PlayerPlatformerAttack _attack;
        private bool _isSitting;

        private void Awake() {
            _attack = GetComponent<PlayerPlatformerAttack>();
            _mov = GetComponent<PlayerPlatformerMovement>();
            _anim = transform.GetComponentInChildren<Animator>();
        }

        private void OnEnable() {
            _attack.OnPerformAttack += PlayAttack;
            _attack.OnBlocking += SetBlock;
        }

        private void OnDisable() {
            _attack.OnPerformAttack -= PlayAttack;
            _attack.OnBlocking -= SetBlock;
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
            
            if (StartSliding) {
                _anim.SetTrigger(Slide);
                StartSliding = false;
                return;
            }

            if (JustLanded) {
                _anim.SetTrigger(Land);
                JustLanded = false;
                return;
            }
            
            SetSitting(_mov.IsSitting);

            _anim.SetFloat(VelocityY, _mov.Rigidbody2D.linearVelocity.y);
            _anim.SetFloat(VelocityX, Mathf.Abs(_mov.Rigidbody2D.linearVelocity.x));
        }

        private void PlayAttack(PlayerAttackSettings.AttackType attackType) {
            switch (attackType) {
                case PlayerAttackSettings.AttackType.Default:
                case PlayerAttackSettings.AttackType.Sit:
                    _anim.SetTrigger(_mov.IsSitting ? SitAttack : Attack);
                    break;
                case PlayerAttackSettings.AttackType.Slow:
                    _anim.SetTrigger(SlowAttack);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attackType), attackType, null);
            }
        }

        private void SetBlock(bool isBlocking) {
            _anim.CrossFadeInFixedTime(isBlocking ? "ToBlock" : "Idle", 0f);
        }
        
        private void SetSitting(bool isSitting) {
            if (_isSitting == isSitting) {
                return;
            }
            _isSitting = isSitting;
            _anim.CrossFadeInFixedTime(isSitting ? "ToSit" : "FromSit", 0f); //Play instead CrossFadeInFixedTime?
        }
    }
}