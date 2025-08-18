using Alchemy.Inspector;
using UnityEngine;

namespace Darkness.Runtime.Gameplay.Effects {
    public class ParallaxBackground : MonoBehaviourExt {
        [HelpBox("Far background (sky) → factor ~0.4\n" + 
                 "Middle background (ruins) → factor ~0.3\n" +
                 "Near background (ruins) → factor ~0.2\n" +
                 "Foreground far → factor ~ -0.1 (moves slightly faster than player)\n" +
                 "Foreground near → factor ~ -0.2 (moves more faster than player)\n")]
        [SerializeField] private float parallaxFactor;

        private Transform _cam;
        private float _lastCamPosX;
        private bool _initialized;

        protected override void OnGameReady() {
            _cam = UnityEngine.Camera.main.transform;
            _lastCamPosX = _cam.position.x;
            _initialized = true;
        }

        private void Update() {
            if (!_initialized) {
                return;
            }
            
            if(Mathf.Approximately(_cam.position.x, _lastCamPosX))
            {
                return;
            }
            
            var delta = _lastCamPosX - _cam.position.x;
            _lastCamPosX = _cam.position.x;
            
            var newPos = transform.localPosition;
            newPos.x -= delta * parallaxFactor;
            transform.localPosition = newPos;
        }
    }
}
