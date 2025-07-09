using UnityEngine;

namespace Darkness.Runtime.Utils.Resource
{
    public class ResourcesExample
    {
        private async void LoadResources()
        {
            GameObject go = null;
            var textureResult = await ResourceLoader.LoadAsync<Texture2D>("Textures/Background").Await();
            textureResult.Match(
                texture => go.GetComponent<Renderer>().material.mainTexture = texture,
                Debug.LogError
            );

            // Using Addressables
            var prefabResult = await AddressableLoader.LoadAddressable<GameObject>("Enemies/Orc").Await();
            prefabResult.Match(
                prefab => Object.Instantiate(prefab, Vector3.zero, Quaternion.identity),
                Debug.LogError
            );
            
            var result = await ResourceLoaderWithProgress.LoadWithProgress<AudioClip>(
                "Audio/Music", 
                progress => Debug.Log($"Loading: {progress * 100}%")
            );
        }
    }
}