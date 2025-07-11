using System;
using Darkness.Runtime.Core;
using Darkness.Runtime.Log;
using DG.Tweening;
using VContainer;
using VContainer.Unity;

namespace Darkness.Runtime
{
    public class Boot : IStartable, IDisposable
    {
        private readonly GameLogger _gameLogger;

        [Inject]
        private Boot(GameLogger gameLogger)
        {
            _gameLogger = gameLogger;
        }
        
        void IStartable.Start()
        {
            StartGame(RunMode.Full);
        }

        private void StartGame(RunMode runMode)
        {
            _gameLogger.SetLevel(GameLogger.LogLevel.All);
            DOTween.Init(false, false, LogBehaviour.Default).SetCapacity(100, 30);
            LoadGameData();
            LoadPlayerState();
            LoadLevel();
        }

        private void LoadGameData()
        {
            
        }

        private void LoadPlayerState()
        {
            
        }

        private async void LoadLevel()
        {
            var sceneResult = await Utils.Resource.AddressableLoader.LoadScene("Scenes/Home").Await();

            sceneResult.Match(
                onSuccess: scene => {
                    _gameLogger.Log("Home scene loaded successfully");
                },
                onFailure: error => {
                    _gameLogger.Error($"Failed to load Home scene: {error}");
                }
            );
        }

        public void Dispose()
        {
            //Cleanup
        }
    }
}
