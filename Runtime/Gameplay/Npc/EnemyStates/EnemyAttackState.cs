using Darkness.Runtime.ScriptableObjects;
using UnityEngine;

namespace Darkness.Runtime.Gameplay.Npc.EnemyStates {
    public class EnemyAttackState : IEnemyState {
        private float _timer;
        private bool _attackPerformed;

        public void Enter(EnemyAI enemyAI, EnemyAISettings enemyAISettings) {
            _timer = 0;
            _attackPerformed = false;
        }

        public void Update(EnemyAI enemyAI, EnemyAISettings enemyAISettings) {
            enemyAI.SetDirection(enemyAI.PlayerTransform.position.x > enemyAI.Transform.position.x ? 1 : -1);
            _timer += Time.deltaTime;
            if (_timer < enemyAISettings.AttackStartPhaseTime) {
                return;
            }

            if (_attackPerformed) {
                if (_timer + enemyAISettings.AttackStartPhaseTime < enemyAISettings.AttackEndPhaseTime) {
                    return;
                }

                enemyAI.ChangeState(new EnemyIdleState());
                return;
            }
            
            enemyAI.AttackCollider.enabled = true;
            var filter = new ContactFilter2D();
            filter.SetLayerMask(enemyAISettings.AttackLayer);
            var hits = new Collider2D[10];
            var hitsOverlap = enemyAI.AttackCollider.Overlap(filter, hits);
            enemyAI.AttackCollider.enabled = false;
            for (var i = 0; i < hitsOverlap; i++) {
                var hit = hits[i];
                if (hit is null) {
                    continue;
                }

                var hitComponent = hit.GetComponent<IHittable>();
                if (hitComponent is not null) {
                    hitComponent.TakeHit(enemyAISettings.Damage);
                    // var physicsComponent = hit.GetComponent<IPhysicsObject>();
                    // if (physicsComponent is not null) {
                    //     var attackDir = (physicsComponent.Transform.position - transform.position).normalized;
                    //     var knockDir = new Vector2(attackDir.x * settings.enemyKnockbackForce,
                    //         settings.enemyBounceUpForce);
                    //     physicsComponent.Rigidbody2D.AddForce(knockDir, ForceMode2D.Impulse);
                    // }
                }
            }

            _attackPerformed = true;
        }

        public void Exit(EnemyAI enemyAI, EnemyAISettings enemyAISettings) { }
    }
}
