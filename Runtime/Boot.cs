using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Darkness.Runtime.Core;
using Darkness.Runtime.Log;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

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
            // await LoadScene("Home");
            await LoadScene("Introduction");
            await LoadScene("CameraAndLighting");
            await LoadScene("Player");

            var player = GameObject.FindWithTag("Player");
            var spawnPoint = GameObject.FindWithTag("PlayerSpawnPoint");
            player.transform.position = spawnPoint.transform.position;
            
            var brain = Camera.main.GetComponent<CinemachineBrain>();
            CinemachineCamera liveCam;
            if (brain.ActiveVirtualCamera is CinemachineCameraManagerBase managerCam)
            {
                liveCam = managerCam.LiveChild as CinemachineCamera;
            }
            else
            {
                liveCam = brain.ActiveVirtualCamera as CinemachineCamera;
            }

            liveCam.ForceCameraPosition(player.transform.position, Quaternion.identity);
            liveCam.Follow = player.transform;
            var cameraBounds = GameObject.FindWithTag("CameraBounds").GetComponent<Collider2D>();
            liveCam.GetComponent<CinemachineConfiner2D>().BoundingShape2D = cameraBounds;
        }

        private async UniTask LoadScene(string scenePath)
        {
            const string prefix = "Scenes/";
            const string postfix = ".unity";
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
