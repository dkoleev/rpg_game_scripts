using System;
using Cysharp.Threading.Tasks;
using Darkness.Runtime.Utils.CustomTypes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Darkness.Runtime.Utils.Resource {
    public static class ResourceLoaderWithProgress {
        public static async UniTask<Result<T>> LoadWithProgress<T>(string path, Action<float> onProgress = null)
            where T : Object {
            var request = Resources.LoadAsync<T>(path);

            await request.ToUniTask(Progress.Create<float>(progress => { onProgress?.Invoke(progress); }));

            onProgress?.Invoke(1f);

            if (request.asset == null) return Result<T>.Fail($"Asset not found at path: {path}");

            return Result<T>.Success((T)request.asset);
        }

        public static AsyncResult<T> LoadWithProgressAsync<T>(string path, Action<float> onProgress = null)
            where T : Object {
            return AsyncResult<T>.FromTask(LoadWithProgress<T>(path, onProgress));
        }
    }
}