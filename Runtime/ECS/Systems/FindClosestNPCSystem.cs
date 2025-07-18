using Darkness.Runtime.ECS.Components;
using Darkness.Runtime.ECS.Components.Tags;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Darkness.Runtime.ECS.Systems {
    [BurstCompile]
    public partial struct FindClosestNPCSystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            var entityManager = state.EntityManager;
            var playerEntity = Entity.Null;
            var playerPosition = float2.zero;

            foreach (var (playerPositionData, entity) in SystemAPI.Query<RefRO<PositionData>>().WithAll<PlayerTag>().WithEntityAccess()) {
                playerEntity = entity;
                playerPosition = playerPositionData.ValueRO.Position;
                break;
            }

            if (playerEntity == Entity.Null) {
                return;
            }
            
            var closestNPC = Entity.Null;
            var closestNPCDistance = float.MaxValue;
            foreach (var (npcPositionData, entity) in SystemAPI.Query<RefRO<PositionData>>().WithAll<NPCTag>().WithEntityAccess()) {
                var distance = math.distancesq(playerPosition, npcPositionData.ValueRO.Position);
                if (distance < 4 && distance < closestNPCDistance) {
                    closestNPC = entity;
                    closestNPCDistance = distance;
                }
            }

            if (closestNPC == Entity.Null) {
                entityManager.RemoveComponent<ClosestNPC>(playerEntity);
            }
            else {
                if (entityManager.HasComponent<ClosestNPC>(playerEntity)) {
                    entityManager.SetComponentData(playerEntity, new ClosestNPC {
                        NPC = closestNPC
                    });
                }
                else {
                    entityManager.AddComponentData(playerEntity, new ClosestNPC {
                        NPC = closestNPC
                    });     
                }
            }
        }
    }
}