using System;
using UnityEngine;
using SPACE_UTIL;

/// <summary>
/// Hinged door implementation with animator-driven animations.
/// Supports separate inside/outside locks OR single common lock (keypad/gate).
/// All state changes flow through Try* methods → Animator → OnAnimationComplete callbacks.
/// </summary>
public class DoorHinged : MonoBehaviour
{
	// ========================================================================
	// INSPECTOR CONFIGURATION - Serialized initial state
	// ========================================================================
	
	[Header("Door Configuration")]
	[Tooltip("Set initial door state - animator will sync to match at game start")]
	[SerializeField] private DoorState initialDoorState = DoorState.Opened;

	[Tooltip("Set initial inside lock state")]
	[SerializeField] private DoorLockState initialInsideLockState = DoorLockState.Locked;

	[Tooltip("Set initial outside lock state")]
	[SerializeField] private DoorLockState initialOutsideLockState = DoorLockState.Unlocked;

	[Space(10)]
	[Tooltip("True = single lock (keypad/gate), False = separate inside/outside locks")]
	[SerializeField] private bool usesCommonLock = false;

	[Tooltip("False = door can never be locked (always unlocked)")]
	[SerializeField] private bool canBeLocked = true;

	[Tooltip("Number of retry attempts before forcing door closed when obstructed")]
	[SerializeField] private int maxCloseRetries = 5;

	[Header("References")]
	[SerializeField] private Animator animator;

	[Tooltip("Layer mask for objects that block door closure (e.g. 'Box' layer)")]
	[SerializeField] private LayerMask obstructionLayer;

	// Internal chaining state - when unlock animation completes, auto-open if this is true
	private bool _pendingOpen = false;

	// Close retry tracking
	private int _closeRetryCount = 0;

	// Sway stop target
	private DoorState _targetStateAfterSway = DoorState.Opened;

	// ========================================================================
	// INTERFACE PROPERTIES - Public read-only access to state
	// ========================================================================

	public bool IsBlocked { get; private set; } = false;
	public bool UsesCommonLock => usesCommonLock;
	public bool CanBeLocked => canBeLocked;
	public bool IsAnimatingDoorPanel { get; private set; } = false;

	public DoorState currDoorState { get; private set; }
	public DoorLockState InsideLockState { get; private set; }
	public DoorLockState OutsideLockState { get; private set; }

	public int maxClosingRetries
	{
		get => maxCloseRetries;
		set => maxCloseRetries = Mathf.Max(1, value); // Ensure at least 1 retry
	}

	// ========================================================================
	// EVENTS - Decoupled notification system
	// ========================================================================

	public event Action<DoorState> OnDoorStateChanged;
	public event Action<DoorLockState, LockSide> OnLockStateChanged;
	// ========================================================================
	// UNITY LIFECYCLE
	// ========================================================================

	private void Awake()
	{
		// Validate references
		if (animator == null)
		{
			
			Debug.LogError($"[DoorHinged] Animator not assigned on {gameObject.name}!", this);
			enabled = false;
			return;
		}

		// CRITICAL: Initialize state from inspector values
		Init();
	}
	private void OnTriggerStay(Collider other)
	{
		// Only check during closing animation
		if (currDoorState != DoorState.Closing) return;
		// if (!obstructionLayer.Contains(other.gameObject.layer)) return;
		if (!obstructionLayer.contains(gameObject)) return;

		// Obstruction detected during close
		_closeRetryCount++;

		if (_closeRetryCount < maxCloseRetries)
		{
			// Reverse back to opening - door bounces back
			Debug.Log($"[DoorHinged] Obstruction detected, retry {_closeRetryCount}/{maxCloseRetries}");
			animator.SetTrigger("doorOpen");
			currDoorState = DoorState.Opening;
		}
		else
		{
			// Force close after max retries (door slams shut regardless of obstruction)
			Debug.Log($"[DoorHinged] Max retries reached, forcing door closed");
			_closeRetryCount = 0;
			// Let closing animation continue without interruption
		}
	}

