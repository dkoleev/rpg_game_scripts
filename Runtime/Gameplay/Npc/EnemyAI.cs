using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Drawing;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Darkness.Runtime.Gameplay.Npc {
    public class EnemyAI : MonoBehaviourGizmosExt {
        [SerializeField] private EnemyState enterState = EnemyState.Idle; 
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float chaseSpeed = 2f;
        [SerializeField] private float chaseRange = 5f;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private bool startDirectionToRight = true;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask wallLayer;
        
        private Rigidbody2D _rigidbody2D;
        
        private EnemyState _currentState;
        private Vector2 _startPosition; 
        private int _currentDirection; // -1 = left, 1 = right
        private Transform _playerTransform;
        private bool _attackInProgress;
        
        private void Awake() {
            _currentDirection = startDirectionToRight ? 1 : -1;
            _rigidbody2D = GetComponent<Rigidbody2D>();
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
            await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(2f, 4f)));
            Patrol();

            var idleTime = Random.Range(2f, 4f);
            var elapsed = 0f;

            while (elapsed < idleTime) {
                // Если игрок в зоне атаки
                if (Vector2.Distance(transform.position, _playerTransform.position) < attackRange) {
                    _currentState = EnemyState.Attack;
                    return;
                }

                // Если игрок в зоне погони
                if (Vector2.Distance(transform.position, _playerTransform.position) < chaseRange) {
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

            if (Vector2.Distance(transform.position, _playerTransform.position) < attackRange) {
                _currentState = EnemyState.Attack;
                return;
            }

            if (Vector2.Distance(transform.position, _playerTransform.position) < chaseRange) {
                _currentState = EnemyState.Chase;
            }
        }

        private void Chase() {
            _currentDirection = _playerTransform.position.x > transform.position.x ? 1 : -1;
            Move();

            var distance = Vector2.Distance(transform.position, _playerTransform.position);
            if (distance > chaseRange + 1.0f) {
                _currentState = EnemyState.Patrol;
            }
            else if (distance <= attackRange) {
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
            if (distance > attackRange) {
                _currentState = EnemyState.Chase;
                return;
            }

            _attackInProgress = true;
            //TODO: Attack
            GameManager.Logger.Log($"{gameObject.name}: perform attack");
            await UniTask.Delay(TimeSpan.FromSeconds(attackCooldown));
            _attackInProgress = false;
        }

        private void Stunned() {
            
        }

        private void Dead() {
            _rigidbody2D.linearVelocity = Vector2.zero;
        }

        private void Move() {
            Debug.Log("Move");
            transform.Translate(Vector2.right * (_currentDirection * moveSpeed * Time.deltaTime));
        }

        private bool IsGroundAhead() {
            return Physics2D.Raycast(transform.position + Vector3.right * (_currentDirection * 0.5f), Vector2.down, 1f,
                groundLayer);
        }

        private bool IsWallAhead() {
            return Physics2D.Raycast(transform.position, Vector2.right * _currentDirection, 0.5f, wallLayer);
        }

        public override void DrawGizmos() {
            base.DrawGizmos();

            // Ground check ray
            Vector3 groundStart = transform.position + Vector3.right * (_currentDirection * 0.5f);
            Vector3 groundEnd = groundStart + Vector3.down * 1f;
            Color groundColor = IsGroundAhead() ? Color.green : Color.red;

            using (Draw.WithColor(groundColor)) {
                using (Draw.WithLineWidth(3)) {
                    Draw.Line(groundStart, groundEnd);
                }
            }

            // Wall check ray
            Vector3 wallStart = transform.position;
            Vector3 wallEnd = wallStart + Vector3.right * _currentDirection * 0.5f;
            Color wallColor = IsWallAhead() ? Color.red : Color.green;

            using (Draw.WithColor(wallColor)) {
                using (Draw.WithLineWidth(3)) {
                    Draw.Line(wallStart, wallEnd);
                }
            }
        
            // Attack range visualization
            Vector3 attackStart = transform.position + new Vector3(0, 0.1f);
            Vector3 attackEnd = attackStart + Vector3.right * _currentDirection * attackRange;

            using (Draw.WithColor(Color.yellow))
            using (Draw.WithLineWidth(3f))
            {
                Draw.Line(attackStart, attackEnd);
            }

            // Optional: draw a small circle at attack end
            using (Draw.WithColor(Color.yellow))
            using (Draw.WithLineWidth(2f))
            {
                float3 center = new float3(attackEnd.x, attackEnd.y, attackEnd.z);
                float3 normal = new float3(0f, 0f, 1f); // Z-axis normal for XY plane
                Draw.Circle(center, normal, 0.05f);
            }
            
            
            // Chase range visualization
            Vector3 chaseStart = transform.position + new Vector3(0, 0.2f);
            Vector3 chaseEnd = chaseStart + Vector3.right * _currentDirection * chaseRange;

            using (Draw.WithColor(Color.blueViolet))
            using (Draw.WithLineWidth(3f))
            {
                Draw.Line(chaseStart, chaseEnd);
            }
        }
    }
}