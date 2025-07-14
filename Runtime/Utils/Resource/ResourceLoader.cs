using Cysharp.Threading.Tasks;
using Darkness.Runtime.Utils.CustomTypes;
using UnityEngine;

namespace Darkness.Runtime.Utils.Resource {
    public static class ResourceLoader {
        public static AsyncResult<T> LoadAsync<T>(string path) where T : Object {
            var task = LoadResourceAsync<T>(path);
            return AsyncResult<T>.FromTask(task);
        }

        private static async UniTask<Result<T>> LoadResourceAsync<T>(string path) where T : Object {
            var request = Resources.LoadAsync<T>(path);

            // Wait for the async operation to complete
            await request;

            if (request.asset == null) return Result<T>.Fail($"Asset of type {typeof(T)} not found at path: {path}");

            return Result<T>.Success((T)request.asset);
        }
    }
}