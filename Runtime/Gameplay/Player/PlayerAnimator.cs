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
            _attack.OnHit += PlayHitWhileAttack;
            _attack.OnBlocking += SetBlock;
        }

        private void OnDisable() {
            _attack.OnPerformAttack -= PlayAttack;
            _attack.OnHit -= PlayHitWhileAttack;
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
                case PlayerAttackSettings.AttackType.Main:
                case PlayerAttackSettings.AttackType.Sit:
                    _anim.SetTrigger(_mov.IsSitting ? SitAttack : Attack);
                    break;
                case PlayerAttackSettings.AttackType.Slow:
                    _anim.SetTrigger(SlowAttack);
                    break;
                case PlayerAttackSettings.AttackType.MainCombo1:
                    _anim.Play("UpLightAttack");
                    break;
                case PlayerAttackSettings.AttackType.MainCombo2:
                    _anim.Play("MainCombo2");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attackType), attackType, null);
            }
        }

        private void PlayHitWhileAttack(PlayerAttackSettings.AttackType attackType) {
            switch (attackType) {
                case PlayerAttackSettings.AttackType.MainCombo1:
                    var stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
                    var progress = stateInfo.normalizedTime % 1f;
                    _anim.Play("UpLightAttackWithEffect", 0, progress);
                    break;
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

        private void PlayAnimationFromOffset(string clipName, float skipSeconds) {
            // Find the clip in the Animator
            var controller = _anim.runtimeAnimatorController;
            AnimationClip targetClip = null;

            foreach (var clip in controller.animationClips) {
                if (clip.name != clipName) continue;
                targetClip = clip;
                break;
            }

            if (targetClip == null) {
                Debug.LogWarning($"Clip '{clipName}' not found in Animator!");
                return;
            }

            var normalizedTime = skipSeconds / targetClip.length;
            _anim.Play(clipName, 0, normalizedTime);
            
        }
    }
}