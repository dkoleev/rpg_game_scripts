using Unity.Entities;
using UnityEngine;

namespace Darkness.Runtime.ECS.Components {
    public struct PlayerInputData : IComponentData {
        public Vector2 MoveDirection;
    }
}