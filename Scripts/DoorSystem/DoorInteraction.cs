using UnityEngine;
using SPACE_UTIL;

/// <summary>
/// Simple keyboard-based door interaction system.
/// Attach to a GameObject in the scene (e.g. GameManager or Player).
/// Handles open/close/lock/unlock based on player position relative to door triggers.
/// </summary>
public class DoorInteraction : MonoBehaviour
{
	[Header("Door Reference")]
	[Tooltip("Drag the GameObject with DoorHinged component here")]
	[SerializeField] private DoorHinged door;

	[Header("Trigger References")]
	[Tooltip("Drag the 'door inside trigger' GameObject here")]
	[SerializeField] private Collider insideTrigger;

	[Tooltip("Drag the 'door outside trigger' GameObject here")]
	[SerializeField] private Collider outsideTrigger;

	[Header("Player Reference")]
	[Tooltip("Drag the player GameObject here (works with CharacterController, Rigidbody, or any GameObject)")]
	[SerializeField] private GameObject player;

	// Runtime state tracking
	private bool _isPlayerInside = false;
	private bool _isPlayerOutside = false;

	// ========================================================================
	// UNITY LIFECYCLE
	// ========================================================================

	private void Awake()
	{
		// Validate all required references
		if (door == null)
		{
			Debug.LogError("[DoorInteraction] Door reference not assigned!", this);
			enabled = false;
			return;
		}

		if (insideTrigger == null || outsideTrigger == null)
		{
			Debug.LogError("[DoorInteraction] Trigger references not assigned!", this);
			enabled = false;
			return;
		}

		if (player == null)
		{
			Debug.LogError("[DoorInteraction] Player reference not assigned!", this);
			enabled = false;
			return;
		}

		// Ensure triggers are marked as triggers
		if (!insideTrigger.isTrigger)
		{
			Debug.LogWarning("[DoorInteraction] Inside trigger is not marked as trigger! Fixing...", insideTrigger);
			insideTrigger.isTrigger = true;
		}

		if (!outsideTrigger.isTrigger)
		{
			Debug.LogWarning("[DoorInteraction] Outside trigger is not marked as trigger! Fixing...", outsideTrigger);
			outsideTrigger.isTrigger = true;
		}

		// Subscribe to door state events for logging
		door.OnDoorStateChanged += OnDoorStateChanged;
		door.OnLockStateChanged += OnLockStateChanged;
	}

	private void OnDestroy()
	{
		// Unsubscribe from events to prevent memory leaks
		if (door != null)
		{
			door.OnDoorStateChanged -= OnDoorStateChanged;
			door.OnLockStateChanged -= OnLockStateChanged;
		}
	}

	private void Update()
	{
		// Update player position relative to triggers
		UpdatePlayerPosition();

		// Handle keyboard input
		HandleInput();
	}

	// ========================================================================
	// PLAYER POSITION TRACKING
	// ========================================================================

	private void UpdatePlayerPosition()
	{
		if (player == null) return;

		// Check if player is inside either trigger
		_isPlayerInside = insideTrigger.bounds.Contains(player.transform.position);
		_isPlayerOutside = outsideTrigger.bounds.Contains(player.transform.position);
	}

	// ========================================================================
	// INPUT HANDLING
	// ========================================================================

	private void HandleInput()
	{
		// KeyCode.O - Open door
		if (Input.GetKeyDown(KeyCode.O))
		{
			HandleOpenDoor();
		}

		// KeyCode.C - Close door
		if (Input.GetKeyDown(KeyCode.C))
		{
			HandleCloseDoor();
		}

		// KeyCode.L - Lock door (side-specific)
		if (Input.GetKeyDown(KeyCode.L))
		{
			HandleLockDoor();
		}

		// KeyCode.U - Unlock door (side-specific)
		if (Input.GetKeyDown(KeyCode.U))
		{
			HandleUnlockDoor();
		}
	}

	// ========================================================================
	// DOOR ACTIONS
	// ========================================================================

	private void HandleOpenDoor()
	{
		Debug.Log(C.method(this, "yellow", adMssg: "Attempting to open door"));

		DoorActionResult result = door.TryOpen();

		switch (result)
		{
			case DoorActionResult.Success:
				Debug.Log(C.method(this, "green", adMssg: "Door opening..."));
				break;

			case DoorActionResult.Locked:
				Debug.Log(C.method(this, "red", adMssg: "Door is locked! Unlock it first with KeyCode.U"));
				break;

			case DoorActionResult.AlreadyInState:
				Debug.Log(C.method(this, "orange", adMssg: "Door is already open"));
				break;

			case DoorActionResult.AnimationInProgress:
				Debug.Log(C.method(this, "orange", adMssg: "Door is currently animating, please wait"));
				break;

			case DoorActionResult.Blocked:
				Debug.Log(C.method(this, "red", adMssg: "Door is supernaturally blocked!"));
				break;

			default:
				Debug.Log(C.method(this, "red", adMssg: $"Failed to open door: {result}"));
				break;
		}
	}