	// ========================================================================
	// INITIALIZATION - Syncs animator to inspector state WITHOUT playing animations
	// ========================================================================
	public void Init()
	{
		// Copy inspector values to runtime state
		currDoorState = initialDoorState;
		InsideLockState = initialInsideLockState;
		OutsideLockState = initialOutsideLockState;

		// Sync animator bools to match initial state - this makes animator jump to correct idle states
		// WITHOUT playing transition animations (bools set initial layer states)
		animator.SetBool("isDoorOpen", currDoorState == DoorState.Opened);
		animator.SetBool("isInsideLocked", InsideLockState == DoorLockState.Locked);
		animator.SetBool("isOutsideLocked", OutsideLockState == DoorLockState.Locked);
		animator.SetBool("isDoorSwaying", false);

		// Force animator to evaluate immediately so door appears in correct state on first frame
		animator.Update(0f);

		Debug.Log($"[DoorHinged] Initialized - State: {currDoorState}, InsideLock: {InsideLockState}, OutsideLock: {OutsideLockState}");
	}

	// ========================================================================
	// PUBLIC API - Door Open/Close
	// ========================================================================
	public DoorActionResult TryOpen()
	{
		// Validation checks
		if (IsAnimatingDoorPanel) return DoorActionResult.AnimationInProgress;
		if (IsBlocked) return DoorActionResult.Blocked;
		if (currDoorState == DoorState.Opened) return DoorActionResult.AlreadyInState;
		if (currDoorState == DoorState.Swaying) return DoorActionResult.Blocked; // Can't open while swaying

		// Check if door is locked - need to unlock first
		bool insideLocked = InsideLockState == DoorLockState.Locked;
		bool outsideLocked = OutsideLockState == DoorLockState.Locked;

		if (insideLocked || outsideLocked)
		{
			// Can't open locked door - caller should unlock first
			return DoorActionResult.Locked;
		}

		// All checks passed - trigger opening animation
		IsAnimatingDoorPanel = true;
		currDoorState = DoorState.Opening;
		animator.trySetBool(DoorAnimParamType.isDoorOpen, true);
		animator.trySetTrigger(DoorAnimParamType.doorOpen);

		OnDoorStateChanged?.Invoke(DoorState.Opening); // for now just log, without any action
		return DoorActionResult.Success;
	}
	public DoorActionResult TryClose()
	{
		// Validation checks
		if (IsAnimatingDoorPanel) return DoorActionResult.AnimationInProgress;
		if (IsBlocked) return DoorActionResult.Blocked;
		if (currDoorState == DoorState.Closed) return DoorActionResult.AlreadyInState;
		if (currDoorState == DoorState.Swaying) return DoorActionResult.Blocked; // Can't close while swaying

		// Reset retry counter for new close attempt
		_closeRetryCount = 0;

		// Trigger closing animation
		TryUnlock(LockSide.Inside);
		TryUnlock(LockSide.Outside);

		IsAnimatingDoorPanel = true;
		currDoorState = DoorState.Closing;
		animator.trySetBool(DoorAnimParamType.isDoorOpen, false);
		animator.trySetTrigger(DoorAnimParamType.doorClose);

		OnDoorStateChanged?.Invoke(DoorState.Closing);
		return DoorActionResult.Success;
	}

