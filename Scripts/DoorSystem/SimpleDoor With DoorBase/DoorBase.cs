namespace SPACE_GAME_1
{
	using UnityEngine;
	using System;
	using SPACE_UTIL;

	// ============================================================================
	// ENUMS - Global door system enums
	// ============================================================================

	public enum DoorState
	{
		Closed,
		Opening,
		Opened,
		Closing,
		Swaying // Supernatural entity control
	}

	public enum DoorLockState
	{
		Locked,
		Locking,
		Unlocked,
		Unlocking,
		UnlockedJam, // Cannot be locked by player - always unlocked
	}

	public enum LockSide
	{
		Inside,
		Outside,
		Any // For common locks (keypad, gate handle)
	}

	public enum DoorActionResult
	{
		Success,
		Failure,
		Blocked,
		Locked,
		UnlockedJam,
		AlreadyInState,
		AnimationInProgress,
		WrongKeyToUnlock,
		ObstructionDetected
	}

	public enum DoorAnimParamType
	{
		// triggers
		doorOpen,
		doorClose,
		doorSwayStart,
		doorSwayStop,
		doorLockedJiggle,
		doorBlockedJiggle,
		lockInside,
		lockOutside,
		lockCommon,
		unlockInside,
		unlockOutside,
		unlockCommon,
		// booleans
		isDoorOpen,
		isInsideLocked,
		isOutsideLocked,
		isDoorSwaying,
	}

	public enum AnimationEventType
	{
		DoorOpeningStarted,
		DoorOpeningComplete,
		DoorClosingStarted,
		DoorClosingComplete,
		InsideLockingStarted,
		InsideLockingComplete,
		InsideUnlockingStarted,
		InsideUnlockingComplete,
		OutsideLockingStarted,
		OutsideLockingComplete,
		OutsideUnlockingStarted,
		OutsideUnlockingComplete,
		CommonLockingStarted,
		CommonLockingComplete,
		CommonUnlockingStarted,
		CommonUnlockingComplete,
		DoorSwayStarted,
		DoorSwayStopped
	}

	// ============================================================================
	// BASE CLASS - All door types inherit from this
	// ============================================================================

	public abstract class DoorBase : MonoBehaviour
	{
		[Header("DEBUG INFO")]
		[SerializeField, TextArea(15, 20)] string _debugInfo;

		[Header("CONFIGURATION")]
		[SerializeField] protected Animator _animator;
		[SerializeField] protected string _keyId = "";
		[SerializeField] protected bool _requiresKeycard = false;
		[SerializeField] protected bool _usesCommonLock = false;
		// [SerializeField] protected bool _canBeLocked = true;
		[SerializeField] protected int _maxClosingRetries = 5;
		[SerializeField] protected float _autoLockDelay = 0f; // 0 = disabled

		[Header("AUDIO CLIPS")]
		[SerializeField] protected AudioClip _doorOpenSound;       // doorSwayOpenClose (opening portion)
		[SerializeField] protected AudioClip _doorCloseSound;      // doorSlam or doorSwayOpenClose (closing portion)
		[SerializeField] protected AudioClip _doorLockSound;       // doorLock
		[SerializeField] protected AudioClip _doorUnlockSound;     // doorUnlock
		[SerializeField] protected AudioClip _doorLockedJiggleSound; // doorLockedJiggle
		[SerializeField] protected AudioClip _doorKnockSound;      // doorKnocks (optional)
		[SerializeField] protected AudioClip _doorSwayLoopSound;   // doorSwayOpenClose (looping)

		[Header("AUDIO SOURCE")]
		[SerializeField] protected AudioSource _audioSource;

		[Header("INITIAL STATE")]
		[SerializeField] protected bool _initIsBlocked = false;
		[SerializeField] protected DoorState _initDoorState = DoorState.Closed;
		[SerializeField] protected DoorLockState _initInsideLockState = DoorLockState.Unlocked;
		[SerializeField] protected DoorLockState _initOutsideLockState = DoorLockState.Unlocked;

		// Public properties
		public string keyId { get => _keyId; set => _keyId = value; }
		public bool requiresKeycard => _requiresKeycard;
		public bool usesCommonLock => _usesCommonLock;
		// public bool canBeLocked => _canBeLocked; // its the same when unlockedJammed both the locks

		// TODO >>
		public int maxClosingRetries => _maxClosingRetries;
		public float autoLockDelay => _autoLockDelay;
		// << TODO

		// Runtime state
		public bool isBlocked { get; protected set; }
		public DoorState currDoorState { get; protected set; }
		public DoorLockState currInsideLockState { get; protected set; }
		public DoorLockState currOutsideLockState { get; protected set; }

		public bool initInsideUnlockedJammed { get; protected set; }
		public bool initOutsideUnlockedJammed { get; protected set; }

		// Auto-lock coroutine tracking
		protected Coroutine _autoLockCoroutine;

		// Events
		public event Action<DoorState> OnDoorStateChanged;
		public event Action<DoorLockState, LockSide> OnLockStateChanged;

		// ========================================================================
		// INITIALIZATION
		// ========================================================================
		protected virtual void Awake()
		{
			Debug.Log(C.method(this, "white"));
			// make sure all necessary serialize field are set and linked
			if (_animator == null)
			{
				Debug.Log(C.method(this, "red", adMssg: "animator not set"));
				return;
			}
			InitializeDoor();
			SetupAudio();
		}
		protected virtual void InitializeDoor()
		{
			// Copy inspector values to runtime state
			// Sync animator to initial state WITHOUT playing animations at the very start >>
			isBlocked = _initIsBlocked;
			currDoorState = _initDoorState;
			currInsideLockState = _initInsideLockState;
			currOutsideLockState = _initOutsideLockState;
			initInsideUnlockedJammed = (_initInsideLockState == DoorLockState.UnlockedJam);
			initOutsideUnlockedJammed = (_initOutsideLockState == DoorLockState.UnlockedJam);

			_animator.trySetBool(DoorAnimParamType.isDoorOpen,
				_initDoorState == DoorState.Opened);
			_animator.trySetBool(DoorAnimParamType.isInsideLocked,
				_initInsideLockState == DoorLockState.Locked);
			_animator.trySetBool(DoorAnimParamType.isOutsideLocked,
				_initOutsideLockState == DoorLockState.Locked);

			// Force immediate evaluation
			_animator.Update(0f); 
			// << Sync animator to initial state WITHOUT playing animations at the very start
		}
		protected virtual void SetupAudio()
		{
			// Create AudioSource if not assigned
			if (_audioSource == null)
			{
				_audioSource = this.GetComponent<AudioSource>();
				if (_audioSource == null)
				{
					Debug.Log(C.method(this, "red", adMssg: "audio source not found"));
					return;
				}

				_audioSource = gameObject.AddComponent<AudioSource>();
				_audioSource.playOnAwake = false;
				_audioSource.spatialBlend = 1f; // 3D sound
			}
		}
		protected virtual void Update()
		{
			_debugInfo = GetDebugInfo();
		}

		// ========================================================================
		// PUBLIC API - Core door operations
		// ========================================================================
		public virtual DoorActionResult TryOpen()
		{
			Debug.Log(C.method(this, "cyan"));
			// Check block
			if (isBlocked)
			{
				_animator.trySetTrigger(DoorAnimParamType.doorBlockedJiggle);
				PlayAudio(_doorLockedJiggleSound); // Blocked uses same jiggle sound
				return DoorActionResult.Blocked;
			}
			// Check current state
			if (currDoorState == DoorState.Opened)
				return DoorActionResult.AlreadyInState;
			if (currDoorState == DoorState.Opening ||
				currDoorState == DoorState.Closing ||
				currDoorState == DoorState.Swaying)
				return DoorActionResult.AnimationInProgress;

			// Check locks
			if (currInsideLockState == DoorLockState.Locked ||
				currOutsideLockState == DoorLockState.Locked)
			{
				_animator.trySetTrigger(DoorAnimParamType.doorLockedJiggle);
				PlayAudio(_doorLockedJiggleSound);
				return DoorActionResult.Locked;
			}

			// Execute
			currDoorState = DoorState.Opening;
			_animator.trySetTrigger(DoorAnimParamType.doorOpen);
			PlayAudio(_doorOpenSound); // Play open sound
			OnDoorStateChanged?
				.Invoke(currDoorState);
			return DoorActionResult.Success;
		}
		public virtual DoorActionResult TryClose()
		{
			Debug.Log(C.method(this, "cyan"));

			if (isBlocked)
				return DoorActionResult.Blocked;
			if (currDoorState == DoorState.Closed)
				return DoorActionResult.AlreadyInState;
			if (currDoorState == DoorState.Opening ||
				currDoorState == DoorState.Closing ||
				currDoorState == DoorState.Swaying)
				return DoorActionResult.AnimationInProgress;
			// Auto-unlock both sides when closing
			if (usesCommonLock)
				TryUnlock(LockSide.Any);
			else
			{
				TryUnlock(LockSide.Inside);
				TryUnlock(LockSide.Outside);
			}
			currDoorState = DoorState.Closing;
			_animator.trySetTrigger(DoorAnimParamType.doorClose);
			PlayAudio(_doorCloseSound); // Play close sound
			OnDoorStateChanged?.Invoke(currDoorState);
			return DoorActionResult.Success;
		}
		public virtual DoorActionResult ForceClose(bool blockDoorAfterClosed = false) // done by super natural entity may block or maynot block the door after closing
		{
			Debug.Log(C.method(this, "orange"));

			// Slam shut - bypass normal checks, just force it closed
			currDoorState = DoorState.Closing;
			_animator.trySetTrigger(DoorAnimParamType.doorClose);
			PlayAudio(_doorCloseSound); // TODO: Use slam sound for force close
			OnDoorStateChanged ?
				.Invoke(currDoorState); // if there are subscribers

			this.isBlocked = blockDoorAfterClosed;
			return DoorActionResult.Success;
		} 
		public virtual DoorActionResult TryLock(LockSide side)
		{
			Debug.Log(C.method(this, "cyan"));
			/*
			// uses jammed init to let know cannot be locked
			if (!canBeLocked)
				return DoorActionResult.Failure;
			*/
			// Cancel auto-lock timer when manually locking
			CancelAutoLockTimer();

			// Common lock
			if (usesCommonLock)
				return PerformCommonLock();
			// Separate locks
			if (side == LockSide.Inside)
			{
				if (initInsideUnlockedJammed)
					return DoorActionResult.UnlockedJam;
				if (currInsideLockState == DoorLockState.Locked)
					return DoorActionResult.AlreadyInState;
				if (currInsideLockState == DoorLockState.Locking ||
					currInsideLockState == DoorLockState.Unlocking)
					return DoorActionResult.AnimationInProgress;

				currInsideLockState = DoorLockState.Locking;
				_animator.trySetTrigger(DoorAnimParamType.lockInside);
				PlayAudio(_doorLockSound); // Play lock sound
				OnLockStateChanged?
					.Invoke(currInsideLockState, LockSide.Inside);
				return DoorActionResult.Success;
			}
			else if (side == LockSide.Outside)
			{
				if (initOutsideUnlockedJammed)
					return DoorActionResult.UnlockedJam;
				if (currOutsideLockState == DoorLockState.Locked)
					return DoorActionResult.AlreadyInState;
				if (currOutsideLockState == DoorLockState.Locking ||
					currOutsideLockState == DoorLockState.Unlocking)
					return DoorActionResult.AnimationInProgress;

				currOutsideLockState = DoorLockState.Locking;
				_animator.trySetTrigger(DoorAnimParamType.lockOutside);
				PlayAudio(_doorLockSound); // Play lock sound
				OnLockStateChanged?.Invoke(currOutsideLockState, LockSide.Outside);
				return DoorActionResult.Success;
			}

			return DoorActionResult.Failure;
		}
		public virtual DoorActionResult TryUnlock(LockSide side, string unlockKey = "any")
		{
			Debug.Log(C.method(this, "cyan"));

			// Check keycard requirement
			if (_requiresKeycard && unlockKey == "any")
			{
				// should word with a globalKey that works with any lock
				/*
				Debug.Log(C.method(null, "yellow", "Keycard required - no key provided"));
				return DoorActionResult.WrongKeyToUnlock;
				*/
			}

			// Check key/keycard ID match
			if (this.requiresKeycard)
				if (unlockKey != "any" && unlockKey != this.keyId)
				{
					Debug.Log(C.method(null, "yellow", $"Wrong key/keycard: got '{unlockKey}', need '{_keyId}'"));
					return DoorActionResult.WrongKeyToUnlock;
				}

			// Common lock
			if (usesCommonLock)
				return PerformCommonUnlock();

			// Separate locks
			if (side == LockSide.Inside)
			{
				if (currInsideLockState == DoorLockState.Unlocked)
					return DoorActionResult.AlreadyInState;
				if (currInsideLockState == DoorLockState.Locking ||
					currInsideLockState == DoorLockState.Unlocking)
					return DoorActionResult.AnimationInProgress;

				currInsideLockState = DoorLockState.Unlocking;
				_animator.trySetTrigger(DoorAnimParamType.unlockInside);
				PlayAudio(_doorUnlockSound); // Play unlock sound
				OnLockStateChanged?.Invoke(currInsideLockState, LockSide.Inside);

				// Start auto-lock timer if configured
				StartAutoLockTimer();

				return DoorActionResult.Success;
			}
			else if (side == LockSide.Outside)
			{
				if (currOutsideLockState == DoorLockState.Unlocked)
					return DoorActionResult.AlreadyInState;
				if (currOutsideLockState == DoorLockState.Locking ||
					currOutsideLockState == DoorLockState.Unlocking)
					return DoorActionResult.AnimationInProgress;

				currOutsideLockState = DoorLockState.Unlocking;
				_animator.trySetTrigger(DoorAnimParamType.unlockOutside);
				PlayAudio(_doorUnlockSound); // Play unlock sound
				OnLockStateChanged?.Invoke(currOutsideLockState, LockSide.Outside);

				// Start auto-lock timer if configured
				StartAutoLockTimer();

				return DoorActionResult.Success;
			}

			return DoorActionResult.Failure;
		}
		//
		public virtual DoorActionResult ForceBlock()
		{
			// such that it cannot be opened or closed, after it reach its destination(in animation)
			Debug.Log(C.method(this, "orange"));
			isBlocked = true;
			return DoorActionResult.Blocked;
		}
		public virtual DoorActionResult ForceUnblock()
		{
			Debug.Log(C.method(this, "lime"));
			isBlocked = false;
			return DoorActionResult.Success;
		}
		public virtual DoorActionResult TryStartSwaying()
		{
			Debug.Log(C.method(this, "cyan"));

			// Can only sway from stable states
			if (currDoorState != DoorState.Opened && currDoorState != DoorState.Closed)
				return DoorActionResult.AnimationInProgress;

			// Force unlock both locks before swaying (supernatural)
			if (usesCommonLock)
				TryUnlock(LockSide.Any);
			else
			{
				TryUnlock(LockSide.Inside);
				TryUnlock(LockSide.Outside);
			}

			// Start swaying
			currDoorState = DoorState.Swaying;
			_animator.trySetTrigger(DoorAnimParamType.doorSwayStart);
			_animator.trySetBool(DoorAnimParamType.isDoorSwaying, true);

			// Play looping sway sound
			if (_doorSwayLoopSound != null && _audioSource != null)
			{
				_audioSource.clip = _doorSwayLoopSound;
				_audioSource.loop = true;
				_audioSource.Play();
			}

			OnDoorStateChanged?.Invoke(currDoorState);
			return DoorActionResult.Success;
		}
		public virtual DoorActionResult TryStopSwaying(DoorState targetState = DoorState.Opened)
		{
			Debug.Log(C.method(this, "cyan"));

			if (currDoorState != DoorState.Swaying)
				return DoorActionResult.Failure;

			// Stop looping sway sound
			if (_audioSource != null && _audioSource.isPlaying && _audioSource.loop)
			{
				_audioSource.Stop();
				_audioSource.loop = false;
			}

			// Stop swaying animation
			_animator.trySetTrigger(DoorAnimParamType.doorSwayStop);
			_animator.trySetBool(DoorAnimParamType.isDoorSwaying, false);

			// Transition to target state
			currDoorState = targetState;
			OnDoorStateChanged?.Invoke(currDoorState);

			return DoorActionResult.Success;
		}

		// ========================================================================
		// ANIMATION CALLBACKS
		// ========================================================================
		public virtual void OnAnimationComplete(AnimationEventType eventType)
		{
			Debug.Log(C.method(this, "lime", $"{eventType}"));
			switch (eventType)
			{
				case AnimationEventType.DoorOpeningComplete:
					currDoorState = DoorState.Opened;
					_animator.trySetBool(DoorAnimParamType.isDoorOpen, true); // act as init behaviour, swaying util
					OnDoorStateChanged?.Invoke(currDoorState);
					break;

				case AnimationEventType.DoorClosingComplete:
					currDoorState = DoorState.Closed;
					_animator.trySetBool(DoorAnimParamType.isDoorOpen, false); // act as init behaviour, swaying util
					OnDoorStateChanged?.Invoke(currDoorState);
					break;

				case AnimationEventType.InsideLockingComplete:
					currInsideLockState = DoorLockState.Locked;
					_animator.trySetBool(DoorAnimParamType.isInsideLocked, true);
					OnLockStateChanged?.Invoke(currInsideLockState, LockSide.Inside); 
					break;

				case AnimationEventType.InsideUnlockingComplete:
					currInsideLockState = DoorLockState.Unlocked;
					_animator.trySetBool(DoorAnimParamType.isInsideLocked, false);
					OnLockStateChanged?.Invoke(currInsideLockState, LockSide.Inside);
					break;

				case AnimationEventType.OutsideLockingComplete:
					currOutsideLockState = DoorLockState.Locked;
					_animator.trySetBool(DoorAnimParamType.isOutsideLocked, true);
					OnLockStateChanged?.Invoke(currOutsideLockState, LockSide.Outside);
					break;

				case AnimationEventType.OutsideUnlockingComplete:
					currOutsideLockState = DoorLockState.Unlocked;
					_animator.trySetBool(DoorAnimParamType.isOutsideLocked, false);
					OnLockStateChanged?.Invoke(currOutsideLockState, LockSide.Outside);
					break;
			}
		}

		// ========================================================================
		// AUTO-LOCK SYSTEM
		// ========================================================================
		protected virtual void StartAutoLockTimer()
		{
			if (_autoLockDelay <= 0f)
				return; // Auto-lock disabled

			// Cancel any existing timer
			if (_autoLockCoroutine != null)
				StopCoroutine(_autoLockCoroutine);

			// Start new timer
			_autoLockCoroutine = StartCoroutine(AutoLockAfterDelay());
		}
		protected virtual void CancelAutoLockTimer()
		{
			if (_autoLockCoroutine != null)
			{
				StopCoroutine(_autoLockCoroutine);
				_autoLockCoroutine = null;
			}
		}
		protected virtual System.Collections.IEnumerator AutoLockAfterDelay()
		{
			Debug.Log(C.method(this, "grey", $"Auto-lock timer started: {_autoLockDelay}s"));
			yield return new WaitForSeconds(_autoLockDelay);

			// Auto-lock both sides (or common lock)
			// IMPORTANT: Animation will play, audio will play via TryLock()
			if (usesCommonLock)
			{
				var result = TryLock(LockSide.Any);
				Debug.Log(C.method(this, "grey", $"Auto-lock triggered: {result}"));
			}
			else
			{
				var insideResult = TryLock(LockSide.Inside);
				var outsideResult = TryLock(LockSide.Outside);
				Debug.Log(C.method(this, "grey", $"Auto-lock triggered: Inside={insideResult}, Outside={outsideResult}"));
			}

			_autoLockCoroutine = null;
		}

		// ========================================================================
		// AUDIO SYSTEM
		// ========================================================================

		/// <summary>
		/// Plays audio clip. Called automatically by Try* methods.
		/// Override this for custom audio behavior (pitch variation, 3D falloff, etc.)
		/// </summary>
		protected virtual void PlayAudio(AudioClip clip)
		{

			if (clip == null || _audioSource == null)
			{
				Debug.Log(C.method(this, "red", adMssg: $"{clip} == null || {this._audioSource} == null"));
			}

			// Don't interrupt looping sounds
			if (_audioSource.isPlaying && _audioSource.loop) return;

			_audioSource.PlayOneShot(clip);
		}

		// ========================================================================
		// HELPER METHODS
		// ========================================================================
		protected virtual DoorActionResult PerformCommonLock()
		{
			// For common locks, outside lock is active, inside is jammed
			if (!initOutsideUnlockedJammed && initInsideUnlockedJammed)
			{
				if (currOutsideLockState == DoorLockState.Unlocked)
				{
					currOutsideLockState = DoorLockState.Locking;
					_animator.trySetTrigger(DoorAnimParamType.lockOutside);
					PlayAudio(_doorLockSound); // Play lock sound
					OnLockStateChanged?.Invoke(currOutsideLockState, LockSide.Any);
					return DoorActionResult.Success;
				}
				if (currOutsideLockState == DoorLockState.Locking ||
					currOutsideLockState == DoorLockState.Unlocking)
					return DoorActionResult.AnimationInProgress;
			}

			Debug.Log(C.method(null, "red", "Invalid common lock configuration"));
			return DoorActionResult.Failure;
		}
		protected virtual DoorActionResult PerformCommonUnlock()
		{
			if (!initOutsideUnlockedJammed && initInsideUnlockedJammed)
			{
				if (currOutsideLockState == DoorLockState.Locked)
				{
					currOutsideLockState = DoorLockState.Unlocking;
					_animator.trySetTrigger(DoorAnimParamType.unlockOutside);
					PlayAudio(_doorUnlockSound); // Play unlock sound
					OnLockStateChanged?.Invoke(currOutsideLockState, LockSide.Any);
					return DoorActionResult.Success;
				}
				if (currOutsideLockState == DoorLockState.Locking ||
					currOutsideLockState == DoorLockState.Unlocking)
					return DoorActionResult.AnimationInProgress;
			}

			Debug.Log(C.method(null, "red", "Invalid common lock configuration"));
			return DoorActionResult.Failure;
		}
		protected virtual string GetDebugInfo()
		{
			return $@"=== DOOR DEBUG INFO ===
Config:
  usesCommonLock: {usesCommonLock}
  requiresKeycard: {requiresKeycard}
  keyId: {(_keyId == "" ? "(none)" : _keyId)}
  autoLockDelay: {(_autoLockDelay <= 0 ? "disabled" : $"{_autoLockDelay}s")}

State:
  isBlocked: {isBlocked}
  currDoorState: {currDoorState}
  currInsideLockState: {currInsideLockState}
  currOutsideLockState: {currOutsideLockState}
  autoLockActive: {(_autoLockCoroutine != null ? "YES" : "NO")}

Init:
  initInsideUnlockedJammed: {initInsideUnlockedJammed}
  initOutsideUnlockedJammed: {initOutsideUnlockedJammed}";
		}
	}
}