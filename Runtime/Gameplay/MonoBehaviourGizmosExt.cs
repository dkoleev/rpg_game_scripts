using Cysharp.Threading.Tasks;
using Drawing;
using UnityEngine;

namespace Darkness.Runtime.Gameplay {
    public class MonoBehaviourGizmosExt : MonoBehaviourGizmos {
        /// <summary>
        /// Cached GameManager instance.
        /// </summary>
        protected GameManager GameManager { get; private set; }
        /// <summary>
        /// Indicates whether the game is ready.
        /// </summary>
        protected bool GameIsReady => GameManager.GameIsReady;

        protected virtual async UniTask Start() {
            GameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
            if (GameManager == null) {
                Debug.LogError("GameManager not found in the scene. Please ensure a GameManager instance exists.");
                return;
            }

            await UniTask.WaitUntil(() => GameIsReady);
            OnGameReady();
        }

        protected virtual void OnGameReady() { }
    }
}