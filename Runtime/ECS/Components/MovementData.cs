using Unity.Entities;
using Unity.Mathematics;

namespace Darkness.Runtime.ECS.Components {
    public struct MovementData : IComponentData {
        public float2 Velocity;
        public float Speed;
    }
}