using System;
using Cysharp.Threading.Tasks;
using Darkness.Runtime.Messages;
using Darkness.Runtime.ScriptableObjects;
using Drawing;
using MessagePipe;
using Unity.Mathematics;
using UnityEngine;

namespace Darkness.Runtime.Gameplay.Player {
    public class PlayerPlatformerAttack : MonoBehaviourGizmos {
        [SerializeField] private PlayerAttackSettings settings;

        public event Action<PlayerAttackSettings.AttackType> OnPerformAttack;
        public event Action<bool> OnBlocking;
        public bool IsBlocking { get; private set; }
        public bool AttackInProgress { get; private set; }

        private ISubscriber<InputMessage> _inputSubscriber;
        private IDisposable _disposable;
        private Rigidbody2D _rb;
        private PlayerPlatformerMovement _movement;

        private void Awake() {
            _rb = GetComponent<Rigidbody2D>();
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
                        return;
                    }

                    if (data.Phase == InputMessage.InputPhase.Performed) {
                        OnPerformAttack?.Invoke(data.IsSlowAttack
                            ? PlayerAttackSettings.AttackType.Slow
                            : PlayerAttackSettings.AttackType.Default);
                        AttackInProgress = true;
                        FinishAttack(data.IsSlowAttack).Forget();
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

        private async UniTaskVoid FinishAttack(bool isSlow) {
            if (isSlow) {
                await UniTask.Delay(TimeSpan.FromSeconds(0.360f));
                PerformAttack();
                await UniTask.Delay(TimeSpan.FromSeconds(0.720f));
                AttackInProgress = false;
            }
            else {
                await UniTask.Delay(TimeSpan.FromSeconds(0.300f));
                PerformAttack();
                await UniTask.Delay(TimeSpan.FromSeconds(0.420f));
                AttackInProgress = false;
            }
        }

        private void PerformAttack() {
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
