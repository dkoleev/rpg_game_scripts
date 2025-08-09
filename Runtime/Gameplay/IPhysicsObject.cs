using UnityEngine;

namespace Darkness.Runtime.Gameplay {
    public interface IPhysicsObject {
        Rigidbody2D Rigidbody2D { get; }
        Transform Transform { get; }
    }
}