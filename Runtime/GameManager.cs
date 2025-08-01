using UnityEngine;
using VContainer;

namespace Darkness.Runtime {
    public class GameManager : MonoBehaviour {
        public SaveSystem SaveSystem { get; private set; }
        
        [Inject]
        public void Construct(SaveSystem saveSystem) {
            SaveSystem = saveSystem;
        }
    }
}