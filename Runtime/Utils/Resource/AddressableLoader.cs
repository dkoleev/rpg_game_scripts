using System;
using Cysharp.Threading.Tasks;
using Darkness.Runtime.Utils.CustomTypes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Darkness.Runtime.Utils.Resource {
    public class AddressableLoader {
        public AsyncResult<T> LoadAddressable<T>(string key) {
            var task = LoadAddressableAsync<T>(key);
            return AsyncResult<T>.FromTask(task);
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

        public AsyncResult<SceneInstance> LoadScene(string key, LoadSceneMode loadMode = LoadSceneMode.Single) {
            var task = LoadSceneAsync(key, loadMode);
            return AsyncResult<SceneInstance>.FromTask(task);
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
    }
}