using System;
using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using Darkness.Runtime.Gameplay.Npc.EnemyStates;
using Darkness.Runtime.ScriptableObjects;
using Drawing;
using Unity.Mathematics;
using UnityEngine;

namespace Darkness.Runtime.Gameplay.Npc {
    public class EnemyAI : MonoBehaviourGizmosExt, IHittable, IPhysicsObject {
        [Required] [SerializeField] private EnemyAISettings settings;

        /// <summary>
        /// Triggered when the enemy state changes.
        /// </summary>
        /// <param name="oldState">The state before transition.</param>
        /// <param name="newState">The state after transition.</param>
        public delegate void StateChangedHandler(IEnemyState oldState, IEnemyState newState);
        public event StateChangedHandler OnStateChanged;

        public Rigidbody2D Rigidbody2D => _rigidbody2D;
        public Transform Transform => transform;
        
        public Transform PlayerTransform => _playerTransform;

        private bool _isFacingRight => _currentDirection == 1;
        private Rigidbody2D _rigidbody2D;
        private Vector2 _startPosition;
        private int _currentDirection; // -1 = left, 1 = right
        private Transform _playerTransform;
        private bool _attackInProgress;

        private IEnemyState _currentStateMachine;

        private void Awake() {
            _currentDirection = settings.StartDirectionToRight ? 1 : -1;
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        protected override async UniTask Start() {
            await base.Start();
            ChangeState(new EnemyIdleState());
        }

        protected override void OnGameReady() {
            _startPosition = transform.position;
            _playerTransform = GameObject.FindWithTag("Player").transform;
        }
        
        public void ChangeState(IEnemyState newState) {
            var oldState = _currentStateMachine;
            
            _currentStateMachine?.Exit(this, settings);
            _currentStateMachine = newState;
            _currentStateMachine.Enter(this, settings);
            
            OnStateChanged?.Invoke(oldState, newState);

            GameManager.Logger.Log(oldState is null
                ? $"[Enemy]: State changed to {newState.GetType().Name}"
                : $"[Enemy]: State changed from {oldState.GetType().Name} to {newState.GetType().Name}");
        }

        private void Update() {
            if (!GameIsReady) {
                return;
            }

            _currentStateMachine.Update(this, settings);
        }
        
        public void TakeHit(int damage) {
            
        }
        
        private void ReturnToStartPoint() {
            _currentDirection = _startPosition.x > transform.position.x ? 1 : -1;
            Move();
            if (Vector2.Distance(transform.position, _startPosition) < 0.1f) {
                // _currentState = EnemyState.Idle;
            }
        }
        
        private void Stunned() { }

        private void Dead() {
            _rigidbody2D.linearVelocity = Vector2.zero;
        }

        public void Move() {
            var dir  = _isFacingRight ? Vector2.right : Vector2.left;
            transform.Translate(dir * (_currentDirection * settings.MoveSpeed * Time.deltaTime));
        }

        public void ChangeDirection() {
            _currentDirection *= -1;
            UpdateFacing();
        }

        public void SetDirection(int direction) {
            _currentDirection = direction;
            UpdateFacing();
        }

        public bool IsGroundAhead() {
            return Physics2D.Raycast(
                transform.position + Vector3.right * (_currentDirection * settings.GroundCheckForwardDistance),
                Vector2.down, settings.GroundCheckDownDistance,
                settings.GroundLayer);
        }

        public bool IsWallAhead() {
            return Physics2D.Raycast(transform.position, Vector2.right * _currentDirection, settings.WallCheckDistance,
                settings.WallLayer);
        }
        
        private void UpdateFacing() {
            transform.rotation = Quaternion.Euler(0f, _isFacingRight ? 0f : 180f, 0f);
        }

        public override void DrawGizmos() {
            base.DrawGizmos();

            // Ground check ray
            var groundStart = transform.position +
                              Vector3.right * (_currentDirection * settings.GroundCheckForwardDistance);
            var groundEnd = groundStart + Vector3.down * settings.GroundCheckDownDistance;
            var groundColor = IsGroundAhead() ? Color.green : Color.red;

            using (Draw.WithColor(groundColor)) {
                using (Draw.WithLineWidth(3)) {
                    Draw.Line(groundStart, groundEnd);
                    DrawLabel(groundStart, groundEnd, $"groundCheck: {settings.GroundCheckDownDistance:F2}");
                }
            }

            // Wall check ray
            var wallStart = transform.position;
            var wallEnd = wallStart + Vector3.right * _currentDirection * 0.5f;
            var wallColor = IsWallAhead() ? Color.red : Color.green;

            using (Draw.WithColor(wallColor)) {
                using (Draw.WithLineWidth(3)) {
                    Draw.Line(wallStart, wallEnd);
                    DrawLabel(wallStart, wallEnd, $"wallCheck: {settings.WallCheckDistance:F2}");
                }
            }

            // Attack range visualization
            var attackStart = transform.position + new Vector3(0, 0.1f);
            var attackEnd = attackStart + Vector3.right * _currentDirection * settings.AttackRange;

            using (Draw.WithColor(Color.yellow)) {
                using (Draw.WithLineWidth(3f)) {
                    Draw.Line(attackStart, attackEnd);
                    DrawLabel(attackStart, attackEnd, $"attack: {settings.AttackRange:F2}");
                }
            }

            // Add label above the middle of the line
            // Vector3 attackMid = (attackStart + attackEnd) * 0.5f;// + Vector3.up * 0.1f; // shift up 0.1

            // Optional: draw a small circle at attack end
            using (Draw.WithColor(Color.yellow)) {
                using (Draw.WithLineWidth(2f)) {
                    var center = new float3(attackEnd.x, attackEnd.y, attackEnd.z);
                    var normal = new float3(0f, 0f, 1f); // Z-axis normal for XY plane
                    Draw.Circle(center, normal, 0.05f);
                }
            }


            // Chase range visualization
            var chaseStart = transform.position + new Vector3(0, 0.2f);
            var chaseEnd = chaseStart + Vector3.right * _currentDirection * settings.ChaseRange;

            using (Draw.WithColor(Color.blueViolet)) {
                using (Draw.WithLineWidth(3f)) {
                    Draw.Line(chaseStart, chaseEnd);
                    DrawLabel(chaseStart, chaseEnd, $"chase: {settings.ChaseRange:F2}");
                }
            }

            return;

            void DrawLabel(Vector3 startPoint, Vector3 endPoint, string text, float size = 30) {
                var mid = (startPoint + endPoint) * 0.5f + Vector3.up * 0.03f;
                Draw.Label2D(mid, text, size);
            }
        }
    }
}
