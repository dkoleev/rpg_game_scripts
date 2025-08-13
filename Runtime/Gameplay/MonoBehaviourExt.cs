using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Darkness.Runtime.Gameplay {
    /// <summary>
    /// Base class that provides access to the GameManager and waits until the game is ready.
    /// </summary>
    public class MonoBehaviourExt : MonoBehaviour {
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