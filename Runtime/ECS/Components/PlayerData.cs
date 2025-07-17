using Unity.Entities;
using Unity.Mathematics;

namespace Darkness.Runtime.ECS.Components {
    public struct PlayerData : IComponentData {
        public float2 Velocity;
        public float Speed;
    }
}