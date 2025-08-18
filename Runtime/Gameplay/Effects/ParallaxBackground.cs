using UnityEngine;

namespace Darkness.Runtime.Gameplay.Effects {
    public class ParallaxBackground : MonoBehaviourExt {
        // Far background (sky) → factor ~0.05
        // Closer background (ruins) → factor ~0.3
        // Foreground decor → factor ~1.2 (moves slightly faster than player)
        [SerializeField] [Range(0f, 2f)] private float factorX = 0.5f;
        // [SerializeField] [Range(0f, 2f)] private float factorY = 0.5f;
        [SerializeField] private float pixelsPerUnit = 32f; // совпадает с PPU у спрайтов

        private Transform _cam;
        private Vector3 _lastCamPos;
        private bool _initialized;

        protected override void OnGameReady() {
            _cam = UnityEngine.Camera.main.transform;
            _lastCamPos = _cam.position;
            _initialized = true;
        }
        

        private void LateUpdate() {
            if (!_initialized) {
                return;
            }
            
            var delta = _cam.position - _lastCamPos;
        
            // Сдвигаем фон
            var newPos = transform.position + new Vector3(
                delta.x * factorX,
                0,// delta.y * factorY,
                0
            );
        
            // Округляем к ближайшему пикселю
            newPos.x = Mathf.Round(newPos.x * pixelsPerUnit) / pixelsPerUnit;
            newPos.y = Mathf.Round(newPos.y * pixelsPerUnit) / pixelsPerUnit;
        
            transform.position = newPos;
            _lastCamPos = _cam.position;
        }
    }
}
