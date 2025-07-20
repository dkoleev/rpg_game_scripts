using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Darkness.Runtime.ECS.Components;
using Darkness.Runtime.ECS.Components.Tags;
using Darkness.Runtime.Log;
using Darkness.Runtime.Presentation;
using Unity.Entities;
using Unity.Mathematics;
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
                .ForEach((Entity entity, in EntityViewData entityViewData, in PositionData positionData) => {
                    if (_entityViewManager.HasView(entity)) {
                        return;
                    }

                    var isNPC = entityManager.HasComponent<NPCTag>(entity);
                    var entityType = isNPC ? EntityType.NPC : EntityType.Player;
                    _entityViewManager.Register(
                        entity, entityType, entityViewData.PrefabPath.ToString(), positionData.Position).Forget();
                    entityManager.AddComponent<ViewLinkedTag>(entity);
                }).WithStructuralChanges().WithoutBurst().Run();

            Entities.WithAll<ViewLinkedTag>().ForEach((Entity entity, ref PositionData positionData) => {
                if (!_entityViewManager.HasView(entity)) {
                    return;
                }

                var view = _entityViewManager.Views[entity];
                positionData.Position = new float2(view.transform.position.x, view.transform.position.y);
            }).WithoutBurst().Run();

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
