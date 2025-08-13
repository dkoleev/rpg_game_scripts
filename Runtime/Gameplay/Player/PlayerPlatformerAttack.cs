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
        private const int MaxLightAttackIndex = 2;
        
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

        private void Awake() {
            _rb = GetComponent<Rigidbody2D>();
            _playerInput = GetComponent<PlayerInput>();
            _movement = GetComponent<PlayerPlatformerMovement>();
        }

        protected override void OnGameReady() {
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
            LightAttackInFinalStageProgress = false;
            
            if (_lightAttackSeriesIndex > MaxLightAttackIndex) {
                _lightAttackSeriesIndex = 0;
            }
            
            AttackInProgress = true;
            if (_movement.IsSitting) {
                OnPerformAttack?.Invoke(PlayerAttackSettings.AttackType.Sit);
            }
            else {
                OnPerformAttack?.Invoke(isSlow ? PlayerAttackSettings.AttackType.Slow : GetLightAttackType());
            }

            if (isSlow) {
                await UniTask.Delay(TimeSpan.FromSeconds(0.360f), cancellationToken: cancellationToken);

                PerformAttack(PlayerAttackSettings.AttackType.Slow);
                
                await UniTask.Delay(TimeSpan.FromSeconds(0.720f), cancellationToken: cancellationToken);
            }
            else {
                if (_lightAttackSeriesIndex == 0) {
                    await UniTask.Delay(TimeSpan.FromSeconds(0.300f), cancellationToken: cancellationToken);

                    PerformAttack(PlayerAttackSettings.AttackType.Main);
                    
                    LightAttackInFinalStageProgress = true;
                    await UniTask.Delay(TimeSpan.FromSeconds(0.180f), cancellationToken: cancellationToken);
                    LightAttackInFinalStageProgress = false;
                    if (_lightAttackSeriesIndex == 1) {
                        _attackCancellationSource?.Cancel();
                        _attackCancellationSource = new CancellationTokenSource();
                        Attack(false, _attackCancellationSource.Token).Forget();
                        return;
                    }
                    await UniTask.Delay(TimeSpan.FromSeconds(0.300f), cancellationToken: cancellationToken);
                }
                else if (_lightAttackSeriesIndex == 1) {
                    await UniTask.Delay(TimeSpan.FromSeconds(0.300f), cancellationToken: cancellationToken);
                    
                    PerformAttack(PlayerAttackSettings.AttackType.MainCombo1);
                    
                    LightAttackInFinalStageProgress = true;
                    await UniTask.Delay(TimeSpan.FromSeconds(0.180f), cancellationToken: cancellationToken);
                    LightAttackInFinalStageProgress = false;
                    if (_lightAttackSeriesIndex == 2) {
                        _attackCancellationSource?.Cancel();
                        _attackCancellationSource = new CancellationTokenSource();
                        Attack(false, _attackCancellationSource.Token).Forget();
                        return;
                    }
                    
                    await UniTask.Delay(TimeSpan.FromSeconds(0.180f), cancellationToken: cancellationToken);

                    _lightAttackSeriesIndex = 0;
                } else if (_lightAttackSeriesIndex == 2) {
                    await UniTask.Delay(TimeSpan.FromSeconds(0.360f), cancellationToken: cancellationToken);

                    PerformAttack(PlayerAttackSettings.AttackType.MainCombo2);
                
                    await UniTask.Delay(TimeSpan.FromSeconds(0.720f), cancellationToken: cancellationToken);
                    
                    _lightAttackSeriesIndex = 0;
                }
            }

            AttackInProgress = false;
            GameManager.Logger.Log("[Attack]: Finish attack");
        }
        
        private void PerformAttack(PlayerAttackSettings.AttackType attackType) {
            var hitBoxOffset = settings.hitBoxOffset;
            var hitBoxSize = settings.hitboxSize;
            switch (attackType) {
                case PlayerAttackSettings.AttackType.MainCombo1:
                    hitBoxOffset = settings.hitBoxCombo1Offset;
                    hitBoxSize = settings.hitboxCombo1Size;
                    break;
                case PlayerAttackSettings.AttackType.MainCombo2:
                    hitBoxOffset = settings.hitBoxCombo2Offset;
                    hitBoxSize = settings.hitboxCombo2Size;
                    break;
            }
            
            var hitBoxPos = (Vector2)transform.position + (Vector2)(transform.rotation * hitBoxOffset);
            var hits = Physics2D.OverlapBoxAll(hitBoxPos, hitBoxSize, settings.attackAngle,
                settings.enemyLayer);
            foreach (var hit in hits) {
                var isHit = false;
                var hitComponent = hit.GetComponent<IHittable>();
                if (hitComponent is not null) {
                    hitComponent.TakeHit(25);

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
                    return PlayerAttackSettings.AttackType.Main;
                case 1:
                    return PlayerAttackSettings.AttackType.MainCombo1;
                case 2:
                    return PlayerAttackSettings.AttackType.MainCombo2;
            }

            return PlayerAttackSettings.AttackType.Main;
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
                        Draw.Label2D(
                            float3.zero + new float3(0, settings.hitboxSize.y * 0.6f, 0), // Position above the box
                            "Attack Hitbox Main"
                        );
                    }
                }
            }
            
            using (Draw.WithColor(Color.indianRed)) {
                using (Draw.InLocalSpace(transform)) {
                    using (Draw.WithMatrix(Matrix4x4.TRS(
                               settings.hitBoxCombo1Offset,
                               Quaternion.Euler(0, 0, settings.attackAngle), Vector3.one))) {
                        Draw.WireBox(float3.zero, new float3(settings.hitboxCombo1Size.x, settings.hitboxCombo1Size.y, 0));
                        Draw.Label2D(
                            float3.zero + new float3(0, settings.hitboxCombo1Size.y * 0.6f, 0), // Position above the box
                            "Attack Hitbox Combo_1"
                        );
                    }
                }
            }
            
            using (Draw.WithColor(Color.softRed)) {
                using (Draw.InLocalSpace(transform)) {
                    using (Draw.WithMatrix(Matrix4x4.TRS(
                               settings.hitBoxCombo2Offset,
                               Quaternion.Euler(0, 0, settings.attackAngle), Vector3.one))) {
                        Draw.WireBox(float3.zero, new float3(settings.hitboxCombo2Size.x, settings.hitboxCombo2Size.y, 0));
                        Draw.Label2D(
                            float3.zero + new float3(0, settings.hitboxCombo2Size.y * 0.6f, 0), // Position above the box
                            "Attack Hitbox Combo_2"
                        );
                    }
                }
            }
        }
    }
}
