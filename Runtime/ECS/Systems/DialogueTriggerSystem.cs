using Darkness.Runtime.ECS.Components;
using Darkness.Runtime.ECS.Components.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Darkness.Runtime.ECS.Systems {
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct DialogueTriggerSystem : ISystem {
        private EntityQuery _dialogueTriggerQuery;
        public void OnCreate(ref SystemState state) {
            _dialogueTriggerQuery = SystemAPI.QueryBuilder().WithAll<DialogueInProgressTag>().Build();
        }

        public void OnUpdate(ref SystemState state) {
            Debug.LogError("DialogueTriggerSystem");
            if (!_dialogueTriggerQuery.IsEmptyIgnoreFilter) {
                return;
            }

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (inputData, closestNPC, playerEntity) in 
                     SystemAPI.Query<RefRO<InputData>, RefRO<ClosestNPC>>().
                         WithAll<PlayerTag>().
                         WithNone<DialogueInProgressTag>().
                         WithEntityAccess()) {
                if (inputData.ValueRO.InteractPressed && closestNPC.ValueRO.NPC != Entity.Null) {
                    ecb.AddComponent<DialogueInProgressTag>(closestNPC.ValueRO.NPC);
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}