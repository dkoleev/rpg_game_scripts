using Cysharp.Threading.Tasks;
using Darkness.Runtime.ScriptableObjects;
using Darkness.Runtime.Utils.Resource;
using UnityEngine.SceneManagement;
using VContainer;

namespace Darkness.Runtime.Gameplay.Levels {
    public class LevelsManager {
        private readonly LevelsListData _levelsListData;
        private readonly AddressableLoader _addressableLoader;

        [Inject]
        public LevelsManager(LevelsListData levelsListData, AddressableLoader addressableLoader) {
            _levelsListData = levelsListData;
            _addressableLoader = addressableLoader;
        }

        public async UniTask LoadLevel(LevelType levelType) {
            await _addressableLoader.LoadScene(_levelsListData.levels[levelType], LoadSceneMode.Additive);
        }
    }
}