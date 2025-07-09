using System.Threading.Tasks;

namespace Darkness.Runtime.Utils.CustomTypes
{
    public struct AsyncResult<T>
    {
        public Task<Result<T>> Task { get; }
    
        public AsyncResult(Task<Result<T>> task)
        {
            Task = task;
        }

        public async Task<Result<T>> Await()
        {
            return await Task;
        }

        public static AsyncResult<T> FromTask(Task<Result<T>> task) => new AsyncResult<T>(task);
    }
}