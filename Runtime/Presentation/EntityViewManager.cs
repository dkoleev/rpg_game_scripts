using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Entities;
using UnityEngine;

namespace Darkness.Runtime.Presentation {
    [UsedImplicitly]
    public class EntityViewManager {
        private readonly Dictionary<Entity, GameObject> _views = new();

        public IReadOnlyDictionary<Entity, GameObject> Views => _views;

        public void Register(Entity entity, GameObject view)
        {
            _views[entity] = view;
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
