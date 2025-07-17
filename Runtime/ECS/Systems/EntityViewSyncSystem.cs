using System.Collections.Generic;
using Darkness.Runtime.ECS.Components;
using Darkness.Runtime.ECS.Components.Tags;
using Darkness.Runtime.Presentation;
using Unity.Entities;
using UnityEngine;
using VContainer;

namespace Darkness.Runtime.ECS.Systems {
    public partial class EntityViewSyncSystem : SystemBase {
        private Entity _registryEntity;
        private GameObject _prefab;
        private EntityViewManager _entityViewManager;

        [Inject]
        public void Construct(EntityViewManager entityViewManager) {
            _entityViewManager = entityViewManager;
        }

        protected override void OnCreate() {
            // Load your visual prefab (replace with Addressables if needed)
            _prefab = Resources.Load<GameObject>("VisualPrefab"); // Make sure prefab exists in Resources
        }

        protected override void OnUpdate() {
            if (_entityViewManager is null) {
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

                    var go = Object.Instantiate(_prefab);
                    go.transform.position = new Vector3(positionData.Position.x, positionData.Position.y, 0f);

                    var view = go.AddComponent<EntityView>();
                    view.Entity = entity;

                    _entityViewManager.Register(entity, go);
                    // Tag to mark view created
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
