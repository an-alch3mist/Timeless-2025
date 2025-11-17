namespace SPACE_GAME_0
{
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
				// BlockedOpened, // TODO
				// BlockedClosed, // TODO
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
		Any // For common locks (keypad, gate handle), it doesnt matter where the user toggle's it.
	}

	public enum DoorActionResult
	{
		Success,
		Failure,
		Blocked,                // Door is supernaturally blocked
		Locked,                 // Attempted open but door is locked
		UnlockedJam,
		AlreadyInState,         // Already in target state (e.g. already open)
		AnimationInProgress,    // Another action is animating - prevents spam
		WrongKeyToUnlock,          // Tried LockSide.Both on separate locks or vice versa
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
		// CONFIGURATION CONSTANT THROUGH OUT GAME
		// ========================================================================
		string key { get; set; }
		bool canBeLocked { get; set; }         // false = door can never be locked
		bool usesCommonLock { get; set; }      // true = single lock (keypad/gate), false = separate inside/outside
		bool initInsideUnlockedJammed { get; set; }
		bool initOutsideUnlockedJammed { get; set; }

		int maxClosingRetries { get; set; } // Default = 5 - how many times to retry closing before forcing


		// ========================================================================
		// READ-ONLY PROPERTIES - State can only change through Try* methods
		// ========================================================================
		bool isBlocked { get; set; }           // Supernatural block - cannot open/close
		DoorState currDoorState { get; }
		DoorLockState currInsideLockState { get; }
		DoorLockState currOutsideLockState { get; }

		// ========================================================================
		// since all 4 are in different layers
		bool isAnimatingDoorPanel { get; set; }         // CRITICAL: Prevents action spam and state corruption in layer: 0
		bool isAnimatingDoorLockInside { get; set; }    // CRITICAL: Prevents action spam and state corruption in layer: 1
		bool isAnimatingDoorLockOutside { get; set; }   // CRITICAL: Prevents action spam and state corruption in layer: 2
		bool isAnimatingDoorLockCommon { get; set; }    // CRITICAL: Prevents action spam and state corruption in layer: 3


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
		/// unlock both the locks(if not unlockeJammed) if locked
		/// Attempt to close the door. Retries up to MaxCloseRetries if obstructed(rigid body hit with door panel).
		/// After max retries, forces door closed (slam shut).
		/// </summary>
		DoorActionResult TryClose();

		/// <summary>
		/// Attempt to close the door. Faster closed (slam shut). without locking, 
		/// 
		/// and different sound, different animation(a faster version of shut) compared to regular close
		/// </summary>
		DoorActionResult ForceClose();

		/// <summary>
		/// Unified lock method. Use LockSide.Both for common locks, Inside/Outside for separate locks.
		/// Returns WrongLockType if side doesn't match door's lock configuration.
		/// </summary>
		DoorActionResult TryLock(LockSide side);

		/// <summary>
		/// Unified unlock method. Use LockSide.Both for common locks, Inside/Outside for separate locks.
		/// Returns WrongLockType if side doesn't match door's lock configuration.
		/// </summary>
		DoorActionResult TryUnlock(LockSide side, string key = "");

		/// <summary>
		/// Supernatural ability - blocks door from being opened/closed.
		/// Lock state remains unchanged.
		/// </summary>
		DoorActionResult ForceBlock();

		/// <summary>
		/// Supernatural ability - removes block on door.
		/// </summary>
		DoorActionResult ForceUnblock();

		/// <summary>
		/// --for future
		/// Supernatural ability - makes door sway back and forth.
		/// Forces both locks to unlock state and plays looping sway animation.
		/// Block state remains unchanged (can sway while blocked).
		/// </summary>
		DoorActionResult TryStartSwaying();

		/// <summary>
		/// --for future
		/// Stops door swaying and transitions to targetState (Opened or Closed).
		/// Typically called when player looks away or after timer.
		/// </summary>
		DoorActionResult TryStopSwaying(DoorState targetState = DoorState.Opened);

		// ========================================================================
		// JUST TO LOG
		// ========================================================================
		string getStr { get; }

		// ========================================================================
		// ANIMATION CALLBACK - Called by DoorAnimationEventForwarder
		// ========================================================================

		/// <summary>
		/// Type-safe callback from Animation Events. Sets IsAnimating = false at appropriate times.
		/// Handles state transitions and chained actions (e.g. unlock → open).
		/// </summary>
		void OnAnimationComplete(AnimationEventType eventType);

	} 
}