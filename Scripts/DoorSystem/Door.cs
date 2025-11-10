using System.Collections;
using UnityEngine;

using SPACE_UTIL;
using SPACE_DrawSystem;

namespace SPACE_GAME
{
	/// <summary>
	/// Main door component implementing IDoor interface.
	/// Attach to root door_hinged GameObject (the one with Animator).
	/// </summary>
	[RequireComponent(typeof(Animator))]
	public class Door : MonoBehaviour, IDoor
	{
		// ====================================================================
		// SERIALIZED FIELDS
		// ====================================================================

		[Header("Door Identity")]
		[SerializeField] string _doorId = "door 01"; // ground floor 1st room(base 10 + 1 style for room number)

		[Header("Lock Configuration")]
		[SerializeField] bool _usesCommonLock = false;

		[Header("Initial States")]
		[SerializeField] DoorState _initialDoorState = DoorState.closed;
		[SerializeField] DoorLockState _initialLockStateInside = DoorLockState.unlocked;
		[SerializeField] DoorLockState _initialLockStateOutside = DoorLockState.unlocked;
		[SerializeField] bool _initialBlocked = false;

		[Header("Animation Settings")]
		[Tooltip("If true, waits for animation events to confirm completion")]
		[SerializeField] bool useAnimationEvents = true;

		[Header("Debug")]
		[SerializeField] bool showDebugLogs = false;
		[SerializeField] bool drawDebugLines = false;

		// ====================================================================
		// PRIVATE FIELDS
		// ====================================================================

		private Animator animator;
		private DoorState _doorState;
		private DoorLockState _LockStateInside;
		private DoorLockState _LockStateOutside;
		private bool _blocked;

		// Animation state tracking
		private bool isAnimating = false;
		private Coroutine currentAnimation;

		// Player detection
		private DoorSide? playerSide = null; // null = no player nearby

		// ====================================================================
		// PUBLIC API - IDoor PROPERTIES
		// ====================================================================

		public bool blocked => _blocked;
		public bool usesCommonLock => _usesCommonLock;
		public DoorState doorState => _doorState;
		public DoorLockState doorLockStateInside => _LockStateInside;
		public DoorLockState doorLockStateOutside => _LockStateOutside;

		// ====================================================================
		// UNITY LIFECYCLE
		// ====================================================================

		void Awake()
		{
			if (showDebugLogs) Debug.Log(C.method(this, "white"));

			animator = GetComponent<Animator>();
			if (animator == null)
			{
				Debug.Log(C.method(this, "red", adMssg: "Animator component missing!"));
			}

			// Initialize states
			_doorState = _initialDoorState;
			_LockStateInside = _initialLockStateInside;
			_LockStateOutside = _initialLockStateOutside;
			_blocked = _initialBlocked;

			// Sync animator bools
			SyncAnimatorBools();
		}

		void Start()
		{
			if (showDebugLogs)
				Debug.Log(C.method(this, "white"));

			// Load saved state if exists
			// LoadState();
		}

		void Update()
		{
			if (drawDebugLines)
			{
				DrawDebugInfo();
			}
		}

		// ====================================================================
		// PUBLIC API - LOCK OPERATIONS
		// ====================================================================

		public bool TryLockInside()
		{
			if (isAnimating)
			{
				if (showDebugLogs) Debug.Log(C.method(this, "yellow", adMssg: "animation in progress"));
				return false;
			}

			if (_LockStateInside == DoorLockState.locked)
			{
				if (showDebugLogs) Debug.Log(C.method(this, "yellow", adMssg: "already locked inside"));
				return false;
			}

			if (_usesCommonLock)
			{
				currentAnimation = StartCoroutine(AnimateLockCommon(true));
			}
			else
			{
				currentAnimation = StartCoroutine(AnimateLockInside(true));
			}

			return true;
		}

