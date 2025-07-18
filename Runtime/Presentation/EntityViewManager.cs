using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Darkness.Runtime.ECS;
using Darkness.Runtime.Utils.Resource;
using JetBrains.Annotations;
using Unity.Entities;
using UnityEngine;

namespace Darkness.Runtime.Presentation {
    [UsedImplicitly]
    public class EntityViewManager {
        public IReadOnlyDictionary<Entity, GameObject> Views => _views;
        
        private readonly Dictionary<Entity, GameObject> _views = new();
        private readonly AddressableLoader _addressableLoader;

        public EntityViewManager(AddressableLoader addressableLoader) {
            _addressableLoader = addressableLoader;
        }

        public async UniTask Register(Entity entity, EntityType entityType, string prefabPath, Vector2 spawnPosition) {
            var prefabResult = await _addressableLoader.LoadAddressable<GameObject>(prefabPath).Await();
            prefabResult.Match(
                prefab => {
                    var instance = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
                    instance.transform.position = spawnPosition;
                    var entityView = instance.AddComponent<EntityView>();
                    entityView.Entity = entity;
                    if (entityType == EntityType.NPC) {
                        var npcView = instance.AddComponent<NPCView>();
                        npcView.Entity = entity;
                    }
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
    }
}
