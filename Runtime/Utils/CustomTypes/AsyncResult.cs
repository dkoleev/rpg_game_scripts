using Cysharp.Threading.Tasks;

namespace Darkness.Runtime.Utils.CustomTypes
{
    public struct AsyncResult<T>
    {
        public UniTask<Result<T>> Task { get; }

        public AsyncResult(UniTask<Result<T>> task)
        {
            Task = task;
        }

        public async UniTask<Result<T>> Await()
        {
            return await Task;
        }

        public static AsyncResult<T> FromTask(UniTask<Result<T>> task) => new AsyncResult<T>(task);
    }
}