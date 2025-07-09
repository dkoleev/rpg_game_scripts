using System;
using System.Threading.Tasks;
using Darkness.Runtime.Utils.CustomTypes;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Darkness.Runtime.Utils.Resource
{
   
    public static class AddressableLoader
    {
        public static AsyncResult<T> LoadAddressable<T>(string key)
        {
            var task = LoadAddressableAsync<T>(key);
            return AsyncResult<T>.FromTask(task);
        }

        private static async Task<Result<T>> LoadAddressableAsync<T>(string key)
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
        
            try
            {
                await handle.Task;
            
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    return Result<T>.Success(handle.Result);
                }
                else
                {
                    Addressables.Release(handle);
                    return Result<T>.Fail($"Failed to load addressable: {key}");
                }
            }
            catch (Exception e)
            {
                Addressables.Release(handle);
                return Result<T>.Fail($"Error loading addressable {key}: {e.Message}");
            }
        }
    }
}