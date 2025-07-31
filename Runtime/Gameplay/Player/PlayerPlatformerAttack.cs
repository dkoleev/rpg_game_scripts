using System;
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

        private ISubscriber<InputMessage> _inputSubscriber;
        private IDisposable _disposable;
        
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
                    if (data.Phase == InputMessage.InputPhase.Performed) {
                        OnPerformAttack?.Invoke(data.IsSlowAttack ? AttackType.Slow : AttackType.Default);
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

        public void OnDestroy() {
            _disposable?.Dispose();
        }
    }
}