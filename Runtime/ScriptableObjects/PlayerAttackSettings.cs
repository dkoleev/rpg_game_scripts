using UnityEngine;

namespace Darkness.Runtime.ScriptableObjects {
    [CreateAssetMenu(fileName = "PlayerAttackSettings", menuName = "Game/Player Attack Settings")]
    public class PlayerAttackSettings : ScriptableObject {
        public enum AttackType {
            Default,
            Slow,
            Sit
        }

        [Header("hitbox")]
        [SerializeField] public Vector2 hitboxSize;
        [SerializeField] public Vector2 hitBoxOffset;
        [SerializeField] public float attackAngle;
        [SerializeField] public LayerMask enemyLayer;
        [Header("Knock Player")]
        [SerializeField] public float playerKnockbackForce;
        [SerializeField] public bool playerBounceOnHit;
        [Header("Knock Enemy")]
        [SerializeField] public float enemyKnockbackForce;
        [SerializeField] public float enemyBounceUpForce;
        [SerializeField] public float enemyStunTime;
    }
}
