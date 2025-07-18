using Darkness.Runtime.ECS.Components;
using Darkness.Runtime.ECS.Components.Tags;
using Unity.Burst;
using Unity.Entities;

namespace Darkness.Runtime.ECS.Systems {
    [BurstCompile]
    public partial struct DialogueTriggerSystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            foreach (var (inputData, closestNPC, playerEntity) in 
                     SystemAPI.Query<RefRO<InputData>, RefRO<ClosestNPC>>().
                         WithAll<PlayerTag>().
                         WithNone<DialogueInProgressTag>().
                         WithEntityAccess()) {
                if (inputData.ValueRO.InteractPressed && closestNPC.ValueRO.NPC != Entity.Null) {
                    state.EntityManager.AddComponentData(closestNPC.ValueRO.NPC, new DialogueInProgressTag());
                }
            }
        }
    }
}