using System.Threading.Tasks;
using Darkness.Runtime.Utils.CustomTypes;
using UnityEngine;

namespace Darkness.Runtime.Utils
{
    public static class ResourceLoader
    {
        public static AsyncResult<T> LoadAsync<T>(string path) where T : Object
        {
            var task = LoadResourceAsync<T>(path);
            return AsyncResult<T>.FromTask(task);
        }

        private static async Task<Result<T>> LoadResourceAsync<T>(string path) where T : Object
        {
            ResourceRequest request = Resources.LoadAsync<T>(path);
        
            // Wait for the async operation to complete
            while (!request.isDone)
            {
                await Task.Yield();
            }

            if (request.asset == null)
            {
                return Result<T>.Fail($"Asset of type {typeof(T)} not found at path: {path}");
            }

            return Result<T>.Success((T)request.asset);
        }
    }
}