using Cysharp.Threading.Tasks;
using Darkness.Runtime.Log;
using UnityEngine;
using VContainer.Unity;

namespace Darkness.Runtime.DebTools {
    public class DebugHotKeys : ITickable {
        private readonly SaveSystem _saveSystem;
        private readonly GameLogger _gameLogger;

        public DebugHotKeys(SaveSystem saveSystem, GameLogger gameLogger) {
            _saveSystem = saveSystem;
            _gameLogger = gameLogger;
        }

        public void Tick() {
            if (UnityEngine.Input.GetKey(KeyCode.LeftControl) && UnityEngine.Input.GetKeyDown(KeyCode.S)) {
                _saveSystem.Save();
            }
        }
    }
}
