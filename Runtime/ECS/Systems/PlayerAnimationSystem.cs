using Darkness.Runtime.ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Darkness.Runtime.ECS.Systems {
    public partial struct PlayerAnimationSystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            foreach (var (playerAnimationData, inputData)
                     in SystemAPI.Query<RefRW<PlayerAnimationData>, RefRO<InputData>>()) {
                playerAnimationData.ValueRW.Moving = math.lengthsq(inputData.ValueRO.MoveValue) > 0f;
                playerAnimationData.ValueRW.Attacking = inputData.ValueRO.Attack;
            }
        }
    }
}
