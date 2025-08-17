using Darkness.Runtime.ScriptableObjects;
using UnityEngine;

namespace Darkness.Runtime.Gameplay.Npc.EnemyStates {
    public class EnemyDeadState : IEnemyState {
        public void Enter(EnemyAI enemyAI, EnemyAISettings enemyAISettings) {
            enemyAI.Rigidbody2D.linearVelocity = Vector2.zero;
        }
        
        public void Update(EnemyAI enemyAI, EnemyAISettings enemyAISettings) {
        }
        
        public void Exit(EnemyAI enemyAI, EnemyAISettings enemyAISettings) {
        }
    }
}