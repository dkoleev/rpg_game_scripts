using UnityEngine;
using UnityEngine.InputSystem;

namespace Darkness.Runtime.Gameplay.Player {
    public class PlayerInteractor {
        private readonly float _interactRadius = 0.5f;
        private LayerMask _interactableLayer;
        private IInteractable _currentTarget;
        private Transform _rootTransform;
        private InputAction _interactAction;

        private void Start() {
            _interactableLayer = LayerMask.GetMask("Interactable");
            _interactAction = InputSystem.actions.FindAction("Interact");
        }

        private void Update() {
            ScanForInteractables();

            if (_currentTarget != null) {
                ShowPrompt(_currentTarget.PromptMessage);

                if (_interactAction.WasPerformedThisFrame()) _currentTarget.Interact();
            }
            else {
                HidePrompt();
            }
        }

        private void ScanForInteractables() {
            var hits = Physics2D.OverlapCircleAll(_rootTransform.position, _interactRadius, _interactableLayer);
            _currentTarget = null;
            var closest = Mathf.Infinity;

            foreach (var hit in hits) {
                var interactable = hit.GetComponent<IInteractable>();
                if (interactable != null) {
                    var dist = Vector2.Distance(_rootTransform.position, hit.transform.position);
                    if (dist < closest) {
                        closest = dist;
                        _currentTarget = interactable;
                    }
                }
            }
        }

        private void ShowPrompt(string msg) {
            // Example: UIController.Instance.ShowInteractionPrompt(msg);
        }

        private void HidePrompt() {
            // UIController.Instance.HideInteractionPrompt();
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_rootTransform.position, _interactRadius);
        }
    }
}