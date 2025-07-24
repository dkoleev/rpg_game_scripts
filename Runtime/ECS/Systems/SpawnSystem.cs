using Darkness.Runtime.ECS.Components;
using Darkness.Runtime.ECS.Components.Tags;
using Unity.Entities;
using Unity.Mathematics;

namespace Darkness.Runtime.ECS.Systems {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class SpawnSystem : SystemBase {
        private EntityArchetype _npcArchetype;
        private EntityArchetype _playerArchetype;
        private EndSimulationEntityCommandBufferSystem _endSimulationEcbSystem;

        protected override void OnCreate() {
            // Cache the EndSimulationEntityCommandBufferSystem for efficient reuse
            _endSimulationEcbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            
            _playerArchetype = EntityManager.CreateArchetype(
                typeof(PlayerTag),
                typeof(MovementData),
                typeof(InputData),
                typeof(AnimationData),
                typeof(PositionData),
                typeof(EntityViewData),
                typeof(CombatData)
            );

            // Create archetype with all required components
            _npcArchetype = EntityManager.CreateArchetype(
                typeof(NPCTag),
                typeof(PositionData),
                typeof(MovementData),
                typeof(EntityViewData)
                // ViewLinkedTag will be added by EntityViewSyncSystem
            );
        }

        protected override void OnUpdate() {
            // Get the command buffer from the ECB system
            var ecb = _endSimulationEcbSystem.CreateCommandBuffer();
            var npcArchetype = _npcArchetype;
            var playerArchetype = _playerArchetype;

            // Process spawn points that haven't spawned an NPC yet
            Entities
                .WithName("SpawnCharactersFromSpawnPoints")
                .WithNone<ProcessedSpawnPointTag>()
                .ForEach((Entity spawnPointEntity, ref SpawnPointData spawnPoint) => {
                    if (spawnPoint.IsPlayerSpawnPoint) {
                        // Create player entity
                        var playerEntity = ecb.CreateEntity(playerArchetype);
                        ecb.SetComponent(playerEntity, new PlayerTag());
                        ecb.SetComponent(playerEntity, new PositionData {
                            Position = spawnPoint.SpawnPosition
                        });
                        // Initialize player data
                        ecb.SetComponent(playerEntity, new MovementData
                        {
                            Velocity = float2.zero,
                            Speed = 50f
                        });
                        // Initialize input data
                        ecb.SetComponent(playerEntity, new InputData
                        {
                            MoveValue = float2.zero
                        });
                        ecb.SetComponent(playerEntity, new AnimationData
                        {
                            Moving = false
                        });
                        ecb.SetComponent(playerEntity, new CombatData {
                            ActionInProgress = false,
                            CurrentActionTime = 0f
                        });
                        ecb.SetComponent(playerEntity, new EntityViewData {
                            PrefabPath = spawnPoint.PrefabPath,
                            Entity = playerEntity // Correctly reference the entity itself
                        });
                        
                        // Update spawn point with reference to created entity
                        spawnPoint.SpawnedEntity = playerEntity;
                        ecb.SetComponent(spawnPointEntity, spawnPoint);
                    }
                    else {
                        // Create NPC entity
                        var npcEntity = ecb.CreateEntity(npcArchetype);
                        // Set NPC components
                        ecb.SetComponent(npcEntity, new NPCTag());
                        // Set view data for synchronization with GameObject world
                        ecb.SetComponent(npcEntity, new EntityViewData {
                            PrefabPath = spawnPoint.PrefabPath,
                            Entity = npcEntity // Correctly reference the entity itself
                        });
                        ecb.SetComponent(npcEntity, new PositionData {
                            Position = spawnPoint.SpawnPosition
                        });
                    
                        // Update spawn point with reference to created entity
                        spawnPoint.SpawnedEntity = npcEntity;
                        ecb.SetComponent(spawnPointEntity, spawnPoint);
                    }
                    
                    // Mark spawn point as processed
                    ecb.AddComponent<ProcessedSpawnPointTag>(spawnPointEntity);
                }).WithoutBurst().Schedule();

            // Ensure the ECB system knows we scheduled work for it
            _endSimulationEcbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
