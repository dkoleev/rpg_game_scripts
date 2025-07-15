using Unity.Entities;
using Unity.Mathematics;

namespace Darkness.Runtime.ECS.Components {
    public struct PlayerData : IComponentData {
        public float3 Velocity;
        public float Speed;
    }
}