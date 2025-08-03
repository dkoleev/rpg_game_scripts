using System.Collections.Generic;
using Darkness.Runtime.Gameplay.Levels;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Darkness.Runtime.ScriptableObjects {
    [CreateAssetMenu(fileName = "BootSettings", menuName = "Game/Boot Settings")]
    public class BootSettings : ScriptableObject {
        [Tooltip("The default scenes to load when the game starts")]
        [Header("Scenes")]
        [field:SerializeField] public List<LevelType> DefaultScenes { get; private set; }
        [field:SerializeField] public List<AssetReference> AdditionalScenesToLoad { get; private set; }
    }
}