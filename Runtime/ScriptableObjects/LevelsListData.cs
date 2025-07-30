using AYellowpaper.SerializedCollections;
using Darkness.Runtime.Gameplay.Levels;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Darkness.Runtime.ScriptableObjects {
    [CreateAssetMenu(fileName = "LevelsListData", menuName = "Game/Levels List Data")]
    public class LevelsListData : ScriptableObject {
        public SerializedDictionary<LevelType, AssetReference> levels;
    }
}