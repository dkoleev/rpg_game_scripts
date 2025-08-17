using Darkness.Runtime.ScriptableObjects;
using UnityEngine;

namespace Darkness.Runtime.Gameplay.Npc.EnemyStates {
    public class EnemyPatrolState : IEnemyState {
        public void Enter(EnemyAI enemyAI, EnemyAISettings enemyAISettings) {
            
        }
        
        public void Update(EnemyAI enemyAI, EnemyAISettings enemyAISettings) {
            enemyAI.Move();
            if (!enemyAI.IsGroundAhead() || enemyAI.IsWallAhead()) {
                enemyAI.ChangeDirection();
            }

            if (Vector2.Distance(enemyAI.Transform.position, enemyAI.PlayerTransform.position) < enemyAISettings.AttackRange) {
                enemyAI.ChangeState(new EnemyAttackState());
                return;
            }

            if (Vector2.Distance(enemyAI.Transform.position, enemyAI.PlayerTransform.position) < enemyAISettings.ChaseRange) {
                enemyAI.ChangeState(new EnemyChaseState());
            }
        }
        
        public void Exit(EnemyAI enemyAI, EnemyAISettings enemyAISettings) {
            
        }
    }
}
