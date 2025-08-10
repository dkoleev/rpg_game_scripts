using Drawing;

namespace Darkness.Runtime.Gameplay {
    public class MonoBehaviourGizmosExt : MonoBehaviourGizmos {
        protected GameManager GameManager { get; private set; }
        
        protected virtual void Awake() {
            GameManager = FindAnyObjectByType<GameManager>();
        }
    }
}