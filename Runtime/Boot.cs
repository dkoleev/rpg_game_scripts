using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Darkness.Runtime.Core;
using Darkness.Runtime.ECS.Components;
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
            
            InitializeECS();
            await LoadScenes();
            await SpawnCharacters();            
            InitializeCamera(GameObject.FindWithTag("Player").transform);
        }

        private void InitializeECS() {
            var world = World.DefaultGameObjectInjectionWorld;
            var entityManager = world.EntityManager;
            var playerArchetype = entityManager.CreateArchetype(
                typeof(PlayerData),
                typeof(InputData),
                typeof(PlayerAnimationData)
            );
            // Create player entity
            var playerEntity = entityManager.CreateEntity(playerArchetype);
            // Initialize player data
            entityManager.SetComponentData(playerEntity, new PlayerData
            {
                Velocity = float2.zero,
                Speed = 50f
            });
            // Initialize input data
            entityManager.SetComponentData(playerEntity, new InputData
            {
                MoveValue = float2.zero
            });
            entityManager.SetComponentData(playerEntity, new PlayerAnimationData
            {
                Moving = false
            });
        }

        private void LoadGameData() { }

        private void LoadPlayerState() { }
        
        private async UniTask LoadScenes() {
            // await LoadScene("Maps/Home");
            await LoadScene("Maps/Introduction");
            await LoadScene("CameraAndLighting");
            await LoadScene("Player");
            await LoadScene("Dialogues");
        }

        private async UniTask SpawnCharacters() {
            var player = GameObject.FindWithTag("Player");
            var spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
            foreach (var point in spawnPoints) {
                var scp = point.GetComponent<SuperCustomProperties>();
                if (scp.TryGetCustomProperty("character", out var character)) {
                    var characterId = character.m_Value;
                    if (characterId == "Player") {
                        player.transform.position = point.transform.position;
                    }
                    else {
                        var prefabResult = await _addressableLoader
                            .LoadAddressable<GameObject>($"Characters/{characterId}.prefab").Await();
                        prefabResult.Match(
                            prefab => {
                                var instance = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
                                instance.transform.position = point.transform.position;
                            },
                            Debug.LogError
                        );
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