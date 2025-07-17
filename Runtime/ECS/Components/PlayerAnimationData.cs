using Unity.Entities;

namespace Darkness.Runtime.ECS.Components {
    public struct PlayerAnimationData : IComponentData {
        public bool Moving;
        public bool Attacking;
    }
}