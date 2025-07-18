using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Darkness.Runtime.ECS.Components {
    public struct SpawnPointData : IComponentData {
        public float2 SpawnPosition;
        public Entity SpawnedEntity;
        public FixedString64Bytes PrefabPath;
    }
}