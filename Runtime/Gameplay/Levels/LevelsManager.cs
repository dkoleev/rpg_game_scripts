using Cysharp.Threading.Tasks;
using Darkness.Runtime.ScriptableObjects;
using Darkness.Runtime.Utils.Resource;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VContainer;

namespace Darkness.Runtime.Gameplay.Levels {
    public class LevelsManager {
        private readonly LevelsList _levelsList;
        private readonly AddressableLoader _addressableLoader;
        private readonly SaveSystem _saveSystem;

        [Inject]
        public LevelsManager(LevelsList levelsList, AddressableLoader addressableLoader, SaveSystem saveSystem) {
            _levelsList = levelsList;
            _addressableLoader = addressableLoader;
            _saveSystem = saveSystem;
        }

        public async UniTask<SceneInstance> LoadLevel(LevelType levelType, bool activateOnLoad = true ) {
            var sceneInstance = await LoadLevel(_levelsList.levels[levelType], activateOnLoad);;
            _saveSystem.Save();
            return sceneInstance;
        }
        
        public async UniTask<SceneInstance> LoadLevel(AssetReference sceneRef, bool activateOnLoad = true ) {
            var sceneInstance = await _addressableLoader.LoadScene(sceneRef, LoadSceneMode.Additive, activateOnLoad);
            _saveSystem.Save();
            return sceneInstance;
        }
        
        public void UnloadLevel(SceneInstance sceneInstance) {
            _addressableLoader.UnloadSceneAsync(sceneInstance).Forget();
        }
    }
}