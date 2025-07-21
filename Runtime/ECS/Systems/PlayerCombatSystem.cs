using Darkness.Runtime.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace Darkness.Runtime.ECS.Systems {
    public partial struct PlayerCombatSystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            foreach (var (combatData, inputData) in SystemAPI.Query<RefRW<CombatData>, RefRO<InputData>>()) {
                if (inputData.ValueRO.Attack && !combatData.ValueRO.AttackInProgress) {
                    combatData.ValueRW.AttackInProgress = true;
                    combatData.ValueRW.AttackTime = 0.5f;
                }
                
                if (combatData.ValueRO.AttackTime > 0) {
                    combatData.ValueRW.AttackTime -= SystemAPI.Time.DeltaTime;
                }
                else {
                    combatData.ValueRW.AttackInProgress = false;
                }
            }
        }
    }
}