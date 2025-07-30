using System;
using Cysharp.Threading.Tasks;
using Darkness.Runtime.Log;
using Darkness.Runtime.Utils.CustomTypes;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Darkness.Runtime.Utils.Resource {
    public class AddressableLoader {
        private readonly GameLogger _gameLogger;

        public AddressableLoader(GameLogger gameLogger) {
            _gameLogger = gameLogger;
        }
        
        public AsyncResult<T> LoadAddressable<T>(string key) {
            var task = LoadAddressableAsync<T>(key);
            return AsyncResult<T>.FromTask(task);
        }
        
        public async UniTask<T> LoadAddressable<T>(AssetReference assetReference) {
            var task = LoadAddressableAsync<T>(assetReference);
            var result = await AsyncResult<T>.FromTask(task).Await();

            return result.Value;
        }

        private async UniTask<Result<T>> LoadAddressableAsync<T>(string key) {
            var handle = Addressables.LoadAssetAsync<T>(key);

            try {
                await handle.ToUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded) {
                    return Result<T>.Success(handle.Result);
                }
                else {
                    Addressables.Release(handle);
                    return Result<T>.Fail($"Failed to load addressable: {key}");
                }
            }
            catch (Exception e) {
                Addressables.Release(handle);
                return Result<T>.Fail($"Error loading addressable {key}: {e.Message}");
            }
        }
        
        private async UniTask<Result<T>> LoadAddressableAsync<T>(AssetReference assetReference) {
            
            var handle = assetReference.LoadAssetAsync<T>();
            try {
                await handle.ToUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded) {
                    return Result<T>.Success(handle.Result);
                }
                else {
                    Addressables.Release(handle);
                    return Result<T>.Fail($"Failed to load addressable: {assetReference.RuntimeKey}");
                }
            }
            catch (Exception e) {
                Addressables.Release(handle);
                return Result<T>.Fail($"Error loading addressable {assetReference.RuntimeKey}: {e.Message}");
            }
        }

        public AsyncResult<SceneInstance> LoadScene(string key, LoadSceneMode loadMode = LoadSceneMode.Single) {
            var task = LoadSceneAsync(key, loadMode);
            return AsyncResult<SceneInstance>.FromTask(task);
        }
        
        public async UniTask<SceneInstance> LoadScene(AssetReference assetReference, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true) {
            var task = LoadSceneAsync(assetReference, loadMode, activateOnLoad);
            var sceneResult = await AsyncResult<SceneInstance>.FromTask(task).Await();
            sceneResult.Match(
                scene => {
                    _gameLogger.Log($"{sceneResult.Value.Scene.name} scene loaded successfully");
                },
                error => { _gameLogger.Error($"Failed to load {sceneResult.Value.Scene.name} scene: {error}"); }
            );

            return sceneResult.Value;
        }
        
        private async UniTask<Result<SceneInstance>> LoadSceneAsync(AssetReference assetReference,
            LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true) {
            var handle = assetReference.LoadSceneAsync(loadMode, activateOnLoad);

            try {
                await handle.ToUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded) {
                    return Result<SceneInstance>.Success(handle.Result);
                }
                else {
                    Addressables.Release(handle);
                    return Result<SceneInstance>.Fail($"Failed to load scene: {assetReference.RuntimeKey}");
                }
            }
            catch (Exception e) {
                Addressables.Release(handle);
                return Result<SceneInstance>.Fail($"Error loading scene {assetReference.RuntimeKey}: {e.Message}");
            }
        }

        private async UniTask<Result<SceneInstance>> LoadSceneAsync(string key,
            LoadSceneMode loadMode = LoadSceneMode.Single) {
            var handle = Addressables.LoadSceneAsync(key, loadMode);

            try {
                await handle.ToUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded) {
                    return Result<SceneInstance>.Success(handle.Result);
                }
                else {
                    Addressables.Release(handle);
                    return Result<SceneInstance>.Fail($"Failed to load scene: {key}");
                }
            }
            catch (Exception e) {
                Addressables.Release(handle);
                return Result<SceneInstance>.Fail($"Error loading scene {key}: {e.Message}");
            }
        }

        public AsyncResult<SceneInstance> LoadSceneWithProgress(string key,
            LoadSceneMode loadMode = LoadSceneMode.Single, Action<float> onProgress = null) {
            var task = LoadSceneWithProgressAsync(key, loadMode, onProgress);
            return AsyncResult<SceneInstance>.FromTask(task);
        }

        private async UniTask<Result<SceneInstance>> LoadSceneWithProgressAsync(string key,
            LoadSceneMode loadMode = LoadSceneMode.Single, Action<float> onProgress = null) {
            var handle = Addressables.LoadSceneAsync(key, loadMode);

            try {
                await handle.ToUniTask(Progress.Create<float>(progress => { onProgress?.Invoke(progress); }));

                onProgress?.Invoke(1f);

                if (handle.Status == AsyncOperationStatus.Succeeded) {
                    return Result<SceneInstance>.Success(handle.Result);
                }
                else {
                    Addressables.Release(handle);
                    return Result<SceneInstance>.Fail($"Failed to load scene: {key}");
                }
            }
            catch (Exception e) {
                Addressables.Release(handle);
                return Result<SceneInstance>.Fail($"Error loading scene {key}: {e.Message}");
            }
        }
        
        public async UniTask<Result<bool>> UnloadSceneAsync(SceneInstance sceneInstance) {
            var unloadHandle = Addressables.UnloadSceneAsync(sceneInstance);

            try {
                await unloadHandle.ToUniTask();

                if (unloadHandle.Status == AsyncOperationStatus.Succeeded) {
                    return Result<bool>.Success(true);
                }
                else {
                    return Result<bool>.Fail("Failed to unload scene.");
                }
            }
            catch (Exception e) {
                return Result<bool>.Fail($"Error unloading scene: {e.Message}");
            }
        }
    }
}