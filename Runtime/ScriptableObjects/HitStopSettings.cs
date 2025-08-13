using DG.Tweening;
using UnityEngine;

namespace Darkness.Runtime.ScriptableObjects {
    [CreateAssetMenu(fileName = "HitStopSettings", menuName = "Game/Hit Stop Settings")]
    public class HitStopSettings : ScriptableObject {
        [Tooltip("Time in stopped state")]
        [field:SerializeField] public float Delay { get; private set; } = 0.08f;
        [Tooltip("Value passed to Time.timeScale")]
        [field: SerializeField] public float SlowScale { get; private set; }
        [Tooltip("Time to scale back to 1.0f. Set 0 for instant scale time back")]
        [field: SerializeField] public float ScaleBackTime { get; private set; }
        [field: SerializeField] public Ease ScaleBackEasing { get; private set; } = Ease.Linear;
    }
}