using UnityEngine;

namespace Darkness.Runtime.Gameplay {
    public class MonoBehaviourExt : MonoBehaviour {
        protected GameManager GameManager { get; private set; }
        
        protected virtual void Awake() {
            GameManager = FindAnyObjectByType<GameManager>();
        }
    }
}