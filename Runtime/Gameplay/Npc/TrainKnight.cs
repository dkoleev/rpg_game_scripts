using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using Darkness.Runtime.Gameplay.Effects;
using Darkness.Runtime.ScriptableObjects;
using UnityEngine;

namespace Darkness.Runtime.Gameplay.Npc {
    public class TrainKnight : MonoBehaviourExt, IHittable, IPhysicsObject {
        [Title("Hit Stop Settings")]
        [SerializeField] [Required] private HitStopSettings takeDamageHitStopSettings;
        [SerializeField] [Required] private HitStopSettings deadHitStopSettings;
        [Title("Flash Damage Settings")]
        [SerializeField] [Required] private FlashDamageSettings takeDamageFlashDamageSettings;
        [SerializeField] [Required] private FlashDamageSettings deadFlashDamageSettings;
        
        public Rigidbody2D Rigidbody2D => _rb;
        public Transform Transform => transform;
        
        private Animator _animator;
        private Rigidbody2D _rb;
        private FlashDamage _flashDamage;
        private HitStop _hitStop;

        private int _health = 100;

        private void CheckFields() {
            Debug.Assert(takeDamageHitStopSettings != null, "Take Damage Hit Stop Settings is not set");
            Debug.Assert(deadHitStopSettings != null, "Take Damage Hit Stop Settings is not set");
            Debug.Assert(takeDamageFlashDamageSettings != null, "Take Damage Hit Stop Settings is not set");
            Debug.Assert(deadFlashDamageSettings != null, "Take Damage Hit Stop Settings is not set");
        }

        private void OnValidate() {
            CheckFields();
        }

        private void Awake() {
            CheckFields();
            
            _animator = GetComponentInChildren<Animator>();
            _rb = GetComponent<Rigidbody2D>();
            _flashDamage = GetComponent<FlashDamage>();
            _hitStop = GetComponent<HitStop>();
        }
        
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                Debug.Log("Player hit");
            }
        }
        
        public void TakeHit(int damage) {
            _health -= damage;
            GameManager.Logger.Log($"{gameObject.name} take hit: damage: <color=green>{damage}</color>, health: <color=green>{_health}</color>");
            if (_health <= 0) {
                _health = 100;
                _animator.Play("Dead");
                if (_flashDamage != null) {
                    _flashDamage.Flash(deadFlashDamageSettings);
                }
    
                if (_hitStop != null) {
                    _hitStop.Stop(deadHitStopSettings).Forget();
                }
            }
            else {
                _animator.Play("Hit");
                if (_flashDamage != null) {
                    _flashDamage.Flash(takeDamageFlashDamageSettings);
                }
    
                if (_hitStop != null) {
                    _hitStop.Stop(takeDamageHitStopSettings).Forget();
                }    
            }
        }

        public void Stun(float duration) {
            
        }
    }
}