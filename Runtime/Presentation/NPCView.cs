using System;
using Darkness.Runtime.ECS.Components;
using Darkness.Runtime.ECS.Components.Tags;
using Darkness.Runtime.Log;
using PixelCrushers.DialogueSystem;
using Unity.Entities;
using UnityEngine;
using VContainer;

namespace Darkness.Runtime.Presentation {
    public class NPCView : MonoBehaviour {
        public Entity Entity;
        
        private DialogueSystemTrigger _dialogueSystemTrigger;
        private GameLogger _gameLogger;
        private Entity _npcEntity;
        private EntityManager _entityManager;

        [Inject]
        private void Construct(GameLogger gameLogger) {
            _gameLogger = gameLogger;
        }
        
        private void Awake() {
            _dialogueSystemTrigger = GetComponent<DialogueSystemTrigger>();
        }

        private void Start() {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            DialogueManager.instance.conversationEnded += OnConversationEnded;
        }

        private void OnConversationEnded(Transform t) {
            if (t.gameObject.name != gameObject.name) {
                return;
            }
            
            _entityManager.RemoveComponent<DialogueInProgressTag>(Entity);
        }

        private void Update() {
            if (_entityManager.HasComponent<DialogueInProgressTag>(Entity)) {
                StartDialogue();
            }
        }

        private void StartDialogue() {
            if (DialogueManager.IsConversationActive) {
                return;
            }
            
            if (_dialogueSystemTrigger is null) {
                _gameLogger.Warning("DialogueSystemTrigger is null", gameObject);
                return;
            }

            _dialogueSystemTrigger.OnUse();    
        }

        private void OnDestroy() {
            DialogueManager.instance.conversationEnded -= OnConversationEnded;
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(gameObject.transform.position, 2);
        }
    }
}