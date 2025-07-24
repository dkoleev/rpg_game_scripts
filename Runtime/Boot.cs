using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Darkness.Runtime.Core;
using Darkness.Runtime.ECS.Components;
using Darkness.Runtime.ECS.Components.Tags;
using Darkness.Runtime.Log;
using Darkness.Runtime.Utils.Resource;
using DG.Tweening;
using SuperTiled2Unity;
using Unity.Cinemachine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Darkness.Runtime {
    public class Boot : IStartable, IDisposable {
        private readonly GameLogger _gameLogger;
        private readonly AddressableLoader _addressableLoader;

        [Inject]
        private Boot(GameLogger gameLogger, AddressableLoader addressableLoader) {
            _gameLogger = gameLogger;
            _addressableLoader = addressableLoader;
        }

        void IStartable.Start() {
            StartGame(RunMode.Full).Forget();
        }

        private async UniTask StartGame(RunMode runMode) {
            _gameLogger.SetLevel(GameLogger.LogLevel.All);
            DOTween.Init(false, false, LogBehaviour.Default).SetCapacity(100, 30);
            LoadGameData();
            LoadPlayerState();
            
            await LoadScenes();
            SpawnCharacters();            
        }
        
        private void LoadGameData() { }

        private void LoadPlayerState() { }
        
        private async UniTask LoadScenes() {
            // await LoadScene("Maps/Home");
            await LoadScene("Maps/Introduction");
            await LoadScene("CameraLightEvents");
            await LoadScene("Dialogues");
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Introduction"));
        }

        private void SpawnCharacters() {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            var spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
            foreach (var point in spawnPoints) {
                var scp = point.GetComponent<SuperCustomProperties>();
                if (scp.TryGetCustomProperty("character", out var character)) {
                    var characterId = character.m_Value;
                    if (characterId == "Player") {
                        var spawnPointEntity = entityManager.CreateEntity();
                        //TODO: get player view data from spawn point
                        entityManager.AddComponentData(spawnPointEntity, new SpawnPointData {
                            IsPlayerSpawnPoint = true,
                            SpawnPosition = new float2(point.transform.position.x, point.transform.position.y),
                            PrefabPath = $"Characters/Fantasy/JoannaDark.prefab"
                        });
                    }
                    else {
                        var spawnPointEntity = entityManager.CreateEntity();
                        entityManager.AddComponentData(spawnPointEntity, new SpawnPointData {
                            IsPlayerSpawnPoint = false,
                            SpawnPosition = new float2(point.transform.position.x, point.transform.position.y),
                            PrefabPath = $"Characters/Fantasy/{characterId}.prefab"
                        });
                    }
                }
            }
        }

        private void InitializeCamera(Transform targetTransform) {
            var brain = Camera.main.GetComponent<CinemachineBrain>();
            CinemachineCamera liveCam;
            if (brain.ActiveVirtualCamera is CinemachineCameraManagerBase managerCam)
                liveCam = managerCam.LiveChild as CinemachineCamera;
            else
                liveCam = brain.ActiveVirtualCamera as CinemachineCamera;

            liveCam.ForceCameraPosition(targetTransform.position, Quaternion.identity);
            liveCam.Follow = targetTransform;
            var cameraBounds = GameObject.FindWithTag("CameraBounds").GetComponent<Collider2D>();
            liveCam.GetComponent<CinemachineConfiner2D>().BoundingShape2D = cameraBounds;
        }

        private async UniTask LoadScene(string scenePath) {
            const string prefix = "Scenes/";
            const string postfix = ".unity";
            var finalPath = prefix + scenePath + postfix;

            var sceneResult = await _addressableLoader.LoadScene(finalPath, LoadSceneMode.Additive).Await();
            sceneResult.Match(
                scene => { _gameLogger.Log($"{sceneResult.Value.Scene.name} scene loaded successfully"); },
                error => { _gameLogger.Error($"Failed to load {sceneResult.Value.Scene.name} scene: {error}"); }
            );
        }

        public void Dispose() {
            //Cleanup
        }
    }
}