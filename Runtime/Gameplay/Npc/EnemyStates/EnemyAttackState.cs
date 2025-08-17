using Darkness.Runtime.ScriptableObjects;
using UnityEngine;

namespace Darkness.Runtime.Gameplay.Npc.EnemyStates {
    public class EnemyAttackState : IEnemyState {
        private float _timer;
        
        public void Enter(EnemyAI enemyAI, EnemyAISettings enemyAISettings) {
            _timer = 0;
        }
        
        public void Update(EnemyAI enemyAI, EnemyAISettings enemyAISettings) {
            _timer += Time.deltaTime;
            if (_timer < enemyAISettings.AttackStartPhaseTime) {
                return;
            }
            
            //TODO perform attack

            if (_timer + enemyAISettings.AttackStartPhaseTime < enemyAISettings.AttackEndPhaseTime) {
                return;
            }
            
            enemyAI.ChangeState(new EnemyIdleState());
        }

        public void Exit(EnemyAI enemyAI, EnemyAISettings enemyAISettings) {
        }
    }
}