	// ========================================================================
	// PUBLIC API - Lock/Unlock (Unified Interface)
	// ========================================================================
	public DoorActionResult TryLock(LockSide side)
	{
		if (IsAnimatingDoorPanel) return DoorActionResult.AnimationInProgress;
		if (!canBeLocked) return DoorActionResult.Blocked; // Door cannot be locked

		// Common lock path (keypad/gate with single lock mechanism)
		if (usesCommonLock)
		{
			if (side != LockSide.Any) return DoorActionResult.WrongKeyToUnlock; // Must use LockSide.Both
			if (InsideLockState == DoorLockState.Locked) return DoorActionResult.AlreadyInState;

			IsAnimatingDoorPanel = true;
			animator.SetTrigger("lockCommon");
			return DoorActionResult.Success;
		}

		// Separate locks path (traditional door with inside/outside handles)
		if (side == LockSide.Any) return DoorActionResult.WrongKeyToUnlock; // Can't lock both separately

		if (side == LockSide.Inside)
		{
			if (InsideLockState == DoorLockState.Locked) return DoorActionResult.AlreadyInState;
			IsAnimatingDoorPanel = true;
			animator.SetBool("isInsideLocked", true);
			animator.SetTrigger("lockInside");
		}
		else // LockSide.Outside
		{
			if (OutsideLockState == DoorLockState.Locked) return DoorActionResult.AlreadyInState;
			IsAnimatingDoorPanel = true;
			animator.SetBool("isOutsideLocked", true);
			animator.SetTrigger("lockOutside");
		}

		return DoorActionResult.Success;
	}
	public DoorActionResult TryUnlock(LockSide side)
	{
		if (IsAnimatingDoorPanel) return DoorActionResult.AnimationInProgress;

		// Common lock path
		if (usesCommonLock)
		{
			if (side != LockSide.Any) return DoorActionResult.WrongKeyToUnlock;
			if (InsideLockState == DoorLockState.Unlocked) return DoorActionResult.AlreadyInState;

			IsAnimatingDoorPanel = true;
			animator.SetTrigger("unlockCommon");
			return DoorActionResult.Success;
		}

		// Separate locks path
		if (side == LockSide.Any) return DoorActionResult.WrongKeyToUnlock;

		if (side == LockSide.Inside)
		{
			if (InsideLockState == DoorLockState.Unlocked) return DoorActionResult.AlreadyInState;
			IsAnimatingDoorPanel = true;
			animator.SetBool("isInsideLocked", false);
			animator.SetTrigger("unlockInside");
		}
		else // LockSide.Outside
		{
			if (OutsideLockState == DoorLockState.Unlocked) return DoorActionResult.AlreadyInState;
			IsAnimatingDoorPanel = true;
			animator.SetBool("isOutsideLocked", false);
			animator.SetTrigger("unlockOutside");
		}

		return DoorActionResult.Success;
	}

	// ========================================================================
	// PUBLIC API - Supernatural Abilities
	// ========================================================================
	public DoorActionResult TryBlock()
	{
		if (IsBlocked) return DoorActionResult.AlreadyInState;
		IsBlocked = true;
		return DoorActionResult.Success;
	}
	public DoorActionResult TryUnblock()
	{
		if (!IsBlocked) return DoorActionResult.AlreadyInState;
		IsBlocked = false;
		return DoorActionResult.Success;
	}
	public DoorActionResult TryStartSwaying()
	{
		if (IsAnimatingDoorPanel) return DoorActionResult.AnimationInProgress;
		if (currDoorState == DoorState.Swaying) return DoorActionResult.AlreadyInState;

		// Supernatural override - force unlock both sides (no animation played, instant)
		InsideLockState = DoorLockState.Unlocked;
		OutsideLockState = DoorLockState.Unlocked;
		animator.SetBool("isInsideLocked", false);
		animator.SetBool("isOutsideLocked", false);

		// Start swaying
		IsAnimatingDoorPanel = true;
		currDoorState = DoorState.Swaying;
		animator.SetBool("isDoorSwaying", true);
		animator.SetTrigger("doorSwayStart");

		OnDoorStateChanged?.Invoke(DoorState.Swaying);
		return DoorActionResult.Success;
	}
	public DoorActionResult TryStopSwaying(DoorState targetState = DoorState.Opened)
	{
		if (currDoorState != DoorState.Swaying) return DoorActionResult.AlreadyInState;

		// Store target state for OnAnimationComplete callback
		_targetStateAfterSway = targetState;

		animator.SetBool("isDoorSwaying", false);
		animator.SetTrigger("doorSwayStop");

		// Animation will handle transition to target state
		return DoorActionResult.Success;
	}

	// ========================================================================
	// ANIMATION CALLBACK - Called by DoorAnimationEventForwarder component
	// ========================================================================

