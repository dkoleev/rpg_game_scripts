using AYellowpaper.SerializedCollections;
using Darkness.Runtime.Gameplay.Levels;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Darkness.Runtime.ScriptableObjects {
    [CreateAssetMenu(fileName = "LevelsListData", menuName = "Game/Levels List Data")]
    public class LevelsList : ScriptableObject {
        public SerializedDictionary<LevelType, AssetReference> levels;
    }
}