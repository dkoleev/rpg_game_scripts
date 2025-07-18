using Darkness.Runtime.ECS.Components;
using Darkness.Runtime.ECS.Components.Tags;
using Unity.Entities;

namespace Darkness.Runtime.ECS.Systems {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class SpawnNPCSystem : SystemBase {
        private EntityArchetype _npcArchetype;
        private EndSimulationEntityCommandBufferSystem _endSimulationEcbSystem;

        protected override void OnCreate() {
            // Cache the EndSimulationEntityCommandBufferSystem for efficient reuse
            _endSimulationEcbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();

            // Create archetype with all required components
            _npcArchetype = EntityManager.CreateArchetype(
                typeof(NPCData),
                typeof(EntityViewData)
                // ViewLinkedTag will be added by EntityViewSyncSystem
            );
        }

        protected override void OnUpdate() {
            // Get the command buffer from the ECB system
            var ecb = _endSimulationEcbSystem.CreateCommandBuffer();
            var npcArchetype = _npcArchetype;

            // Process spawn points that haven't spawned an NPC yet
            Entities
                .WithName("SpawnNPCFromSpawnPoints")
                .WithNone<ProcessedSpawnPointTag>()
                .ForEach((Entity spawnPointEntity, ref SpawnPointData spawnPoint) => {
                    // Create NPC entity
                    var npcEntity = ecb.CreateEntity(npcArchetype);

                    // Set NPC components
                    ecb.SetComponent(npcEntity, new NPCData());

                    // Set view data for synchronization with GameObject world
                    ecb.SetComponent(npcEntity, new EntityViewData {
                        PrefabPath = spawnPoint.PrefabPath,
                        SpawnPosition = spawnPoint.SpawnPosition,
                        Entity = npcEntity // Correctly reference the entity itself
                    });

                    // Update spawn point with reference to created entity
                    spawnPoint.SpawnedEntity = npcEntity;
                    ecb.SetComponent(spawnPointEntity, spawnPoint);

                    // Mark spawn point as processed
                    ecb.AddComponent<ProcessedSpawnPointTag>(spawnPointEntity);
                }).WithoutBurst().Schedule();

            // Ensure the ECB system knows we scheduled work for it
            _endSimulationEcbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
