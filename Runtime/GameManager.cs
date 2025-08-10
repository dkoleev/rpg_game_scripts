using Darkness.Runtime.Log;
using UnityEngine;
using VContainer;

namespace Darkness.Runtime {
    public class GameManager : MonoBehaviour {
        public SaveSystem SaveSystem { get; private set; }
        public GameLogger Logger { get; private set; }
        
        [Inject]
        public void Construct(SaveSystem saveSystem, GameLogger logger) {
            SaveSystem = saveSystem;
            Logger = logger;
        }
    }
}