using Darkness.Runtime.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace Darkness.Runtime.Presentation {
    public class PlayerView : MonoBehaviour {
        private static readonly int IsMovingAnimProperty = Animator.StringToHash("IsMoving");
        private static readonly int AttackAnimProperty = Animator.StringToHash("Attack");

        private Rigidbody2D _rb;
        private SpriteRenderer _characterSprite;
        private Animator _characterAnimator;

        private EntityManager _entityManager;
        private Entity _playerEntity;

        private void Start() {
            _rb = GetComponent<Rigidbody2D>();
            _characterSprite = GetComponentInChildren<SpriteRenderer>();
            _characterAnimator = GetComponentInChildren<Animator>();

            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _playerEntity = _entityManager.CreateEntityQuery(typeof(InputData)).GetSingletonEntity();
        }

        private void FixedUpdate() {
            if (!_entityManager.Exists(_playerEntity)) {
                return;
            }

            var playerData = _entityManager.GetComponentData<PlayerData>(_playerEntity);
            // _rb.MovePosition(new Vector2(playerData.Position.x, playerData.Position.y)); // Physics-based position update
            _rb.linearVelocity = new Vector2(playerData.Velocity.x, playerData.Velocity.y); // Physics-based position update
        }
    }
}
