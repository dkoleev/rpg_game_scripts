using System;
using Darkness.Runtime.Messages;
using Darkness.Runtime.ScriptableObjects;
using MessagePipe;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using VContainer.Unity;
using PlayerInput = Darkness.Runtime.Input.PlayerInput;

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

        private ISubscriber<PerformInputMessage> _inputSubscriber;
        private IDisposable _disposable;
        
        private void Start() {
            SetupSubscribers();
        }

        private void SetupSubscribers() {
            _inputSubscriber = GlobalMessagePipe.GetSubscriber<PerformInputMessage>();
            var disposableBagBuilder = DisposableBag.CreateBuilder();
            _inputSubscriber.Subscribe(OnInput).AddTo(disposableBagBuilder);
            _disposable = disposableBagBuilder.Build();
        }

        private void OnInput(PerformInputMessage data) {
            switch (data.Type) {
                case PerformInputMessage.InputType.Attack:
                    if (data.Phase == PerformInputMessage.InputPhase.Performed) {
                        OnPerformAttack?.Invoke(data.IsSlowAttack ? AttackType.Slow : AttackType.Default);
                    }
                    break;
                case PerformInputMessage.InputType.Roll:
                    break;
                case PerformInputMessage.InputType.Block:
                    switch (data.Phase) {
                        case PerformInputMessage.InputPhase.Started:
                            break;
                        case PerformInputMessage.InputPhase.Performed:
                            IsBlocking = true;
                            OnBlocking?.Invoke(true);
                            break;
                        case PerformInputMessage.InputPhase.Cancelled:
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