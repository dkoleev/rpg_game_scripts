using System;
using System.Collections;
using Darkness.Runtime.ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Darkness.Runtime.Experimental {
	public class PlayerPlatformerMovement : MonoBehaviour {
		//Scriptable object which holds all the player's movement parameters. If you don't want to use it
		//just paste in all the parameters, though you will need to manuly change all references in this script
		public PlayerData Data;

		public Rigidbody2D RB { get; private set; }

		//Script to handle all player animations, all references can be safely removed if you're importing into your own project.
		public PlayerAnimator AnimHandler { get; private set; }


		//Variables control the various actions the player can perform at any time.
		//These are fields which can are public allowing for other sctipts to read them
		//but can only be privately written to.
		public bool IsFacingRight { get; private set; }
		public bool IsJumping { get; private set; }
		public bool IsWallJumping { get; private set; }
		public bool IsDashing { get; private set; }
		public bool IsSliding { get; private set; }
		public bool IsSitting { get; private set; }

		//Timers (also all fields, could be private and a method returning a bool could be used)
		public float LastOnGroundTime { get; private set; }
		public float LastOnWallTime { get; private set; }
		public float LastOnWallRightTime { get; private set; }
		public float LastOnWallLeftTime { get; private set; }

		//Jump
		private bool _isJumpCut;
		private bool _isJumpFalling;

		//Wall Jump
		private float _wallJumpStartTime;
		private int _lastWallJumpDir;

		//Dash
		private int _dashesLeft;
		private bool _dashRefilling;
		private Vector2 _lastDashDir;
		private bool _isDashAttacking;
		
		//Slide
		private Vector2 _lastSlideDir;
		private bool _isSlideAttacking;

		private Vector2 _moveInput;

		public float LastPressedJumpTime { get; private set; }
		public float LastPressedDashTime { get; private set; }
		public float LastPressedSlideTime { get; private set; }

		//Set all of these up in the inspector
		[Header("Checks")] [SerializeField] private Transform _groundCheckPoint;

		//Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
		[SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
		[Space(5)] [SerializeField] private Transform _frontWallCheckPoint;
		[SerializeField] private Transform _backWallCheckPoint;
		[SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);

		[Header("Layers & Tags")] [SerializeField]
		private LayerMask _groundLayer;

		private PlayerInput _playerInput;
		private InputAction _moveAction;
		private InputAction _jumpAction;
		private InputAction _dashAction;
		private InputAction _slideAction;
		
		private void Awake() {
			RB = GetComponent<Rigidbody2D>();
			AnimHandler = GetComponent<PlayerAnimator>();
			_playerInput = GetComponent<PlayerInput>();
			_moveAction = _playerInput.actions["Move"];
			_jumpAction = _playerInput.actions["Jump"];
			_dashAction = _playerInput.actions["Dash"];
			_slideAction = _playerInput.actions["Slide"];
		}

		private void Start() {
			SetGravityScale(Data.gravityScale);
			IsFacingRight = true;
		}

		private void Update() {
			LastOnGroundTime -= Time.deltaTime;
			LastOnWallTime -= Time.deltaTime;
			LastOnWallRightTime -= Time.deltaTime;
			LastOnWallLeftTime -= Time.deltaTime;

			LastPressedJumpTime -= Time.deltaTime;
			LastPressedDashTime -= Time.deltaTime;
			LastPressedSlideTime -= Time.deltaTime;

			_moveInput = _moveAction.ReadValue<Vector2>();
			if (_moveInput.x > 0.01f && !IsFacingRight) {
				IsFacingRight = true;
				transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			}else if (_moveInput.x < -0.01f && IsFacingRight) {
				IsFacingRight = false;
				transform.rotation = Quaternion.Euler(0f, 180f, 0f);
			}

			IsSitting = _moveInput.y < -0.9f && CanSit();
			
			if (_jumpAction.WasPressedThisFrame()) {
				OnJumpInput();
			}

			// if (_jumpAction.WasPressedThisFrame()) {
			// 	OnJumpUpInput();
			// }
			
			if (_dashAction.WasPressedThisFrame()) {
				OnDashInput();
			}
			
			if (_slideAction.WasPressedThisFrame()) {
				OnSlideInput();
			}

			if (!IsJumping) {
				//Ground Check
				if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0,
					    _groundLayer)) //checks if set box overlaps with ground
				{
					if (LastOnGroundTime < -0.1f) {
						AnimHandler.JustLanded = true;
					}

					LastOnGroundTime = Data.coyoteTime; //if so sets the lastGrounded to coyoteTime
				}
			}

			if (!IsDashing && !IsJumping && !IsSliding) {
				//Right Wall Check
				if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) &&
				      IsFacingRight)
				     || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) &&
				         !IsFacingRight)) && !IsWallJumping)
					LastOnWallRightTime = Data.coyoteTime;

				//Right Wall Check
				if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) &&
				      !IsFacingRight)
				     || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) &&
				         IsFacingRight)) && !IsWallJumping)
					LastOnWallLeftTime = Data.coyoteTime;

				//Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
				LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
			}

			if (IsJumping && RB.linearVelocity.y < 0) {
				IsJumping = false;

				_isJumpFalling = true;
			}

			if (IsWallJumping && Time.time - _wallJumpStartTime > Data.wallJumpTime) {
				IsWallJumping = false;
			}

			if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping) {
				_isJumpCut = false;

				_isJumpFalling = false;
			}

			if (!IsDashing && !IsSliding) {
				//Jump
				if (CanJump() && LastPressedJumpTime > 0) {
					IsJumping = true;
					IsWallJumping = false;
					_isJumpCut = false;
					_isJumpFalling = false;
					Jump();

					AnimHandler.StartedJumping = true;
				}
				//WALL JUMP
				else if (CanWallJump() && LastPressedJumpTime > 0) {
					IsWallJumping = true;
					
					IsJumping = false;
					_isJumpCut = false;
					_isJumpFalling = false;

					_wallJumpStartTime = Time.time;
					_lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

					WallJump(_lastWallJumpDir);
				}
			}

			#region DASH CHECKS

			if (CanDash() && LastPressedDashTime > 0) {
				//Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
				Sleep(Data.dashSleepTime);

				//If not direction pressed, dash forward
				if (_moveInput != Vector2.zero) {
					_lastDashDir = _moveInput;
				}
				else {
					_lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;
				}

				IsDashing = true;
				
				IsJumping = false;
				// IsSliding = false;
				IsWallJumping = false;
				_isJumpCut = false;

				AnimHandler.StartDashing = true;
				
				StartCoroutine(nameof(StartDash), _lastDashDir);
			}

			if (CanSlide() && LastPressedSlideTime > 0) {
					//If not direction pressed, slide forward
				if (_moveInput != Vector2.zero) {
					_lastSlideDir = _moveInput;
				}
				else {
					_lastSlideDir = IsFacingRight ? Vector2.right : Vector2.left;
				}

				IsSliding = true;
				
				IsDashing = false;
				IsJumping = false;
				IsWallJumping = false;
				_isJumpCut = false;

				AnimHandler.StartSliding = true;
				StartCoroutine(nameof(StartSlide), _lastSlideDir);
			}

			#endregion

			#region GRAVITY

			if (!_isDashAttacking && !_isSlideAttacking) {
				//Higher gravity if we've released the jump input or are falling
				if (RB.linearVelocity.y < 0 && _moveInput.y < 0) {
					//Much higher gravity if holding down
					SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
					//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
					RB.linearVelocity = new Vector2(RB.linearVelocity.x,
						Mathf.Max(RB.linearVelocity.y, -Data.maxFastFallSpeed));
				}
				else if (_isJumpCut) {
					//Higher gravity if jump button released
					SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
					RB.linearVelocity = new Vector2(RB.linearVelocity.x,
						Mathf.Max(RB.linearVelocity.y, -Data.maxFallSpeed));
				}
				else if ((IsJumping || IsWallJumping || _isJumpFalling) &&
				         Mathf.Abs(RB.linearVelocity.y) < Data.jumpHangTimeThreshold) {
					SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
				}
				else if (RB.linearVelocity.y < 0) {
					//Higher gravity if falling
					SetGravityScale(Data.gravityScale * Data.fallGravityMult);
					//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
					RB.linearVelocity = new Vector2(RB.linearVelocity.x,
						Mathf.Max(RB.linearVelocity.y, -Data.maxFallSpeed));
				}
				else {
					//Default gravity if standing on a platform or moving upwards
					SetGravityScale(Data.gravityScale);
				}
			}
			else {
				//No gravity when dashing or sliding (returns to normal once initial dashAttack phase over)
				SetGravityScale(0);
			}

			#endregion
		}

		private void FixedUpdate() {
			//Handle Run
			if (!IsDashing && !IsSliding) {
				if (IsWallJumping)
					Run(Data.wallJumpRunLerp);
				else
					Run(1);
			}
			else if (_isDashAttacking) {
				Run(Data.dashEndRunLerp);
			}
			else if (_isSlideAttacking) {
				Run(Data.slideEndRunLerp);
			}
		}

		//Methods which whandle input detected in Update()
		private void OnJumpInput() {
			LastPressedJumpTime = Data.jumpInputBufferTime;
		}

		private void OnJumpUpInput() {
			if (CanJumpCut() || CanWallJumpCut())
				_isJumpCut = true;
		}

		private void OnDashInput() {
			LastPressedDashTime = Data.dashInputBufferTime;
		}

		private void OnSlideInput() {
			LastPressedSlideTime = Data.slideInputBufferTime;
		}

		private void SetGravityScale(float scale) {
			RB.gravityScale = scale;
		}

		private void Sleep(float duration) {
			//Method used so we don't need to call StartCoroutine everywhere
			//nameof() notation means we don't need to input a string directly.
			//Removes chance of spelling mistakes and will improve error messages if any
			StartCoroutine(nameof(PerformSleep), duration);
		}

		private IEnumerator PerformSleep(float duration) {
			Time.timeScale = 0;
			yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
			Time.timeScale = 1;
		}


		//MOVEMENT METHODS

		private void Run(float lerpAmount) {
			//Calculate the direction we want to move in and our desired velocity
			float targetSpeed = _moveInput.x * Data.runMaxSpeed;
			//We can reduce are control using Lerp() this smooths changes to are direction and speed
			targetSpeed = Mathf.Lerp(RB.linearVelocityX, targetSpeed, lerpAmount);

			#region Calculate AccelRate

			float accelRate;

			//Gets an acceleration value based on if we are accelerating (includes turning) 
			//or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
			if (LastOnGroundTime > 0)
				accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
			else
				accelRate = (Mathf.Abs(targetSpeed) > 0.01f)
					? Data.runAccelAmount * Data.accelInAir
					: Data.runDeccelAmount * Data.deccelInAir;

			#endregion

			#region Add Bonus Jump Apex Acceleration

			//Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
			if ((IsJumping || IsWallJumping || _isJumpFalling) &&
			    Mathf.Abs(RB.linearVelocity.y) < Data.jumpHangTimeThreshold) {
				accelRate *= Data.jumpHangAccelerationMult;
				targetSpeed *= Data.jumpHangMaxSpeedMult;
			}

			#endregion

			#region Conserve Momentum

			//We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
			if (Data.doConserveMomentum && Mathf.Abs(RB.linearVelocityX) > Mathf.Abs(targetSpeed) &&
			    Mathf.Approximately(Mathf.Sign(RB.linearVelocityX), Mathf.Sign(targetSpeed)) && 
			    Mathf.Abs(targetSpeed) > 0.01f &&
			    LastOnGroundTime < 0) {
				//Prevent any deceleration from happening, or in other words conserve are current momentum
				//You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
				accelRate = 0;
			}

			#endregion

			//Calculate difference between current velocity and desired velocity
			var speedDif = targetSpeed - RB.linearVelocityX;

			//Calculate force along x-axis to apply to thr player
			float movement = speedDif * accelRate;
			
			//Convert this to a vector and apply to rigidbody
			RB.AddForce(movement * Vector2.right, ForceMode2D.Force);
			/*
			 * For those interested here is what AddForce() will do
			 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
			 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
			 */
		}

		private void Jump() {
			//Ensures we can't call Jump multiple times from one press
			LastPressedJumpTime = 0;
			LastOnGroundTime = 0;

			//We increase the force applied if we are falling
			//This means we'll always feel like we jump the same amount 
			//(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
			float force = Data.jumpForce;
			if (RB.linearVelocity.y < 0)
				force -= RB.linearVelocity.y;

			RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
		}

		private void WallJump(int dir) {
			//Ensures we can't call Wall Jump multiple times from one press
			LastPressedJumpTime = 0;
			LastOnGroundTime = 0;
			LastOnWallRightTime = 0;
			LastOnWallLeftTime = 0;

			#region Perform Wall Jump

			Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
			force.x *= dir; //apply force in opposite direction of wall

			if (!Mathf.Approximately(Mathf.Sign(RB.linearVelocity.x), Mathf.Sign(force.x)))
				force.x -= RB.linearVelocity.x;

			if (RB.linearVelocity.y <
			    0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
				force.y -= RB.linearVelocity.y;

			//Unlike in the run we want to use the Impulse mode.
			//The default mode will apply are force instantly ignoring masss
			RB.AddForce(force, ForceMode2D.Impulse);

			#endregion
		}

		//Dash Coroutine
		private IEnumerator StartDash(Vector2 dir) {
			//Overall this method of dashing aims to mimic Celeste, if you're looking for
			// a more physics-based approach try a method similar to that used in the jump

			//LastOnGroundTime = 0;
			LastPressedDashTime = 0;

			float startTime = Time.time;

			_dashesLeft--;
			_isDashAttacking = true;

			//SetGravityScale(0);

			//We keep the player's velocity at the dash speed during the "attack" phase (in celeste the first 0.15s)
			while (Time.time - startTime <= Data.dashAttackTime) {
				RB.linearVelocity = dir.normalized * Data.dashSpeed;
				//Pauses the loop until the next frame, creating something of a Update loop. 
				//This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
				yield return null;
			}

			startTime = Time.time;

			_isDashAttacking = false;

			//Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())
			//SetGravityScale(Data.gravityScale);
			RB.linearVelocity = Data.dashEndSpeed * dir.normalized;

			while (Time.time - startTime <= Data.dashEndTime) {
				yield return null;
			}

			//Dash over
			IsDashing = false;
		}

		//Short period before the player is able to dash again
		private IEnumerator RefillDash(int amount) {
			//SHoet cooldown, so we can't constantly dash along the ground, again this is the implementation in Celeste, feel free to change it up
			_dashRefilling = true;
			yield return new WaitForSeconds(Data.dashRefillTime);
			_dashRefilling = false;
			_dashesLeft = Mathf.Min(Data.dashAmount, _dashesLeft + 1);
		}

		private IEnumerator StartSlide(Vector2 dir) {
			LastPressedSlideTime = 0;
			float startTime = Time.time;
			_isSlideAttacking = true;
			//We keep the player's velocity at the dash speed during the "attack" phase (in celeste the first 0.15s)
			while (Time.time - startTime <= Data.slideTime) {
				RB.linearVelocity = dir.normalized * Data.slideSpeed;
				//Pauses the loop until the next frame, creating something of a Update loop. 
				//This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
				yield return null;
			}

			startTime = Time.time;
			_isSlideAttacking = false;

			//Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())
			//SetGravityScale(Data.gravityScale);
			RB.linearVelocity = Data.slideEndSpeed * dir.normalized;

			while (Time.time - startTime <= Data.slideEndTime) {
				yield return null;
			}

			//Slide over
			IsSliding = false;
		}

		private bool CanSit() {
			return  !IsJumping && !IsWallJumping && !IsDashing && !IsSliding && LastOnGroundTime > 0;
		}

		private bool CanJump() {
			return LastOnGroundTime > 0 && !IsJumping;
		}

		private bool CanWallJump() {
			return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
				(LastOnWallRightTime > 0 && _lastWallJumpDir == 1) ||
				(LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
		}

		private bool CanJumpCut() {
			return IsJumping && RB.linearVelocity.y > 0;
		}

		private bool CanWallJumpCut() {
			return IsWallJumping && RB.linearVelocity.y > 0;
		}

		private bool CanDash() {
			if (!IsDashing && !IsSliding && _dashesLeft < Data.dashAmount && LastOnGroundTime > 0 && !_dashRefilling) {
				StartCoroutine(nameof(RefillDash), 1);
			}

			return _dashesLeft > 0;
		}

		private bool CanSlide() {
			return true;
			return !IsSliding && !IsJumping && !IsWallJumping && !IsDashing && LastOnGroundTime > 0;
		}

		private void OnDrawGizmosSelected() {
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
			Gizmos.color = Color.blue;
			Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
			Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
		}
	}
}