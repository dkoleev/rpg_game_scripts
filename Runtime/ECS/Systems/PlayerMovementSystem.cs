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
                if (combatData.ValueRO.ActionInProgress) {
                    playerData.ValueRW.Velocity = float2.zero;
                    return;
                }
                
                playerData.ValueRW.Velocity = inputData.ValueRO.MoveValue * playerData.ValueRO.Speed * fixedDeltaTime;
                
                /*// Целевая скорость
                var targetVelocity = inputData.ValueRO.MoveValue * playerData.ValueRO.Speed * fixedDeltaTime;
                // Постепенное увеличение скорости
                // Интерполяция между текущей скоростью и целевой скоростью
                var interpolatedVelocity = math.lerp(playerData.ValueRW.Velocity, targetVelocity, fixedDeltaTime * 2.0f); // 5.0f - коэффициент, регулирующий скорость набора
                playerData.ValueRW.Velocity = math.min(targetVelocity, interpolatedVelocity);*/
            }
        }
    }
}
