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
            foreach (var (playerData, inputData) in SystemAPI.Query<RefRW<PlayerData>, RefRO<InputData>>()) {
                playerData.ValueRW.Velocity = inputData.ValueRO.MoveValue * playerData.ValueRO.Speed * fixedDeltaTime;
            }
        }
    }
}
