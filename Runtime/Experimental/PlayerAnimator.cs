using UnityEngine;

namespace Darkness.Runtime.Experimental {
    public class PlayerAnimator : MonoBehaviour {
        private PlayerPlatformerMovement mov;
        private Animator anim;
        private SpriteRenderer spriteRend;

        [Header("Movement Tilt")] [SerializeField]
        private float maxTilt;

        [SerializeField] [Range(0, 1)] private float tiltSpeed;

        [Header("Particle FX")] [SerializeField]
        private GameObject jumpFX;

        [SerializeField] private GameObject landFX;

        public bool startedJumping { private get; set; }
        public bool justLanded { private get; set; }

        public float currentVelY;

        private void Start() {
            mov = GetComponent<PlayerPlatformerMovement>();
            spriteRend = GetComponentInChildren<SpriteRenderer>();
            anim = spriteRend.GetComponent<Animator>();
        }

        private void LateUpdate() {
            #region Tilt

            float tiltProgress;

            int mult = -1;

            if (mov.IsSliding) {
                tiltProgress = 0.25f;
            }
            else {
                tiltProgress = Mathf.InverseLerp(-mov.Data.runMaxSpeed, mov.Data.runMaxSpeed, mov.RB.linearVelocity.x);
                mult = (mov.IsFacingRight) ? 1 : -1;
            }

            float newRot = ((tiltProgress * maxTilt * 2) - maxTilt);
            float rot = Mathf.LerpAngle(spriteRend.transform.localRotation.eulerAngles.z * mult, newRot, tiltSpeed);
            spriteRend.transform.localRotation = Quaternion.Euler(0, 0, rot * mult);

            #endregion

            CheckAnimationState();
        }

        private void CheckAnimationState() {
            if (startedJumping) {
                anim.SetTrigger("Jump");
                startedJumping = false;
                return;
            }

            if (justLanded) {
                anim.SetTrigger("Land");
                justLanded = false;
                return;
            }

            anim.SetFloat("Vel Y", mov.RB.linearVelocity.y);
        }
    }
}