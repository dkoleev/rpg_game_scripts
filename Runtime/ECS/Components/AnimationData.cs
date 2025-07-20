using Unity.Entities;

namespace Darkness.Runtime.ECS.Components {
    public struct AnimationData : IComponentData {
        public bool Moving;
        public bool Attacking;
    }
}