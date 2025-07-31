using AYellowpaper.SerializedCollections;
using Darkness.Runtime.Gameplay;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Darkness.Runtime.ScriptableObjects {
    [CreateAssetMenu(fileName = "CharactersListData", menuName = "Game/Characters List Data")]
    public class CharactersList : ScriptableObject {
        [SerializedDictionary("Character Type", "Character Asset")]
        public SerializedDictionary<CharacterType, AssetReferenceGameObject> characters;
    }
}