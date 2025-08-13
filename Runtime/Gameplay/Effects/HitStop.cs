using System;
using Cysharp.Threading.Tasks;
using Darkness.Runtime.ScriptableObjects;
using UnityEngine;
using DG.Tweening;

namespace Darkness.Runtime.Gameplay.Effects {
    public class HitStop : MonoBehaviour {
        [SerializeField] private HitStopSettings settings;
        
        public async UniTask Stop(HitStopSettings hitStopSettings) {
            Time.timeScale = hitStopSettings.SlowScale;
            await UniTask.Delay(TimeSpan.FromSeconds(hitStopSettings.Delay), ignoreTimeScale: true);
            if (hitStopSettings.ScaleBackTime <= 0) {
                Time.timeScale = 1f;
            }
            else {
                DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, hitStopSettings.ScaleBackTime)
                    .SetUpdate(UpdateType.Normal, true).SetEase(hitStopSettings.ScaleBackEasing);
            }
        } 

        public async UniTask Stop() {
            Time.timeScale = settings.SlowScale;
            await UniTask.Delay(TimeSpan.FromSeconds(settings.Delay), ignoreTimeScale: true);
            if (settings.ScaleBackTime <= 0) {
                Time.timeScale = 1f;
            }
            else {
                DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, settings.ScaleBackTime)
                    .SetUpdate(UpdateType.Normal, true).SetEase(settings.ScaleBackEasing);
            }
        }
    }
}