		public bool TryLockOutside()
		{
			if (isAnimating)
			{
				if (showDebugLogs) Debug.Log(C.method(this, "yellow", adMssg: "animation in progress"));
				return false;
			}

			if (_LockStateOutside == DoorLockState.locked)
			{
				if (showDebugLogs) Debug.Log(C.method(this, "yellow", adMssg: "already locked outside"));
				return false;
			}

			if (_usesCommonLock)
			{
				currentAnimation = StartCoroutine(AnimateLockCommon(true));
			}
			else
			{
				currentAnimation = StartCoroutine(AnimateLockOutside(true));
			}

			return true;
		}

		public bool TryUnlockInside()
		{
			if (isAnimating)
			{
				if (showDebugLogs) Debug.Log(C.method(this, "yellow", adMssg: "animation in progress"));
				return false;
			}

			if (_LockStateInside == DoorLockState.unlocked)
			{
				if (showDebugLogs) Debug.Log(C.method(this, "yellow", adMssg: "already unlocked inside"));
				return false;
			}

			if (_usesCommonLock)
			{
				currentAnimation = StartCoroutine(AnimateLockCommon(false));
			}
			else
			{
				currentAnimation = StartCoroutine(AnimateLockInside(false));
			}

			return true;
		}

		public bool TryUnlockOutside()
		{
			if (isAnimating)
			{
				if (showDebugLogs) Debug.Log(C.method(this, "yellow", adMssg: "animation in progress"));
				return false;
			}

			if (_LockStateOutside == DoorLockState.unlocked)
			{
				if (showDebugLogs) Debug.Log(C.method(this, "yellow", adMssg: "already unlocked outside"));
				return false;
			}

			if (_usesCommonLock)
			{
				currentAnimation = StartCoroutine(AnimateLockCommon(false));
			}
			else
			{
				currentAnimation = StartCoroutine(AnimateLockOutside(false));
			}

			return true;
		}

		// ====================================================================
		// PUBLIC API - DOOR OPERATIONS
		// ====================================================================

		public bool TryOpen(DoorSide fromSide)
		{
			if (_blocked)
			{
				if (showDebugLogs) Debug.Log(C.method(this, "yellow", adMssg: "door is blocked!"));
				PlayBlockedJiggle();
				return false;
			}

			if (isAnimating)
			{
				if (showDebugLogs) Debug.Log(C.method(this, "yellow", adMssg: "animation in progress"));
				return false;
			}

			if (_doorState == DoorState.opened || _doorState == DoorState.opening)
			{
				if (showDebugLogs) Debug.Log(C.method(this, "yellow", adMssg: "door already opened/opening"));
				return false;
			}

			// Check locks
			bool insideLocked = _LockStateInside == DoorLockState.locked;
			bool outsideLocked = _LockStateOutside == DoorLockState.locked;

			if (insideLocked || outsideLocked)
			{
				if (showDebugLogs) Debug.Log(C.method(this, "yellow", adMssg: "door is locked!"));
				PlayLockedJiggle();
				return false;
			}

			currentAnimation = StartCoroutine(AnimateOpen());
			return true;
		}

		public bool TryClose()
		{
			if (_blocked)
			{
				if (showDebugLogs) Debug.Log(C.method(this, "yellow", adMssg: "door is blocked!"));
				PlayBlockedJiggle();
				return false;
			}

			if (isAnimating)
			{
				if (showDebugLogs) Debug.Log(C.method(this, "yellow", adMssg: "animation in progress"));
				return false;
			}

			if (_doorState == DoorState.closed || _doorState == DoorState.closing)
			{
				if (showDebugLogs) Debug.Log(C.method(this, "yellow", adMssg: "door already closed/closing"));
				return false;
			}

			// Auto-unlock both locks when closing
			_LockStateInside = DoorLockState.unlocked;
			_LockStateOutside = DoorLockState.unlocked;

			currentAnimation = StartCoroutine(AnimateClose());
			return true;
		}

		// ====================================================================
		// PUBLIC API - SUPERNATURAL OPERATIONS
		// ====================================================================