	public void OnAnimationComplete(AnimationEventType eventType)
	{
		Debug.Log(C.method(this, "cyan"));
		switch (eventType)
		{
			// ----------------------------------------------------------------
			// Door Movement Events
			// ----------------------------------------------------------------
			case AnimationEventType.DoorOpeningStarted:
				// Opening animation started (optional callback)
				break;
			case AnimationEventType.DoorOpeningComplete:
				IsAnimatingDoorPanel = false;
				currDoorState = DoorState.Opened;
				animator.SetBool("isDoorOpen", true);  // ← ADD THIS
				OnDoorStateChanged?.Invoke(DoorState.Opened);
				break;

			case AnimationEventType.DoorClosingComplete:
				IsAnimatingDoorPanel = false;
				currDoorState = DoorState.Closed;
				animator.SetBool("isDoorOpen", false);  // ← ADD THIS
				OnDoorStateChanged?.Invoke(DoorState.Closed);
				break;

			case AnimationEventType.DoorClosingStarted:
				// Closing animation started (optional callback)
				break;

			// ----------------------------------------------------------------
			// Inside Lock Events
			// ----------------------------------------------------------------
			case AnimationEventType.InsideLockingStarted:
				// Locking animation started (optional callback)
				break;

			case AnimationEventType.InsideLockingComplete:
				IsAnimatingDoorPanel = false;
				InsideLockState = DoorLockState.Locked;
				OnLockStateChanged?.Invoke(DoorLockState.Locked, LockSide.Inside);
				break;

			case AnimationEventType.InsideUnlockingStarted:
				// Unlocking animation started (optional callback)
				break;

			case AnimationEventType.InsideUnlockingComplete:
				IsAnimatingDoorPanel = false;
				InsideLockState = DoorLockState.Unlocked;
				OnLockStateChanged?.Invoke(DoorLockState.Unlocked, LockSide.Inside);

				// CHAINING: If open was attempted while locked, auto-open now
				if (_pendingOpen)
				{
					_pendingOpen = false;
					TryOpen();
				}
				break;

			// ----------------------------------------------------------------
			// Outside Lock Events
			// ----------------------------------------------------------------
			case AnimationEventType.OutsideLockingStarted:
				// Locking animation started (optional callback)
				break;

			case AnimationEventType.OutsideLockingComplete:
				IsAnimatingDoorPanel = false;
				OutsideLockState = DoorLockState.Locked;
				OnLockStateChanged?.Invoke(DoorLockState.Locked, LockSide.Outside);
				break;

			case AnimationEventType.OutsideUnlockingStarted:
				// Unlocking animation started (optional callback)
				break;

			case AnimationEventType.OutsideUnlockingComplete:
				IsAnimatingDoorPanel = false;
				OutsideLockState = DoorLockState.Unlocked;
				OnLockStateChanged?.Invoke(DoorLockState.Unlocked, LockSide.Outside);

				// CHAINING: If open was attempted while locked, auto-open now
				if (_pendingOpen)
				{
					_pendingOpen = false;
					TryOpen();
				}
				break;

			// ----------------------------------------------------------------
			// Common Lock Events (keypad/gate)
			// ----------------------------------------------------------------
			case AnimationEventType.CommonLockingStarted:
				// Locking animation started (optional callback)
				break;

			case AnimationEventType.CommonLockingComplete:
				IsAnimatingDoorPanel = false;
				InsideLockState = DoorLockState.Locked;
				OutsideLockState = DoorLockState.Locked;
				OnLockStateChanged?.Invoke(DoorLockState.Locked, LockSide.Any);
				break;

			case AnimationEventType.CommonUnlockingStarted:
				// Unlocking animation started (optional callback)
				break;

			case AnimationEventType.CommonUnlockingComplete:
				IsAnimatingDoorPanel = false;
				InsideLockState = DoorLockState.Unlocked;
				OutsideLockState = DoorLockState.Unlocked;
				OnLockStateChanged?.Invoke(DoorLockState.Unlocked, LockSide.Any);

				// CHAINING: If open was attempted while locked, auto-open now
				if (_pendingOpen)
				{
					_pendingOpen = false;
					TryOpen();
				}
				break;

			// ----------------------------------------------------------------
			// Supernatural Events
			// ----------------------------------------------------------------
			case AnimationEventType.DoorSwayStarted:
				// Sway loop started (optional callback)
				break;

			case AnimationEventType.DoorSwayStopped:
				IsAnimatingDoorPanel = false;
				currDoorState = _targetStateAfterSway;
				// animator.SetBool("isDoorOpen", _targetStateAfterSway == DoorState.Opened);
				OnDoorStateChanged?.Invoke(_targetStateAfterSway);
				break;

			default:
				Debug.LogWarning($"[DoorHinged] Unhandled animation event: {eventType}");
				break;
		}
	}
}