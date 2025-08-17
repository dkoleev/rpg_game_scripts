using System;
using Darkness.Runtime.Gameplay.Npc.EnemyStates;
using UnityEngine;

namespace Darkness.Runtime.Gameplay.Npc {
    public class EnemyAnimation : MonoBehaviour {
        private Animator _animator;
        private EnemyAI _enemyAI;

        private void Awake() {
            _animator = GetComponentInChildren<Animator>();
            _enemyAI = GetComponent<EnemyAI>();
        }

        private void OnEnable() {
            _enemyAI.OnStateChanged += EnemyAIOnStateChanged;
        }

        private void OnDisable() {
            _enemyAI.OnStateChanged -= EnemyAIOnStateChanged;
        }

        private void EnemyAIOnStateChanged(IEnemyState oldState, IEnemyState newState) {
            switch (newState) {
                case EnemyIdleState:
                    Idle();
                    break;
                case EnemyPatrolState:
                    Walk();
                    break;
                case EnemyAttackState:
                    Attack();
                    break;
                case EnemyChaseState:
                    Walk();
                    break;
                case EnemyDeadState:
                    Death();
                    break;
            }
        }

        private void Walk() {
            _animator.Play("Walk");            
        }

        private void Idle() {
            _animator.Play("Idle");       
        }

        private void Attack() {
            _animator.Play("Attack");
        }

        private void Death() {
            _animator.Play("Death");
        }
    }
}