		public bool TryBlock()
		{
			if (_blocked)
			{
				if (showDebugLogs) Debug.Log(C.method(this, "yellow", adMssg: "already blocked"));
				return false;
			}

			_blocked = true;
			DoorEvents.OnDoorBlocked?.Invoke(_doorId);

			if (showDebugLogs) Debug.Log(C.method(this, "cyan", adMssg: "door blocked"));
			SaveState();
			return true;
		}

		public bool TryUnblock()
		{
			if (!_blocked)
			{
				if (showDebugLogs) Debug.Log(C.method(this, "yellow", adMssg: "already unblocked"));
				return false;
			}

			_blocked = false;

			if (showDebugLogs) Debug.Log(C.method(this, "cyan", adMssg: "door unblocked"));
			SaveState();
			return true;
		}

		public bool TryDoorSwaying()
		{
			if (_doorState == DoorState.swaying)
			{
				if (showDebugLogs) Debug.Log(C.method(this, "yellow", adMssg: "already swaying"));
				return false;
			}

			// Stop any current animation
			if (currentAnimation != null)
			{
				StopCoroutine(currentAnimation);
				currentAnimation = null;
			}

			// Unlock both locks with animation
			_LockStateInside = DoorLockState.unlocked;
			_LockStateOutside = DoorLockState.unlocked;

			currentAnimation = StartCoroutine(AnimateSwaying());
			return true;
		}

		public bool TryDoorStopSwaying(DoorState doorStateAfterStop = DoorState.opened)
		{
			if (_doorState != DoorState.swaying)
			{
				if (showDebugLogs) Debug.Log(C.method(this, "yellow", adMssg: "not swaying"));
				return false;
			}

			if (currentAnimation != null)
			{
				StopCoroutine(currentAnimation);
				currentAnimation = null;
			}

			isAnimating = false;
			_doorState = doorStateAfterStop;

			// Trigger appropriate animation
			if (doorStateAfterStop == DoorState.opened)
			{
				animator.trySetTrigger(AnimParamType.doorOpen);
			}
			else if (doorStateAfterStop == DoorState.closed)
			{
				animator.trySetTrigger(AnimParamType.doorClose);
			}

			SyncAnimatorBools();
			DoorEvents.OnDoorSwayingStop?.Invoke(_doorId);
			SaveState();

			if (showDebugLogs) Debug.Log(C.method(this, "cyan", adMssg: $"stopped swaying -> {doorStateAfterStop}"));
			return true;
		}

		// ====================================================================
		// PUBLIC API - PLAYER DETECTION
		// ====================================================================

		/// <summary>
		/// Called by DoorInteractionZone when player enters/exits
		/// </summary>
		public void SetPlayerSide(DoorSide? side)
		{
			playerSide = side;
			if (showDebugLogs && side.HasValue)
			{
				Debug.Log(C.method(this, "cyan", adMssg: $"player entered {side.Value} zone"));
			}
		}

		// ====================================================================
		// PRIVATE API - ANIMATION COROUTINES
		// ====================================================================

		private IEnumerator AnimateLockInside(bool toLock)
		{
			isAnimating = true;

			AnimParamType trigger = toLock ? AnimParamType.lockInside : AnimParamType.unlockInside;
			animator.trySetTrigger(trigger);

			// Wait for animation (.NET 2.0: cannot yield inside try-catch)
			if (useAnimationEvents)
			{
				// Animation will call OnAnimationComplete()
				while (isAnimating)
					yield return null;
			}
			else
			{
				// Fallback: wait for state info
				yield return new WaitForSeconds(0.1f); // small delay for state transition
				AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(1); // Layer 1: Inside Lock
				while (stateInfo.normalizedTime < 1f)
				{
					yield return null;
					stateInfo = animator.GetCurrentAnimatorStateInfo(1);
				}
				isAnimating = false;
			}

			_LockStateInside = toLock ? DoorLockState.locked : DoorLockState.unlocked;
			SyncAnimatorBools();
			DoorEvents.OnDoorLockStateChanged?.Invoke(_doorId, _LockStateInside, DoorSide.inside);
			SaveState();

			if (showDebugLogs) Debug.Log(C.method(this, "lime", adMssg: $"inside lock -> {_LockStateInside}"));
			currentAnimation = null;
		}

