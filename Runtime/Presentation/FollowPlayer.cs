using System;
using Darkness.Runtime.ECS.Components;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine;

namespace Darkness.Runtime.Presentation {
    public class FollowPlayer : MonoBehaviour {
        private FollowerEntity _ai;
        
        private void Awake() {
            _ai = GetComponent<FollowerEntity>();
            _ai.updateRotation = false;
        }
        
        private void Update() {
            bool hasPlayer = true;
            if (hasPlayer ) {
                _ai.destination = Vector3.zero;
            }
        }
    }
}