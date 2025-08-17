using Darkness.Runtime.ScriptableObjects;
using UnityEngine;

namespace Darkness.Runtime.Gameplay.Npc.EnemyStates {
    public class EnemyIdleState : IEnemyState {
        private float _idleTime;
        private float _timer;


        public void Enter(EnemyAI enemyAI, EnemyAISettings enemyAISettings) {
            _idleTime = Random.Range(2f, 4f);
            _timer = 0f;
        }

        public void Update(EnemyAI enemyAI, EnemyAISettings enemyAISettings) {
            if (enemyAI.PlayerTransform == null) {
                return;
            }

            _timer += Time.deltaTime;

            // Transition to Attack if player in attack range
            if (Vector2.Distance(enemyAI.Transform.position, enemyAI.PlayerTransform.position) < enemyAISettings.AttackRange) {
                enemyAI.ChangeState(new EnemyAttackState());
                return;
            }

            // Transition to Chase if player is close
            if (Vector3.Distance(enemyAI.Transform.position, enemyAI.PlayerTransform.position) < enemyAISettings.ChaseRange) {
                enemyAI.ChangeState(new EnemyChaseState());
                return;
            }

            // Transition to Patrol after idle
            if (_timer >= _idleTime) {
                enemyAI.ChangeState(new EnemyPatrolState());
            }
        }

        public void Exit(EnemyAI enemyAI, EnemyAISettings enemyAISettings) {
        }
    }
}