		private IEnumerator AnimateLockOutside(bool toLock)
		{
			isAnimating = true;

			AnimParamType trigger = toLock ? AnimParamType.lockOutside : AnimParamType.unlockOutside;
			animator.trySetTrigger(trigger);

			if (useAnimationEvents)
			{
				while (isAnimating)
					yield return null;
			}
			else
			{
				yield return new WaitForSeconds(0.1f);
				AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(2); // Layer 2: Outside Lock
				while (stateInfo.normalizedTime < 1f)
				{
					yield return null;
					stateInfo = animator.GetCurrentAnimatorStateInfo(2);
				}
				isAnimating = false;
			}

			_LockStateOutside = toLock ? DoorLockState.locked : DoorLockState.unlocked;
			SyncAnimatorBools();
			DoorEvents.OnDoorLockStateChanged?.Invoke(_doorId, _LockStateOutside, DoorSide.outside);
			SaveState();

			if (showDebugLogs) Debug.Log(C.method(this, "lime", adMssg: $"outside lock -> {_LockStateOutside}"));
			currentAnimation = null;
		}

		private IEnumerator AnimateLockCommon(bool toLock)
		{
			isAnimating = true;

			AnimParamType trigger = toLock ? AnimParamType.lockCommon : AnimParamType.unlockCommon;
			animator.trySetTrigger(trigger);

			if (useAnimationEvents)
			{
				while (isAnimating)
					yield return null;
			}
			else
			{
				yield return new WaitForSeconds(0.1f);
				AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(3); // Layer 3: Common Lock
				while (stateInfo.normalizedTime < 1f)
				{
					yield return null;
					stateInfo = animator.GetCurrentAnimatorStateInfo(3);
				}
				isAnimating = false;
			}

			// Both locks share same state when using common lock
			_LockStateInside = toLock ? DoorLockState.locked : DoorLockState.unlocked;
			_LockStateOutside = toLock ? DoorLockState.locked : DoorLockState.unlocked;
			SyncAnimatorBools();
			DoorEvents.OnDoorLockStateChanged?.Invoke(_doorId, _LockStateInside, DoorSide.inside);
			DoorEvents.OnDoorLockStateChanged?.Invoke(_doorId, _LockStateOutside, DoorSide.outside);
			SaveState();

			if (showDebugLogs) Debug.Log(C.method(this, "lime", adMssg: $"common lock -> {_LockStateInside}"));
			currentAnimation = null;
		}

		private IEnumerator AnimateOpen()
		{
			isAnimating = true;
			_doorState = DoorState.opening;

			animator.trySetTrigger(AnimParamType.doorOpen);
			SyncAnimatorBools();

			if (useAnimationEvents)
			{
				while (isAnimating)
					yield return null;
			}
			else
			{
				yield return new WaitForSeconds(0.1f);
				AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // Layer 0: Door Movement
				while (stateInfo.normalizedTime < 1f)
				{
					yield return null;
					stateInfo = animator.GetCurrentAnimatorStateInfo(0);
				}
				isAnimating = false;
			}

			_doorState = DoorState.opened;
			SyncAnimatorBools();
			DoorEvents.OnDoorStateChanged?.Invoke(_doorId, _doorState);
			SaveState();

			if (showDebugLogs) Debug.Log(C.method(this, "lime", adMssg: "door opened"));
			currentAnimation = null;
		}

		private IEnumerator AnimateClose()
		{
			isAnimating = true;
			_doorState = DoorState.closing;

			animator.trySetTrigger(AnimParamType.doorClose);
			SyncAnimatorBools();

			if (useAnimationEvents)
			{
				while (isAnimating)
					yield return null;
			}
			else
			{
				yield return new WaitForSeconds(0.1f);
				AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
				while (stateInfo.normalizedTime < 1f)
				{
					yield return null;
					stateInfo = animator.GetCurrentAnimatorStateInfo(0);
				}
				isAnimating = false;
			}

			_doorState = DoorState.closed;
			SyncAnimatorBools();
			DoorEvents.OnDoorStateChanged?.Invoke(_doorId, _doorState);
			SaveState();

			if (showDebugLogs) Debug.Log(C.method(this, "lime", adMssg: "door closed"));
			currentAnimation = null;
		}

