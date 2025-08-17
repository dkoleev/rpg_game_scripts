using Darkness.Runtime.ScriptableObjects;
using UnityEngine;

namespace Darkness.Runtime.Gameplay.Npc.EnemyStates {
    public class EnemyChaseState : IEnemyState {
        public void Enter(EnemyAI enemyAI, EnemyAISettings enemyAISettings) {
        }

        public void Update(EnemyAI enemyAI, EnemyAISettings enemyAISettings) {
            enemyAI.SetDirection(enemyAI.PlayerTransform.position.x > enemyAI.Transform.position.x ? 1 : -1);
            enemyAI.Move();

            if (enemyAI.PlayerIsDead) {
                enemyAI.ChangeState(new EnemyIdleState());
                return;
            }

            var distance = Vector2.Distance(enemyAI.Transform.position, enemyAI.PlayerTransform.position);
            if (distance > enemyAISettings.ChaseRange + 1.0f) {
                enemyAI.ChangeState(new EnemyPatrolState());
            }
            else if (distance <= enemyAISettings.AttackRange) {
                enemyAI.ChangeState(new EnemyAttackState());
            }
        }

        public void Exit(EnemyAI enemyAI, EnemyAISettings enemyAISettings) {
        }
    }
}