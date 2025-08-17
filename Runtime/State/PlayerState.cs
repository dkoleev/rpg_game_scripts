using System;
using Darkness.Runtime.Gameplay.Levels;
using UnityEngine;

namespace Darkness.Runtime.State {
    [Serializable]
    public class PlayerState {
        public bool isFacingRight = true;
        public LevelType currentLocation = LevelType.Tutorial;
        public Vector2 currentPosition = Vector2.zero;
        public int currentHealth = 100;
    }
}