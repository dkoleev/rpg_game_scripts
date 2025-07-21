using Darkness.Runtime.ECS.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Darkness.Runtime.ECS.Systems {
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct PlayerMovementSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InputData>();
        }

        public void OnUpdate(ref SystemState state) {
            var fixedDeltaTime = SystemAPI.Time.fixedDeltaTime;
            foreach (var (playerData, inputData, combatData) in 
                     SystemAPI.Query<RefRW<MovementData>, RefRO<InputData>, RefRO<CombatData>>()) {
                if (combatData.ValueRO.AttackInProgress) {
                    playerData.ValueRW.Velocity = float2.zero;
                    return;
                }
                
                playerData.ValueRW.Velocity = inputData.ValueRO.MoveValue * playerData.ValueRO.Speed * fixedDeltaTime;
            }
        }
    }
}
