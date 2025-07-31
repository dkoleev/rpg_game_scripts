using Cysharp.Threading.Tasks;
using Darkness.Runtime.ScriptableObjects;
using Darkness.Runtime.Utils.Resource;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VContainer;

namespace Darkness.Runtime.Gameplay.Levels {
    public class LevelsManager {
        private readonly LevelsList _levelsList;
        private readonly AddressableLoader _addressableLoader;

        [Inject]
        public LevelsManager(LevelsList levelsList, AddressableLoader addressableLoader) {
            _levelsList = levelsList;
            _addressableLoader = addressableLoader;
        }

        public async UniTask<SceneInstance> LoadLevel(LevelType levelType, bool activateOnLoad = true ) {
            var sceneInstance = await _addressableLoader.LoadScene(_levelsList.levels[levelType], LoadSceneMode.Additive, activateOnLoad);
            return sceneInstance;
        }
        
        public void UnloadLevel(SceneInstance sceneInstance) {
            _addressableLoader.UnloadSceneAsync(sceneInstance).Forget();
        }
    }
}