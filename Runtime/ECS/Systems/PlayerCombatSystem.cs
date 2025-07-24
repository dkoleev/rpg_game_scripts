using Darkness.Runtime.ECS.Components;
using Unity.Entities;

namespace Darkness.Runtime.ECS.Systems {
    public partial struct PlayerCombatSystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            foreach (var (combatData, inputData) in SystemAPI.Query<RefRW<CombatData>, RefRW<InputData>>()) {
                if (combatData.ValueRO.CurrentActionTime > 0) {
                    combatData.ValueRW.CurrentActionTime -= SystemAPI.Time.DeltaTime;
                }
                else {
                    combatData.ValueRW.ActionInProgress = false;
                }

                if (combatData.ValueRO.ActionInProgress) {
                    return;
                }

                combatData.ValueRW.IsSlowAttack = false;
                combatData.ValueRW.IsRoll = false;
                combatData.ValueRW.IsAttack = false;

                if (inputData.ValueRO.Attack) {
                    combatData.ValueRW.IsAttack = true;
                    combatData.ValueRW.ActionInProgress = true;
                    combatData.ValueRW.CurrentActionTime = 0.5f;
                }
                else if (inputData.ValueRO.SlowAttack) {
                    combatData.ValueRW.IsSlowAttack = true;
                    combatData.ValueRW.ActionInProgress = true;
                    combatData.ValueRW.CurrentActionTime = 1.0f;
                }
                else if (inputData.ValueRO.Roll) {
                    combatData.ValueRW.IsRoll = true;
                    combatData.ValueRW.ActionInProgress = true;
                    combatData.ValueRW.CurrentActionTime = 1.0f;
                }

                inputData.ValueRW.Attack = false;
                inputData.ValueRW.SlowAttack = false;
                inputData.ValueRW.Roll = false;
            }
        }
    }
}