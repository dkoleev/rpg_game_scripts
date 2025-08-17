using UnityEngine;

namespace Darkness.Runtime.ScriptableObjects {
    [CreateAssetMenu(fileName = "EnemyAI_Settings", menuName = "Game/Enemy AI Settings")]
    public class EnemyAISettings : ScriptableObject {
        [field: SerializeField] public float MoveSpeed { get; private set; } = 1f;
        [field: SerializeField] public float ChaseSpeed { get; private set; } = 2f;
        [field: SerializeField] public float ChaseRange { get; private set; } = 5f;
        [field: SerializeField] public float AttackRange { get; private set; } = 1f;
        [field: SerializeField] public float AttackCooldown { get; private set; } = 1f;
        [field: SerializeField] public float AttackStartPhaseTime { get; private set; } = 1f;
        [field: SerializeField] public float AttackEndPhaseTime { get; private set; } = 1f;
        [field: SerializeField] public float WallCheckDistance { get; private set; } = 0.5f;
        [field: SerializeField] public float GroundCheckDownDistance { get; private set; } = 1.0f;
        [field: SerializeField] public float GroundCheckForwardDistance { get; private set; } = 0.5f;
        [field: SerializeField] public bool StartDirectionToRight { get; private set; } = true;
        [field: SerializeField] public LayerMask GroundLayer { get; private set; }
        [field: SerializeField] public LayerMask WallLayer { get; private set; }
    }
}
