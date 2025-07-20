using Unity.Collections;
using Unity.Entities;

namespace Darkness.Runtime.ECS.Components {
    public struct EntityViewData : IComponentData {
        public Entity Entity;
        public FixedString64Bytes PrefabPath;
    }
}