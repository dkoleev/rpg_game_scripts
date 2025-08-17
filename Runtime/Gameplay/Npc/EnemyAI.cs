using System;
using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using Darkness.Runtime.ScriptableObjects;
using Drawing;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Darkness.Runtime.Gameplay.Npc {
    public class EnemyAI : MonoBehaviourGizmosExt {
        [Required] [SerializeField] private EnemyAISettings settings;

        private Rigidbody2D _rigidbody2D;
        private EnemyState _currentState;
        private Vector2 _startPosition;
        private int _currentDirection; // -1 = left, 1 = right
        private Transform _playerTransform;
        private bool _attackInProgress;
        private Animator _animator;

        private void Awake() {
            _currentDirection = settings.StartDirectionToRight ? 1 : -1;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponentInChildren<Animator>();
        }

        protected override void OnGameReady() {
            _startPosition = transform.position;
            _playerTransform = GameObject.FindWithTag("Player").transform;
        }

        private void Update() {
            if (!GameIsReady) {
                return;
            }

            switch (_currentState) {
                case EnemyState.Idle:
                    Idle().Forget();
                    break;
                case EnemyState.Patrol:
                    Patrol();
                    break;
                case EnemyState.Chase:
                    Chase();
                    break;
                case EnemyState.Attack:
                    Attack().Forget();
                    break;
                case EnemyState.ReturnToStartPoint:
                    ReturnToStartPoint();
                    break;
                case EnemyState.Stunned:
                case EnemyState.Dead:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async UniTask Idle() {
            var token = this.GetCancellationTokenOnDestroy();
            await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(2f, 4f)), cancellationToken: token);
            Patrol();

            var idleTime = Random.Range(2f, 4f);
            var elapsed = 0f;

            while (elapsed < idleTime) {
                if (_playerTransform != null) {
                    await UniTask.Yield(); // ждать следующий кадр
                    return;
                }
                
                // Если игрок в зоне атаки
                if (Vector2.Distance(transform.position, _playerTransform.position) < settings.AttackRange) {
                    _currentState = EnemyState.Attack;
                    return;
                }

                // Если игрок в зоне погони
                if (Vector2.Distance(transform.position, _playerTransform.position) < settings.ChaseRange) {
                    _currentState = EnemyState.Chase;
                    return;
                }

                await UniTask.Yield(); // ждать следующий кадр
                elapsed += Time.deltaTime;
            }

            Patrol();
        }

        private void Patrol() {
            Move();
            if (!IsGroundAhead() || IsWallAhead()) {
                _currentDirection *= -1;
            }

            if (Vector2.Distance(transform.position, _playerTransform.position) < settings.AttackRange) {
                _currentState = EnemyState.Attack;
                return;
            }

            if (Vector2.Distance(transform.position, _playerTransform.position) < settings.ChaseRange) {
                _currentState = EnemyState.Chase;
            }
        }

        private void Chase() {
            _currentDirection = _playerTransform.position.x > transform.position.x ? 1 : -1;
            Move();

            var distance = Vector2.Distance(transform.position, _playerTransform.position);
            if (distance > settings.ChaseRange + 1.0f) {
                _currentState = EnemyState.Patrol;
            }
            else if (distance <= settings.AttackRange) {
                _currentState = EnemyState.Attack;
            }
        }

        private void ReturnToStartPoint() {
            _currentDirection = _startPosition.x > transform.position.x ? 1 : -1;
            Move();
            if (Vector2.Distance(transform.position, _startPosition) < 0.1f) {
                _currentState = EnemyState.Idle;
            }
        }

        private async UniTaskVoid Attack() {
            if (_attackInProgress) {
                return;
            }

            var distance = Vector2.Distance(transform.position, _playerTransform.position);
            if (distance > settings.AttackRange) {
                _currentState = EnemyState.Chase;
                return;
            }

            _attackInProgress = true;
            //TODO: Attack
            GameManager.Logger.Log($"{gameObject.name}: perform attack");
            await UniTask.Delay(TimeSpan.FromSeconds(settings.AttackCooldown));
            _attackInProgress = false;
        }

        private void Stunned() { }

        private void Dead() {
            _rigidbody2D.linearVelocity = Vector2.zero;
        }

        private void Move() {
            transform.Translate(Vector2.right * (_currentDirection * settings.MoveSpeed * Time.deltaTime));
        }

        private bool IsGroundAhead() {
            return Physics2D.Raycast(
                transform.position + Vector3.right * (_currentDirection * settings.GroundCheckForwardDistance),
                Vector2.down, settings.GroundCheckDownDistance,
                settings.GroundLayer);
        }

        private bool IsWallAhead() {
            return Physics2D.Raycast(transform.position, Vector2.right * _currentDirection, settings.WallCheckDistance,
                settings.WallLayer);
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
