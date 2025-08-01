using System.IO;
using Cysharp.Threading.Tasks;
using Darkness.Runtime.Log;
using Darkness.Runtime.State;
using UnityEngine;

namespace Darkness.Runtime {
    public class SaveSystem {
        private readonly GameLogger _gameLogger;
        public GameState Current { get; private set; }
        private readonly string _savePath;

        private bool _progressWasLoaded;

        public SaveSystem(GameLogger gameLogger) {
            _gameLogger = gameLogger;
            Current = new GameState();
            _savePath = Path.Combine(Application.persistentDataPath, "progress.json");
        }
        
        public void Save() {
            if (!_progressWasLoaded) {
                return;
            }
            
            string json = JsonUtility.ToJson(Current, true); // true = prettyPrint
            File.WriteAllText(_savePath, json);
            _gameLogger.Log("Game saved successufully");
        }

        public void Load() {
            if (!File.Exists(_savePath)) {
                Current = new GameState();
                _progressWasLoaded = true;
                return;
            }

            var json = File.ReadAllText(_savePath);
            Current = JsonUtility.FromJson<GameState>(json);
            _progressWasLoaded = true;
        }

        public async UniTask SaveAsync() {
            if (!_progressWasLoaded) {
                return;
            }
            
            var json = JsonUtility.ToJson(Current, true);
            await UniTask.RunOnThreadPool(() => { File.WriteAllText(_savePath, json); });
            _gameLogger.Log("Game saved successufully");
        }

        public async UniTask LoadAsync() {
            if (!File.Exists(_savePath)) {
                Current = new GameState();
                _progressWasLoaded = true;
                return;
            }

            var json = await UniTask.RunOnThreadPool(() => File.ReadAllText(_savePath));
            Current = JsonUtility.FromJson<GameState>(json);

            _progressWasLoaded = true;
        }
    }
}
