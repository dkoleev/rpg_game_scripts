using Unity.Entities;
using Unity.Mathematics;

namespace Darkness.Runtime.ECS.Components {
    public struct InputData : IComponentData {
        public float3 MoveDirection;
    }
}