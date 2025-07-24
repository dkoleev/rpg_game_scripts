using Darkness.Runtime.ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Darkness.Runtime.Presentation {
    public class PlayerView : MonoBehaviour {
        private static readonly int WalkingAnimProperty = Animator.StringToHash("Walking");
        private static readonly int RunningAnimProperty = Animator.StringToHash("Running");
        private static readonly int AttackAnimProperty = Animator.StringToHash("Attack");
        private static readonly int SlowAttackAnimProperty = Animator.StringToHash("SlowAttack");
        private static readonly int RollAnimProperty = Animator.StringToHash("Roll");
        private static readonly int MoveSpeedAnimProperty = Animator.StringToHash("MoveSpeed");

        public Entity Entity;
        
        private Rigidbody2D _rb;
        private SpriteRenderer _characterSprite;
        private Animator _characterAnimator;

        private EntityManager _entityManager;
        private bool _attackAnimationTriggered;

        private void Start() {
            _rb = GetComponent<Rigidbody2D>();
            _characterSprite = GetComponentInChildren<SpriteRenderer>();
            _characterAnimator = GetComponentInChildren<Animator>();
            
            var world = World.DefaultGameObjectInjectionWorld;
            _entityManager = world.EntityManager;
        }

        private void Update() {
            if (!_entityManager.Exists(Entity)) {
                return;
            }

            var animationData = _entityManager.GetComponentData<AnimationData>(Entity);
            var combatData = _entityManager.GetComponentData<CombatData>(Entity);

            if (combatData.ActionInProgress) {
                if (!_attackAnimationTriggered) {
                    if (combatData.IsSlowAttack) {
                        PlaySlowAttackAnimation();
                    }
                    else if(combatData.IsAttack) {
                        PlayAttackAnimation();
                    }else if (combatData.IsRoll) {
                        PlayRollAnimation();
                    }
                    _attackAnimationTriggered = true;
                }
            }
            else {
                _attackAnimationTriggered = false;
                PlayMoveAnimation(animationData);
            }
        }

        private void PlayMoveAnimation(AnimationData animationData) {
            _characterAnimator.SetBool(RunningAnimProperty, animationData.Moving);
        }

        private void FixedUpdate() {
            if (!_entityManager.Exists(Entity)) {
                return;
            }

            var playerData = _entityManager.GetComponentData<MovementData>(Entity);
            // _rb.MovePosition(new Vector2(playerData.Position.x, playerData.Position.y)); // Physics-based position update
            var velocity = new Vector2(playerData.Velocity.x, playerData.Velocity.y);
            _rb.linearVelocity = velocity; // Physics-based position update
            
            _characterAnimator.SetFloat(MoveSpeedAnimProperty, velocity.magnitude);
            
            if (playerData.Velocity.x != 0) _characterSprite.flipX = playerData.Velocity.x < 0;
        }
        
        private void PlayAttackAnimation() {
            _characterAnimator.SetTrigger(AttackAnimProperty);
        }
        
        private void PlaySlowAttackAnimation() {
            _characterAnimator.SetTrigger(SlowAttackAnimProperty);
        }
        
        private void PlayRollAnimation() {
            _characterAnimator.SetTrigger(RollAnimProperty);
        }
    }
}