	private void HandleCloseDoor()
	{
		Debug.Log(C.method(this, "yellow", adMssg: "Attempting to close door"));

		DoorActionResult result = door.TryClose();

		switch (result)
		{
			case DoorActionResult.Success:
				Debug.Log(C.method(this, "green", adMssg: "Door closing..."));
				break;

			case DoorActionResult.AlreadyInState:
				Debug.Log(C.method(this, "orange", adMssg: "Door is already closed"));
				break;

			case DoorActionResult.AnimationInProgress:
				Debug.Log(C.method(this, "orange", adMssg: "Door is currently animating, please wait"));
				break;

			case DoorActionResult.Blocked:
				Debug.Log(C.method(this, "red", adMssg: "Door is supernaturally blocked!"));
				break;

			default:
				Debug.Log(C.method(this, "red", adMssg: $"Failed to close door: {result}"));
				break;
		}
	}

	private void HandleLockDoor()
	{
		// Determine which side to lock based on player position
		LockSide side = GetPlayerLockSide();

		if (side == LockSide.Any)
		{
			Debug.Log(C.method(this, "red", adMssg: "You must be inside or outside a trigger to lock the door!"));
			return;
		}

		Debug.Log(C.method(this, "yellow", adMssg: $"Attempting to lock door from {side} side"));

		DoorActionResult result = door.TryLock(side);

		switch (result)
		{
			case DoorActionResult.Success:
				Debug.Log(C.method(this, "green", adMssg: $"Locking door from {side} side..."));
				break;

			case DoorActionResult.AlreadyInState:
				Debug.Log(C.method(this, "orange", adMssg: $"{side} lock is already locked"));
				break;

			case DoorActionResult.AnimationInProgress:
				Debug.Log(C.method(this, "orange", adMssg: "Door is currently animating, please wait"));
				break;

			case DoorActionResult.Blocked:
				Debug.Log(C.method(this, "red", adMssg: "Door cannot be locked (canBeLocked = false)"));
				break;

			case DoorActionResult.WrongKeyToUnlock:
				Debug.Log(C.method(this, "red", adMssg: "Wrong lock type! Check if door uses common lock"));
				break;

			default:
				Debug.Log(C.method(this, "red", adMssg: $"Failed to lock door: {result}"));
				break;
		}
	}

	private void HandleUnlockDoor()
	{
		// Determine which side to unlock based on player position
		LockSide side = GetPlayerLockSide();

		if (side == LockSide.Any)
		{
			Debug.Log(C.method(this, "red", adMssg: "You must be inside or outside a trigger to unlock the door!"));
			return;
		}

		Debug.Log(C.method(this, "yellow", adMssg: $"Attempting to unlock door from {side} side"));

		DoorActionResult result = door.TryUnlock(side);

		switch (result)
		{
			case DoorActionResult.Success:
				Debug.Log(C.method(this, "green", adMssg: $"Unlocking door from {side} side..."));
				break;

			case DoorActionResult.AlreadyInState:
				Debug.Log(C.method(this, "orange", adMssg: $"{side} lock is already unlocked"));
				break;

			case DoorActionResult.AnimationInProgress:
				Debug.Log(C.method(this, "orange", adMssg: "Door is currently animating, please wait"));
				break;

			case DoorActionResult.WrongKeyToUnlock:
				Debug.Log(C.method(this, "red", adMssg: "Wrong lock type! Check if door uses common lock"));
				break;

			default:
				Debug.Log(C.method(this, "red", adMssg: $"Failed to unlock door: {result}"));
				break;
		}
	}

	// ========================================================================
	// HELPER METHODS
	// ========================================================================

	private LockSide GetPlayerLockSide()
	{
		if (door.UsesCommonLock)
		{
			// Common lock doors require LockSide.Both
			return (_isPlayerInside || _isPlayerOutside) ? LockSide.Any : LockSide.Any;
		}

		// Separate locks - determine based on trigger position
		if (_isPlayerInside && !_isPlayerOutside)
			return LockSide.Inside;
		else if (_isPlayerOutside && !_isPlayerInside)
			return LockSide.Outside;
		else
			return LockSide.Any; // Invalid state - not in either trigger
	}

	// ========================================================================
	// EVENT CALLBACKS - For logging state changes
	// ========================================================================

	private void OnDoorStateChanged(DoorState newState)
	{
		Debug.Log(C.method(this, "cyan", adMssg: $"Door state changed to: {newState}"));
	}

	private void OnLockStateChanged(DoorLockState newLockState, LockSide side)
	{
		Debug.Log(C.method(this, "cyan", adMssg: $"Lock state changed: {side} is now {newLockState}"));
	}

	// ========================================================================
	// DEBUG VISUALIZATION
	// ========================================================================

	private void OnDrawGizmos()
	{
		if (insideTrigger != null)
		{
			Gizmos.color = new Color(0, 1, 0, 0.3f); // Green
			Gizmos.DrawCube(insideTrigger.bounds.center, insideTrigger.bounds.size);
		}

		if (outsideTrigger != null)
		{
			Gizmos.color = new Color(1, 0, 0, 0.3f); // Red
			Gizmos.DrawCube(outsideTrigger.bounds.center, outsideTrigger.bounds.size);
		}

		if (player != null)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(player.transform.position, 0.5f);
		}
	}
}