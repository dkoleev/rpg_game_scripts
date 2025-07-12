using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Darkness.Runtime.Core;
using Darkness.Runtime.Log;
using DG.Tweening;
using UnityEngine.SceneManagement;
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
            StartGame(RunMode.Full).Forget();
        }

        private async UniTask StartGame(RunMode runMode)
        {
            _gameLogger.SetLevel(GameLogger.LogLevel.All);
            DOTween.Init(false, false, LogBehaviour.Default).SetCapacity(100, 30);
            LoadGameData();
            LoadPlayerState();
            await LoadScenes();
        }

        private void LoadGameData()
        {
            
        }

        private void LoadPlayerState()
        {
            
        }

        private async UniTask LoadScenes()
        {
            await LoadScene("Player");
            await LoadScene("Home");
        }

        private async UniTask LoadScene(string scenePath)
        {
            var prefix = "Scenes/";
            var postfix = ".unity";
            var finalPath = prefix + scenePath + postfix;
            
            var sceneResult = await Utils.Resource.AddressableLoader.LoadScene(finalPath, LoadSceneMode.Additive).Await();
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
