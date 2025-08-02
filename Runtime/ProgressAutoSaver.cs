using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Darkness.Runtime {
    public class ProgressAutoSaver : IAsyncStartable, IDisposable {
        private readonly SaveSystem _progress;
        private CancellationTokenSource _cts;

        public ProgressAutoSaver(SaveSystem saveSystem)
        {
            _progress = saveSystem;
        }
        
        public async UniTask StartAsync(CancellationToken cancellation = new CancellationToken()) {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
            PeriodicSaveAsync(_cts.Token).Forget();
        }
        
        private async UniTaskVoid PeriodicSaveAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(30), cancellationToken: token);
                    await _progress.SaveAsync();
                }
            }
            catch (OperationCanceledException) { }
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}