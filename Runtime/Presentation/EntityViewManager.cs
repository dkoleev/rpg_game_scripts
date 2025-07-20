using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Darkness.Runtime.ECS;
using Darkness.Runtime.Utils.Resource;
using JetBrains.Annotations;
using Unity.Cinemachine;
using Unity.Entities;
using UnityEngine;

namespace Darkness.Runtime.Presentation {
    [UsedImplicitly]
    public class EntityViewManager {
        private const string PlayerRootPath = "Common/Player_Root.prefab";
        private const string NPCRootPath = "Common/NPC_Root.prefab";
        public IReadOnlyDictionary<Entity, GameObject> Views => _views;
        
        private readonly Dictionary<Entity, GameObject> _views = new();
        private readonly AddressableLoader _addressableLoader;

        public EntityViewManager(AddressableLoader addressableLoader) {
            _addressableLoader = addressableLoader;
        }

        public async UniTask Register(Entity entity, EntityType entityType, string prefabPath, Vector2 spawnPosition) {
            var rootPrefab =  
                await _addressableLoader.LoadAddressable<GameObject>(entityType == EntityType.Player ? PlayerRootPath : NPCRootPath).Await();

            if (entityType == EntityType.Player) {
                InitializeCamera(rootPrefab.Value.transform);
            }

            var prefabResult = await _addressableLoader.LoadAddressable<GameObject>(prefabPath).Await();
            prefabResult.Match(
                prefab => {
                    var instance = Object.Instantiate(rootPrefab.Value, Vector3.zero, Quaternion.identity);
                    instance.transform.position = spawnPosition;
                    var entityView = instance.AddComponent<EntityView>();
                    switch (entityType) {
                        case EntityType.Player: {
                            var playerView = instance.AddComponent<PlayerView>();
                            playerView.Entity = entity;
                            break;
                        }
                        case EntityType.NPC: {
                            var npcView = instance.AddComponent<NPCView>();
                            npcView.Entity = entity;
                            break;
                        }
                    }
                    entityView.Entity = entity;
                    
                    var viewInstance = Object.Instantiate(prefab, instance.transform);
                    viewInstance.transform.localPosition = Vector3.zero;
                    viewInstance.transform.localRotation = Quaternion.identity;
                    
                    _views[entity] = instance;
                },
                Debug.LogError
            );
        }

        public void Unregister(Entity entity) {
            if (!_views.TryGetValue(entity, out var view)) {
                return;
            }
            
            Object.Destroy(view);
            _views.Remove(entity);
        }

        public bool HasView(Entity entity) => _views.ContainsKey(entity);
        
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

    }
}
