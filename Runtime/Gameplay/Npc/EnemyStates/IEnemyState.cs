using Darkness.Runtime.ScriptableObjects;

namespace Darkness.Runtime.Gameplay.Npc.EnemyStates {
    public interface IEnemyState {
        void Enter(EnemyAI enemyAI, EnemyAISettings enemyAISettings);
        void Update(EnemyAI enemyAI, EnemyAISettings enemyAISettings);
        void Exit(EnemyAI enemyAI, EnemyAISettings enemyAISettings);
    }
}