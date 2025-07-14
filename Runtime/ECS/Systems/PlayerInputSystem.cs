using Darkness.Runtime.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace Darkness.Runtime.ECS.Systems {
    public partial class PlayerInputSystem : SystemBase {
        protected override void OnUpdate() {
            Entities.ForEach((ref PlayerInputData inputData) => { inputData.MoveDirection = Vector2.zero; })
                .ScheduleParallel();
        }
    }
}