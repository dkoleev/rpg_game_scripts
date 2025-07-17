using Unity.Entities;
using UnityEngine;

namespace Darkness.Runtime.ECS.Components {
    public class EntityViewData : IComponentData {
        public Entity Entity;
        public UnityObjectRef<GameObject> EntityView;
    }
}