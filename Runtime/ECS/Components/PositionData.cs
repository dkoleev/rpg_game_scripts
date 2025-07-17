using Unity.Entities;
using Unity.Mathematics;

namespace Darkness.Runtime.ECS.Components {
    public struct PositionData : IComponentData {
        public float2 Position;
    }
}