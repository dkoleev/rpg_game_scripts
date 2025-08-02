using UnityEngine;
using Yogi.UnityDevConsole.Scripts.Runtime;

namespace Darkness.Runtime.DevTools {
    public static class DevConsoleCommands {
        private static GameManager _gameManager;

        private static void Setup() {
            _gameManager ??= Object.FindAnyObjectByType<GameManager>();
        }
        
        [DevConsoleCommand("save")]
        public static void Save() {
            Setup();
            _gameManager.SaveSystem.Save();
        }
    }
}