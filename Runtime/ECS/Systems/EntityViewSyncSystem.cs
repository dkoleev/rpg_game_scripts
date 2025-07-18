using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Darkness.Runtime.ECS.Components;
using Darkness.Runtime.ECS.Components.Tags;
using Darkness.Runtime.Log;
using Darkness.Runtime.Presentation;
using Darkness.Runtime.Utils.Resource;
using Unity.Entities;
using UnityEngine;
using VContainer;

namespace Darkness.Runtime.ECS.Systems {
    public partial class EntityViewSyncSystem : SystemBase {
        private Entity _registryEntity;
        private EntityViewManager _entityViewManager;
        private GameLogger _gameLogger;

        [Inject]
        public void Construct(EntityViewManager entityViewManager, GameLogger gameLogger) {
            _entityViewManager = entityViewManager;
            _gameLogger = gameLogger;
        }

        protected override void OnCreate() { }

        protected override void OnUpdate() {
            if (_entityViewManager is null) {
                //_gameLogger.Warning($"{nameof(EntityViewSyncSystem)}: {nameof(EntityViewManager)} is null");
                return;
            }

            var entityManager = EntityManager;

            // 1. Create views for new entities
            Entities
                .WithNone<ViewLinkedTag>()
                .ForEach((Entity entity, in EntityViewData entityViewData) => {
                    if (_entityViewManager.HasView(entity)) {
                        return;
                    }

                    var isNPC = entityManager.HasComponent<NPCData>(entity);
                    var entityType = isNPC ? EntityType.NPC : EntityType.Player;
                    _entityViewManager.Register(
                        entity, entityType, entityViewData.PrefabPath.ToString(), entityViewData.SpawnPosition).Forget();
                    entityManager.AddComponent<ViewLinkedTag>(entity);
                }).WithStructuralChanges().WithoutBurst().Run();

            // 2. Destroy views if entity no longer exists
            var toRemove = new List<Entity>();
            foreach (var kv in _entityViewManager.Views) {
                if (!entityManager.Exists(kv.Key)) {
                    toRemove.Add(kv.Key);
                }
            }

            foreach (var entity in toRemove) {
                _entityViewManager.Unregister(entity);
            }
        }
    }
}
