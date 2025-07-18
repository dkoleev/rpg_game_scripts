using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Darkness.Runtime.ECS.Components {
    public class EntityViewData : IComponentData {
        public Entity Entity;
        public float2 SpawnPosition;
        public FixedString64Bytes PrefabPath;
    }
}