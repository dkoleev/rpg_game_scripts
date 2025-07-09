using System;
using System.Threading.Tasks;
using Darkness.Runtime.Utils.CustomTypes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Darkness.Runtime.Utils.Resource
{
    public static class ResourceLoaderWithProgress
    {
        public static async Task<Result<T>> LoadWithProgress<T>(string path, Action<float> onProgress = null) where T : Object
        {
            ResourceRequest request = Resources.LoadAsync<T>(path);
        
            while (!request.isDone)
            {
                onProgress?.Invoke(request.progress);
                await Task.Yield();
            }

            onProgress?.Invoke(1f);

            if (request.asset == null)
            {
                return Result<T>.Fail($"Asset not found at path: {path}");
            }

            return Result<T>.Success((T)request.asset);
        }
    }
}