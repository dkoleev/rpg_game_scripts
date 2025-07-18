using System;
using Darkness.Runtime.ECS.Components;
using Darkness.Runtime.Log;
using PixelCrushers.DialogueSystem;
using Unity.Entities;
using UnityEngine;

namespace Darkness.Runtime.Presentation {
    public class NPCView : MonoBehaviour {
        public Entity Entity;
        
        private DialogueSystemTrigger _dialogueSystemTrigger;
        private GameLogger _gameLogger;
        private Entity _npcEntity;
        private EntityManager _entityManager;

        private void Construct(GameLogger gameLogger) {
            _gameLogger = gameLogger;
        }
        
        private void Awake() {
            _dialogueSystemTrigger = GetComponent<DialogueSystemTrigger>();
        }

        private void Start() {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        public void StartDialogue() {
            if (_dialogueSystemTrigger is null) {
                _gameLogger.Warning("DialogueSystemTrigger is null", gameObject);
                return;
            }

            _dialogueSystemTrigger.OnUse();    
        }
        
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(gameObject.transform.position, 2);
        }
    }
}