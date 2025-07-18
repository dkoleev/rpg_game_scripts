using Unity.Entities;

namespace Darkness.Runtime.ECS.Components {
    public struct ClosestNPC : IComponentData {
        public Entity NPC;
    }
}