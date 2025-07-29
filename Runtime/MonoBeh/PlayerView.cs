using Darkness.Runtime.Gameplay.Player;
using UnityEngine;

namespace Darkness.Runtime.MonoBeh {
    public class PlayerView : MonoBehaviour {
        [SerializeField] private new Rigidbody2D rigidbody;
        [SerializeField] private Animator animator;

        //Set all of these up in the inspector
        [Header("Checks")] [field: SerializeField] public Transform GroundCheckPoint { get; private set; }
        //Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
        [field: SerializeField] public Vector2 GroundCheckSize { get; private set; }
        [field: SerializeField] public Transform FrontWallCheckPoint { get; private set; }
        [field: SerializeField] public Transform BackWallCheckPoint { get; private set; }
        [field: SerializeField] public Vector2 WallCheckSize { get; private set; }
        [Header("Layers & Tags")] [field: SerializeField] public LayerMask GroundLayer { get; private set; }
        
        public PlayerViewData Data { get; private set; }

        private void Awake() {
            Data = new PlayerViewData(rigidbody, animator);
        }
    }
}