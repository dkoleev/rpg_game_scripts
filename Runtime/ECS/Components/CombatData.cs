using Unity.Entities;

namespace Darkness.Runtime.ECS.Components {
    public struct CombatData : IComponentData {
        public bool ActionInProgress;
        public float CurrentActionTime;
        public bool IsSlowAttack;
        public bool IsAttack;
        public bool IsRoll;
    }
}