using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Darkness.Runtime.Messages;
using Darkness.Runtime.ScriptableObjects;
using Drawing;
using MessagePipe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Darkness.Runtime.Gameplay.Player {
    public class PlayerPlatformerAttack : MonoBehaviourGizmosExt {
        private const int MaxLightAttackIndex = 1;
        
        [SerializeField] private PlayerAttackSettings settings;

        public event Action<PlayerAttackSettings.AttackType> OnPerformAttack;
        public event Action<PlayerAttackSettings.AttackType> OnHit;
        public event Action<bool> OnBlocking;
        public bool IsBlocking { get; private set; }
        public bool AttackInProgress { get; private set; }
        public bool LightAttackInFinalStageProgress { get; private set; }

        private ISubscriber<InputMessage> _inputSubscriber;
        private IDisposable _disposable;
        private Rigidbody2D _rb;
        private PlayerPlatformerMovement _movement;
        private PlayerInput _playerInput;
        private int _lightAttackSeriesIndex;
        private CancellationTokenSource _attackCancellationSource;

        protected override void Awake() {
            base.Awake();
            
            _rb = GetComponent<Rigidbody2D>();
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Start() {
            SetupSubscribers();
        }

        private void SetupSubscribers() {
            _inputSubscriber = GlobalMessagePipe.GetSubscriber<InputMessage>();
            var disposableBagBuilder = DisposableBag.CreateBuilder();
            _inputSubscriber.Subscribe(OnInput).AddTo(disposableBagBuilder);
            _disposable = disposableBagBuilder.Build();
        }

        private void OnInput(InputMessage data) {
            switch (data.Type) {
                case InputMessage.InputType.Attack:
                    if (AttackInProgress) {
                        if (LightAttackInFinalStageProgress) {
                            _lightAttackSeriesIndex++;
                            _attackCancellationSource?.Cancel(); // Cancel any ongoing attack
                            _attackCancellationSource = new CancellationTokenSource();

                            Attack(false, _attackCancellationSource.Token).Forget();
                        }
                        return;
                    }

                    _attackCancellationSource = new CancellationTokenSource();
                    if (data.Phase == InputMessage.InputPhase.Performed) {
                        Attack(data.IsSlowAttack, _attackCancellationSource.Token).Forget();
                    }

                    break;
                case InputMessage.InputType.Roll:
                    break;
                case InputMessage.InputType.Block:
                    switch (data.Phase) {
                        case InputMessage.InputPhase.Started:
                            break;
                        case InputMessage.InputPhase.Performed:
                            IsBlocking = true;
                            OnBlocking?.Invoke(true);
                            break;
                        case InputMessage.InputPhase.Cancelled:
                            IsBlocking = false;
                            OnBlocking?.Invoke(false);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async UniTaskVoid Attack(bool isSlow, CancellationToken cancellationToken) {
            GameManager.Logger.Log("[Attack]: Start attack");
            _playerInput.DeactivateInput();
            LightAttackInFinalStageProgress = false;
            
            if (_lightAttackSeriesIndex > MaxLightAttackIndex) {
                _lightAttackSeriesIndex = 0;
            }
            
            AttackInProgress = true;
            OnPerformAttack?.Invoke(isSlow
                ? PlayerAttackSettings.AttackType.Slow
                : GetLightAttackType());
            
            if (cancellationToken.IsCancellationRequested) {
                CleanupAfterAttack();
                return;
            }
            
            if (isSlow) {
                await UniTask.Delay(TimeSpan.FromSeconds(0.360f), cancellationToken: cancellationToken);
                if (cancellationToken.IsCancellationRequested) {
                    CleanupAfterAttack();
                    return;
                }

                PerformAttack(PlayerAttackSettings.AttackType.Slow);
                
                await UniTask.Delay(TimeSpan.FromSeconds(0.720f), cancellationToken: cancellationToken);
                if (cancellationToken.IsCancellationRequested) {
                    CleanupAfterAttack();
                    return;
                }
            }
            else {
                if (_lightAttackSeriesIndex == 0) {
                    await UniTask.Delay(TimeSpan.FromSeconds(0.300f), cancellationToken: cancellationToken);
                    if (cancellationToken.IsCancellationRequested) {
                        CleanupAfterAttack();
                        return;
                    }

                    PerformAttack(PlayerAttackSettings.AttackType.Light);
                    LightAttackInFinalStageProgress = true;
                    await UniTask.Delay(TimeSpan.FromSeconds(0.420f), cancellationToken: cancellationToken);
                    if (cancellationToken.IsCancellationRequested) {
                        CleanupAfterAttack();
                        return;
                    }

                    LightAttackInFinalStageProgress = false;
                }
                else if (_lightAttackSeriesIndex == 1) {
                    await UniTask.Delay(TimeSpan.FromSeconds(0.300f), cancellationToken: cancellationToken);
                    if (cancellationToken.IsCancellationRequested) {
                        CleanupAfterAttack();
                        return;
                    }

                    PerformAttack(PlayerAttackSettings.AttackType.UpLight);
                    LightAttackInFinalStageProgress = true;
                    await UniTask.Delay(TimeSpan.FromSeconds(0.360f), cancellationToken: cancellationToken);
                    if (cancellationToken.IsCancellationRequested) {
                        CleanupAfterAttack();
                        return;
                    }

                    LightAttackInFinalStageProgress = false;
                    _lightAttackSeriesIndex = 0;
                }
            }

            CleanupAfterAttack();
        }
        
        private void CleanupAfterAttack() {
            AttackInProgress = false;
            _playerInput.ActivateInput();
            GameManager.Logger.Log("[Attack]: Finish attack");
        }

        private void PerformAttack(PlayerAttackSettings.AttackType attackType) {
            var hitBoxPos = (Vector2)transform.position + (Vector2)(transform.rotation * settings.hitBoxOffset);
            var hits = Physics2D.OverlapBoxAll(hitBoxPos, settings.hitboxSize, settings.attackAngle,
                settings.enemyLayer);
            foreach (var hit in hits) {
                var isHit = false;
                var hitComponent = hit.GetComponent<IHittable>();
                if (hitComponent is not null) {
                    hitComponent.TakeHit(0);

                    var physicsComponent = hit.GetComponent<IPhysicsObject>();
                    if (physicsComponent is not null) {
                        var attackDir = (physicsComponent.Transform.position - transform.position).normalized;
                        var knockDir = new Vector2(attackDir.x * settings.enemyKnockbackForce,
                            settings.enemyBounceUpForce);
                        physicsComponent.Rigidbody2D.AddForce(knockDir, ForceMode2D.Impulse);
                    }

                    isHit = true;
                    OnHit?.Invoke(attackType);
                }

                if (isHit) {
                    if (settings.playerBounceOnHit) {
                        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);
                        _rb.AddForce(Vector2.up * settings.playerKnockbackForce, ForceMode2D.Impulse);
                    }

                    break;
                }
            }
        }

        private PlayerAttackSettings.AttackType GetLightAttackType() {
            switch (_lightAttackSeriesIndex) {
                case 0:
                    return PlayerAttackSettings.AttackType.Light;
                case 1:
                    return PlayerAttackSettings.AttackType.UpLight;
            }

            return PlayerAttackSettings.AttackType.Light;
        }

        public void OnDestroy() {
            _disposable?.Dispose();
        }

        public override void DrawGizmos() {
            using (Draw.WithColor(Color.darkRed)) {
                using (Draw.InLocalSpace(transform)) {
                    using (Draw.WithMatrix(Matrix4x4.TRS(
                               settings.hitBoxOffset,
                               Quaternion.Euler(0, 0, settings.attackAngle), Vector3.one))) {
                        Draw.WireBox(float3.zero, new float3(settings.hitboxSize.x, settings.hitboxSize.y, 0));
                    }
                }
            }
        }
    }
}
