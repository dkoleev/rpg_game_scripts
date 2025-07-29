using UnityEngine;

namespace Darkness.Runtime.Gameplay.Player {
    public class PlayerViewData {
        public readonly Rigidbody2D Rigidbody;
        public readonly Animator Animator;

        public PlayerViewData(Rigidbody2D rigidbody, Animator animator) {
            Rigidbody = rigidbody;
            Animator = animator;
        }
    }
}