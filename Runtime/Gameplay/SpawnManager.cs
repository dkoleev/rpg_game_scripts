using Cysharp.Threading.Tasks;
using Darkness.Runtime.Log;
using Darkness.Runtime.ScriptableObjects;
using Darkness.Runtime.Utils.Resource;
using UnityEngine;

namespace Darkness.Runtime.Gameplay {
    public class SpawnManager {
        private readonly AddressableLoader _addressableLoader;
        private readonly CharactersList _charactersList;
        private readonly GameLogger _gameLogger;

        public SpawnManager(AddressableLoader addressableLoader,
            CharactersList charactersList,
            GameLogger gameLogger) {
            _addressableLoader = addressableLoader;
            _charactersList = charactersList;
            _gameLogger = gameLogger;
        }

        public async UniTask<GameObject> SpawnCharacter(CharacterType characterType, Vector3 position) {
            var assetRef = _charactersList.characters[characterType];
            var asset = await _addressableLoader.LoadAddressable<GameObject>(assetRef);
            var result = Object.Instantiate(asset, position, Quaternion.identity);

            return result;
        }
    }
}
