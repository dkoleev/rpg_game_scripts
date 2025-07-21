using Unity.Entities;

namespace Darkness.Runtime.ECS.Components {
    public struct CombatData : IComponentData {
        public bool AttackInProgress;
        public float AttackTime;
    }
}