		private IEnumerator AnimateSwaying()
		{
			isAnimating = true;
			_doorState = DoorState.swaying;

			// Note: doorSwayingLoopAnim should be set up in animator
			// This is a placeholder - implement based on your animation setup

			DoorEvents.OnDoorSwayingStart?.Invoke(_doorId);
			SyncAnimatorBools();
			SaveState();

			if (showDebugLogs) Debug.Log(C.method(this, "cyan", adMssg: "door swaying started"));

			// Swaying continues until TryDoorStopSwaying() is called
			while (_doorState == DoorState.swaying)
			{
				yield return null;
			}

			isAnimating = false;
			currentAnimation = null;
		}

		// ====================================================================
		// PRIVATE API - FEEDBACK
		// ====================================================================

		private void PlayBlockedJiggle()
		{
			animator.trySetTrigger(AnimParamType.doorBlockedJiggle);
			DoorEvents.OnDoorBlocked?.Invoke(_doorId);
		}

		private void PlayLockedJiggle()
		{
			animator.trySetTrigger(AnimParamType.doorLockedJiggle);
			DoorEvents.OnDoorLockedJiggle?.Invoke(_doorId);
		}

		// ====================================================================
		// PRIVATE API - STATE MANAGEMENT
		// ====================================================================

		private void SyncAnimatorBools()
		{
			animator.trySetBool(AnimParamType.isInsideLocked, _LockStateInside == DoorLockState.locked);
			animator.trySetBool(AnimParamType.isOutsideLocked, _LockStateOutside == DoorLockState.locked);
			animator.trySetBool(AnimParamType.isDoorOpen, _doorState == DoorState.opened);
		}

		private void SaveState()
		{
			DoorData data = new DoorData()
			{
				doorId = _doorId,
				doorState = _doorState,
				lockStateInside = _LockStateInside,
				lockStateOutside = _LockStateOutside,
				blocked = _blocked,
			};
			// Note: This saves individual door. For multiple doors, use DoorRegistry
			// data.Save();
		}

		/*
		private void LoadState()
		{
			DoorData data = LOG.LoadGameData<DoorData>(GameDataType.doorRegeistryData);

			if (data != null && data.doorId == _doorId)
			{
				_doorState = data.doorState;
				_LockStateInside = data.lockStateInside;
				_LockStateOutside = data.lockStateOutside;
				_blocked = data.blocked;

				SyncAnimatorBools();

				if (showDebugLogs) Debug.Log(C.method(this, "lime", adMssg: "loaded saved state"));
			}
		}
		*/

		// ====================================================================
		// EVENT METHODS (Called by Animation Events)
		// ====================================================================

		/// <summary>
		/// Called by animation event when lock/unlock/open/close animation completes
		/// Add this as Animation Event at end keyframe of animations
		/// </summary>
		public void OnAnimationComplete()
		{
			isAnimating = false;
			if (showDebugLogs) Debug.Log(C.method(this, "lime", adMssg: "animation complete"));
		}

		// ====================================================================
		// DEBUG
		// ====================================================================

		private void DrawDebugInfo()
		{
			Vector3 pos = transform.position;
			Vector3 up = Vector3.up * 2f;

			// Door state
			string stateColor = _doorState == DoorState.opened ? "lime" :
							   _doorState == DoorState.closed ? "red" : "yellow";

			Line.create($"{_doorId}_state")
				.setA(pos)
				.setN(up)
				.setCol(Color.white);

			// Block indicator
			if (_blocked)
			{
				Line.create($"{_doorId}_blocked")
					.setA(pos + Vector3.right * 0.5f)
					.setN(up)
					.setCol(Color.red)
					.setE(0.05f);
			}
			else
			{
				Line.create($"{_doorId}_blocked").toggle(false);
			}
		}
	}
}