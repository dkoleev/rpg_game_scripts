using Cysharp.Threading.Tasks;
using Darkness.Runtime.Log;
using Darkness.Runtime.ScriptableObjects;
using Darkness.Runtime.Utils.Resource;
using UnityEngine;

namespace Darkness.Runtime.Gameplay {
    public class SpawnManager {
        private readonly AddressableLoader _addressableLoader;
        private readonly CharactersListData _charactersListData;
        private readonly GameLogger _gameLogger;

        public SpawnManager(AddressableLoader addressableLoader,
            CharactersListData charactersListData,
            GameLogger gameLogger) {
            _addressableLoader = addressableLoader;
            _charactersListData = charactersListData;
            _gameLogger = gameLogger;
        }

        public async UniTask<GameObject> SpawnCharacter(CharacterType characterType, Vector3 position) {
            var assetRef = _charactersListData.characters[characterType];
            var asset = await _addressableLoader.LoadAddressable<GameObject>(assetRef);
            var result = Object.Instantiate(asset, position, Quaternion.identity);

            return result;
        }
    }
}
