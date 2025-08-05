using System;
using Cysharp.Threading.Tasks;
using Darkness.Runtime.Messages;
using MessagePipe;
using UnityEngine;

namespace Darkness.Runtime.Gameplay.Player {
    public class PlayerPlatformerAttack : MonoBehaviour {
        public enum AttackType {
            Default,
            Slow,
            Sit
        }

        public event Action<AttackType> OnPerformAttack;
        public event Action<bool> OnBlocking;
        public bool IsBlocking { get; private set; }
        public bool AttackInProgress { get; private set; }

        private ISubscriber<InputMessage> _inputSubscriber;
        private IDisposable _disposable;
        private PlayerAttack _playerAttack;

        private void Awake() {
            _playerAttack = GetComponentInChildren<PlayerAttack>();
            _playerAttack.SetActive(false);
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
                        OnPerformAttack?.Invoke(data.IsSlowAttack ? AttackType.Slow : AttackType.Default);
                        AttackInProgress = true;
                        FinishAttack().Forget();
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

        private async UniTaskVoid FinishAttack() {
            await UniTask.Delay(TimeSpan.FromSeconds(0.300f));
            _playerAttack.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(0.180f));
            _playerAttack.SetActive(false);
            await UniTask.Delay(TimeSpan.FromSeconds(0.240f));
            AttackInProgress = false;
        }

        public void OnDestroy() {
            _disposable?.Dispose();
        }
    }
}
