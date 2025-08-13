using Alchemy.Inspector;
using Darkness.Runtime.ScriptableObjects;
using DG.Tweening;
using UnityEngine;

namespace Darkness.Runtime.Gameplay.Effects {
    public class FlashDamage : MonoBehaviour {
        private static readonly int FlashColor = Shader.PropertyToID("_FlashColor");
        private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");

        [SerializeField] private bool defaultSettings;
        [ShowIf("defaultSettings")]
        [SerializeField] private FlashDamageSettings settings;

        private SpriteRenderer[] _spriteRenderers;
        private Material[] _materials;

        private void Awake() {
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            _materials = new Material[_spriteRenderers.Length];
            for (var i = 0; i < _spriteRenderers.Length; i++) {
                _materials[i] = _spriteRenderers[i].material;
            }
        }

        public void Flash() {
            foreach (var material in _materials) {
                material.SetColor(FlashColor, settings.Color);
                material.DOFloat(1, FlashAmount, settings.Duration).SetEase(settings.Curve);
            }
        }
        
        public void Flash(FlashDamageSettings flashDamageSettings) {
            foreach (var material in _materials) {
                material.SetColor(FlashColor, flashDamageSettings.Color);
                material.DOFloat(1, FlashAmount, flashDamageSettings.Duration).SetEase(flashDamageSettings.Curve);
            }
        }
    }
}
