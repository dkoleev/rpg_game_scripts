using Darkness.Runtime.ECS.Components;
using Unity.Entities;

namespace Darkness.Runtime.ECS.Systems {
    public partial class SpawnNPCSystem : SystemBase {
        private EntityArchetype _npcArchetype;

        protected override void OnCreate() {
            _npcArchetype = EntityManager.CreateArchetype(
                typeof(NPCData)
            );
        }

        protected override void OnUpdate() {
            var entityManager = EntityManager;

            // Use Entity Command Buffer for thread-safe entity operations
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            // Query all entities with SpawnPointComponent that haven't spawned anything yet
            Entities.WithoutBurst().ForEach((Entity spawnPointEntity, ref SpawnPointData spawnPoint) => {
                if (spawnPoint.SpawnedEntity != Entity.Null) {
                    return; // Skip already processed spawn points
                }

                var npcEntity = ecb.CreateEntity(_npcArchetype);
                ecb.SetComponent(npcEntity, new NPCData {
                    Position = spawnPoint.SpawnPosition,
                });

                spawnPoint.SpawnedEntity = npcEntity;
                ecb.SetComponent(spawnPointEntity, spawnPoint);

            }).Run();

            // Play back all commands to ensure thread safety
            ecb.Playback(entityManager);
            ecb.Dispose();

        }
    }
}