using System;
using UnityEngine;
// impoved via: https://claude.ai/share/8f49bd28-aa38-4ce4-93b0-56aee148448a

// ============================================================================
// ENUMS - Stable API that won't break existing code when adding new states
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
	Unlocked,
	UnlockedJammed // Cannot be locked by player - always unlocked
}

public enum LockSide
{
	Inside,
	Outside,
	Both // For common locks (keypad, gate handle)
}

public enum DoorActionResult
{
	Success,
	Blocked,                // Door is supernaturally blocked
	Locked,                 // Attempted open but door is locked
	AlreadyInState,         // Already in target state (e.g. already open)
	AnimationInProgress,    // Another action is animating - prevents spam
	WrongLockType,          // Tried LockSide.Both on separate locks or vice versa
	ObstructionDetected     // Object blocking door closure
}

// named exact same inside animatorCOntroller too.
public enum DoorAnimParamType
{
	// trigger
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

	// boolean
	isDoorOpen,
	isInsideLocked,
	isOutsideLocked,
	isDoorSwaying,
}

public enum AnimationEventType
{
	// Door movement events
	DoorOpeningStarted,
	DoorOpeningComplete,
	DoorClosingStarted,
	DoorClosingComplete,

	// Inside lock events
	InsideLockingStarted,
	InsideLockingComplete,
	InsideUnlockingStarted,
	InsideUnlockingComplete,

	// Outside lock events
	OutsideLockingStarted,
	OutsideLockingComplete,
	OutsideUnlockingStarted,
	OutsideUnlockingComplete,

	// Common lock events (for keypad/gate locks)
	CommonLockingStarted,
	CommonLockingComplete,
	CommonUnlockingStarted,
	CommonUnlockingComplete,

	// Supernatural events
	DoorSwayStarted,
	DoorSwayStopped
}

// ============================================================================
// INTERFACE - Public contract for all door types
// ============================================================================

public interface IDoor
{
	// ========================================================================
	// READ-ONLY PROPERTIES - State can only change through Try* methods
	// ========================================================================

	bool IsBlocked { get; }           // Supernatural block - cannot open/close
	bool UsesCommonLock { get; }      // true = single lock (keypad/gate), false = separate inside/outside
	bool CanBeLocked { get; }         // false = door can never be locked
	bool IsAnimating { get; }         // CRITICAL: Prevents action spam and state corruption

	DoorState CurrentState { get; }
	DoorLockState InsideLockState { get; }
	DoorLockState OutsideLockState { get; }

	// ========================================================================
	// EVENTS - Enables loosely-coupled systems (audio, AI, quests)
	// ========================================================================

	event Action<DoorState> OnDoorStateChanged;
	event Action<DoorLockState, LockSide> OnLockStateChanged; // LockSide indicates which lock changed

	// ========================================================================
	// INITIALIZATION - Syncs animator to match inspector-set initial state
	// ========================================================================

	/// <summary>
	/// Synchronizes animator controller state to match inspector-serialized values.
	/// MUST be called from MonoBehaviour.Awake() to ensure door visuals match initial state.
	/// Example: If inspector shows door Opened + InsideLocked, animator will immediately 
	/// show door in open position with inside handle in locked position (no animation played).
	/// </summary>
	void Init();

	// ========================================================================
	// PUBLIC API - All interactions go through these methods
	// ========================================================================

	/// <summary>
	/// Attempt to open the door. Will auto-unlock if needed and both locks are unlocked.
	/// Returns AnimationInProgress if animation is running - prevents spam-clicking.
	/// </summary>
	DoorActionResult TryOpen();

	/// <summary>
	/// Attempt to close the door. Retries up to MaxCloseRetries if obstructed.
	/// After max retries, forces door closed (slam shut).
	/// </summary>
	DoorActionResult TryClose();

	/// <summary>
	/// Unified lock method. Use LockSide.Both for common locks, Inside/Outside for separate locks.
	/// Returns WrongLockType if side doesn't match door's lock configuration.
	/// </summary>
	DoorActionResult TryLock(LockSide side);

	/// <summary>
	/// Unified unlock method. Use LockSide.Both for common locks, Inside/Outside for separate locks.
	/// Returns WrongLockType if side doesn't match door's lock configuration.
	/// </summary>
	DoorActionResult TryUnlock(LockSide side);

	/// <summary>
	/// Supernatural ability - blocks door from being opened/closed.
	/// Lock state remains unchanged.
	/// </summary>
	DoorActionResult TryBlock();

	/// <summary>
	/// Supernatural ability - removes block on door.
	/// </summary>
	DoorActionResult TryUnblock();

	/// <summary>
	/// Supernatural ability - makes door sway back and forth.
	/// Forces both locks to unlock state and plays looping sway animation.
	/// Block state remains unchanged (can sway while blocked).
	/// </summary>
	DoorActionResult TryStartSwaying();

	/// <summary>
	/// Stops door swaying and transitions to targetState (Opened or Closed).
	/// Typically called when player looks away or after timer.
	/// </summary>
	DoorActionResult TryStopSwaying(DoorState targetState = DoorState.Opened);

	// ========================================================================
	// ANIMATION CALLBACK - Called by DoorAnimationEventForwarder
	// ========================================================================

	/// <summary>
	/// Type-safe callback from Animation Events. Sets IsAnimating = false at appropriate times.
	/// Handles state transitions and chained actions (e.g. unlock → open).
	/// </summary>
	void OnAnimationComplete(AnimationEventType eventType);

	// ========================================================================
	// CONFIGURATION
	// ========================================================================

	int MaxCloseRetries { get; set; } // Default = 5 - how many times to retry closing before forcing
}