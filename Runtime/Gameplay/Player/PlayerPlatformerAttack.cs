using System;
using Darkness.Runtime.Messages;
using MessagePipe;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using VContainer.Unity;
using PlayerInput = Darkness.Runtime.Input.PlayerInput;

namespace Darkness.Runtime.Gameplay.Player {
    public class PlayerPlatformerAttack : IStartable, IDisposable {
        public enum AttackType {
            Default,
            Slow,
            Sit
        }
        
        public event Action<AttackType> OnPerformAttack;
        public event Action<bool> OnBlocking;
        public bool IsBlocking { get; private set; }
        
        private readonly ISubscriber<PerformInputMessage> _attackInputSubscriber;
        private IDisposable _disposable;
        
        public PlayerPlatformerAttack(ISubscriber<PerformInputMessage> attackInputSubscriber) {
            _attackInputSubscriber = attackInputSubscriber;
        }

        public void Start() {
            SetupSubscribers();
        }

        private void SetupSubscribers() {
            var disposableBagBuilder = DisposableBag.CreateBuilder();
            _attackInputSubscriber.Subscribe(OnInput).AddTo(disposableBagBuilder);
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

        public void Dispose() {
            _disposable?.Dispose();
        }

        private void AttackStarted(InputAction.CallbackContext context) {
            
        }

        private void AttackPerformed(InputAction.CallbackContext context) {
            if (context.interaction is SlowTapInteraction) {
                OnPerformAttack?.Invoke(AttackType.Slow);
            }
            else {
                OnPerformAttack?.Invoke(AttackType.Default);
            }
        }
        
        private void AttackCancelled(InputAction.CallbackContext context) {
            
        }
        
        
    }
}