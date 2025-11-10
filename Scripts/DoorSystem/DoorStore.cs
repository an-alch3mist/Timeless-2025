using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using SPACE_UTIL;

namespace SPACE_GAME
{
	// ========================================================================
	// ENUMS
	// ========================================================================

	public enum DoorState
	{
		closed,
		opening,
		opened,
		closing,
		swaying,
	}

	public enum DoorLockState
	{
		locked,
		unlocked,
	}

	public enum DoorSide
	{
		inside,
		outside,
	}

	public enum ResourceType
	{
		audio__doorLocked,
		audio__doorOpen,
		audio__doorClose,
		audio__doorUnlock,
		audio__doorCreak,
	}


	// Animator parameters
	public enum AnimParamType
	{
		// Door movement triggers
		doorOpen,
		doorClose,

		// Lock/Unlock triggers (4 separate actions)
		lockInside,
		unlockInside,
		lockOutside,
		unlockOutside,

		// Lock triggers for common lock (single handle)
		lockCommon,
		unlockCommon,

		// Feedback triggers
		doorBlockedJiggle,     // When blocked while door was (.opened or .closed) and (trying to open/close)
		doorLockedJiggle,      // When locked and trying to open

		// State bools for animator conditions
		isInsideLocked,        // Bool: reflects doorLockStateInside
		isOutsideLocked,       // Bool: reflects doorLockStateOutside
		isDoorOpen,            // Bool: reflects doorState == opened
	}

	// ========================================================================
	// INTERFACE
	// ========================================================================

	/// <summary>
	/// Core door interface for first-person door systems.
	/// Supports dual-sided locks, blocking, and supernatural swaying.
	/// </summary>
	public interface IDoor
	{
		// ===== STATE =====
		bool blocked { get; }
		bool usesCommonLock { get; }
		DoorState doorState { get; }
		DoorLockState doorLockStateInside { get; }
		DoorLockState doorLockStateOutside { get; }

		// ===== LOCK OPERATIONS =====
		bool TryLockInside();
		bool TryLockOutside();
		bool TryUnlockInside();
		bool TryUnlockOutside();

		// ===== DOOR OPERATIONS =====
		bool TryOpen(DoorSide fromSide);
		bool TryClose();

		// ===== SUPERNATURAL OPERATIONS =====
		bool TryBlock();
		bool TryUnblock();
		bool TryDoorSwaying();
		bool TryDoorStopSwaying(DoorState doorStateAfterStop = DoorState.opened);
	}

	// ========================================================================
	// EVENTS
	// ========================================================================

	/// <summary>
	/// Door system events for gameplay integration
	/// </summary>
	public static class DoorEvents
	{
		// Door state events
		public static UnityEvent<string, DoorState> OnDoorStateChanged = new UnityEvent<string, DoorState>();
		public static UnityEvent<string, DoorLockState, DoorSide> OnDoorLockStateChanged = new UnityEvent<string, DoorLockState, DoorSide>();

		// Feedback events
		public static UnityEvent<string> OnDoorBlocked = new UnityEvent<string>();
		public static UnityEvent<string> OnDoorLockedJiggle = new UnityEvent<string>();

		// Swaying events
		public static UnityEvent<string> OnDoorSwayingStart = new UnityEvent<string>();
		public static UnityEvent<string> OnDoorSwayingStop = new UnityEvent<string>();
	}

	// ========================================================================
	// GAME SAVE DATA STRUCTURES
	// ========================================================================

	/// <summary>
	/// Serializable door state for save/load
	/// </summary>
	[System.Serializable]
	public class DoorData
	{
		public string doorId;
		public DoorState doorState;
		public DoorLockState lockStateInside;
		public DoorLockState lockStateOutside;
		public bool blocked;
	}

	/// <summary>
	/// Collection of all door states in scene
	/// </summary>
	[System.Serializable]
	public class DoorRegistry
	{
		public List<DoorData> doors = new List<DoorData>();

		public void Save()
		{
			LOG.SaveGameData(GameDataType.doorRegeistryData, this.ToJson());
		}
		// GameStore.doorRegistry = LOG.LoadGameData<DoorRegistry>(GameDataType.doorRegestry); // <- Loading is done via
	}

	// ========================================================================
	// ANIMATION PARAMETERS (Already in GameStore.cs AnimParamType)
	// ========================================================================

	// These are already defined in your GameStore.cs:
	// doorOpen, doorClose, lockInside, unlockInside, lockOutside, unlockOutside
	// lockCommon, unlockCommon, doorBlockedJiggle, doorLockedJiggle
	// isInsideLocked, isOutsideLocked, isDoorOpen
}