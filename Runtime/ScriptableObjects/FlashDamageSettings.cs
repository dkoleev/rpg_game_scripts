using UnityEngine;

namespace Darkness.Runtime.ScriptableObjects {
    [CreateAssetMenu(fileName = "FlashDamageSettings", menuName = "Game/Flash Damage Settings")]
    public class FlashDamageSettings : ScriptableObject {
        [ColorUsage(true, true)]
        [field: SerializeField] public Color Color { get; private set; } = Color.white;
        [field: SerializeField] public float Duration { get; private set; } = 0.1f;
        [field: SerializeField] public AnimationCurve Curve { get; private set; }
    }
}