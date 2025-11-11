using UnityEngine;
using SPACE_UTIL;

/// <summary>
/// Example script showing how to interact with IDoor from player/AI code.
/// Attach to player controller or interaction system.
/// </summary>
public class DoorInteraction : MonoBehaviour
{
	[SerializeField] private LayerMask doorLayer;
	[SerializeField] private float interactionRange = 10f;
	[SerializeField] private KeyCode openCloseKey = KeyCode.E;
	[SerializeField] private KeyCode lockKey = KeyCode.L;
	[SerializeField] private KeyCode unlockKey = KeyCode.U;

	[SerializeField] private IDoor _currentDoor; // Door player is looking at
	[SerializeField] private Camera _playerCamera;

	private void Start()
	{
		Debug.Log(C.method(this));
		_currentDoor = this.GetComponent<DoorHinged>();
		Debug.Log(_currentDoor.CanBeLocked);
		// _playerCamera = Camera.main;

		// Example: Subscribe to door events for UI feedback
		// In production, use event bus or UI manager
	}
	private void Update()
	{
		// DetectDoorLookAt();
		HandleInput();
	}

	private void DetectDoorLookAt()
	{
		Ray ray = _playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

		if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, doorLayer))
		{
			IDoor door = hit.collider.GetComponent<IDoor>();

			if (door != _currentDoor)
			{
				_currentDoor = door;
				// Show UI prompt: "Press E to open/close"
			}
		}
		else
		{
			if (_currentDoor != null)
			{
				_currentDoor = null;
				// Hide UI prompt
			}
		}
	}
	private void HandleInput()
	{
		if (_currentDoor == null) return;

		// Open/Close door
		if (Input.GetKeyDown(openCloseKey))
		{
			DoorActionResult result = _currentDoor.CurrentState == DoorState.Closed
				? _currentDoor.TryOpen()
				: _currentDoor.TryClose();

			HandleDoorActionResult(result, "open/close");
		}

		// Lock door (from player's side - determine inside/outside based on player position)
		if (Input.GetKeyDown(lockKey))
		{
			LockSide side = DeterminePlayerSide();
			DoorActionResult result = _currentDoor.TryLock(side);
			HandleDoorActionResult(result, justToLogAction: "lock");
		}

		// Unlock door
		if (Input.GetKeyDown(unlockKey))
		{
			LockSide side = DeterminePlayerSide();
			DoorActionResult result = _currentDoor.TryUnlock(side);
			HandleDoorActionResult(result, "unlock");
		}
	}

	private void HandleDoorActionResult(DoorActionResult result, string justToLogAction)
	{
		// Provide player feedback based on result
		switch (result)
		{
			case DoorActionResult.Success:
				Debug.Log($"Door {justToLogAction} successful".colorTag("lime"));
				// Play success audio
				break;

			case DoorActionResult.AnimationInProgress:
				Debug.Log("Wait for door animation to finish".colorTag("cyan"));
				// Play "wait" audio or show UI
				break;

			case DoorActionResult.Locked:
				Debug.Log("Door is locked! Unlock it first".colorTag("cyan"));
				// Play locked jiggle sound, show UI prompt
				break;

			case DoorActionResult.Blocked:
				Debug.Log("Something supernatural is blocking the door...".colorTag("cyan"));
				// Play eerie audio, trigger horror event
				break;

			case DoorActionResult.AlreadyInState:
				Debug.Log("Door is already in that state".colorTag("cyan"));
				break;

			case DoorActionResult.WrongLockType:
				Debug.LogError("Code error: wrong lock type used!".colorTag("cyan"));
				break;

			case DoorActionResult.ObstructionDetected:
				Debug.Log("Door blocked by object".colorTag("cyan"));
				break;
		}
	}
	private LockSide DeterminePlayerSide()
	{
		// Determine if player is on inside or outside of door
		// Simple approach: check player position relative to door forward direction

		Transform doorTransform = (_currentDoor as MonoBehaviour)?.transform;
		if (doorTransform == null) return LockSide.Inside; // Fallback

		Vector3 playerToDoor = doorTransform.position - transform.position;
		float dot = Vector3.Dot(doorTransform.forward, playerToDoor.normalized);

		// If player is in front of door's forward direction = outside, behind = inside
		return dot > 0 ? LockSide.Outside : LockSide.Inside;
	}

	// ========================================================================
	// Example: Supernatural AI System
	// ========================================================================

	/// <summary>
	/// Example of how supernatural AI would interact with doors
	/// </summary>
	public void SupernaturalEvent_BlockDoor(IDoor door)
	{
		door.TryBlock();
		Debug.Log("Ghost has blocked the door!");
		// Play spooky effects
	}
	public void SupernaturalEvent_SwayDoor(IDoor door)
	{
		if (door.TryStartSwaying() == DoorActionResult.Success)
		{
			Debug.Log("Door begins swaying ominously...");
			// Start timer to stop swaying after 3 seconds
			Invoke(nameof(StopSwaying), 3f);
		}
	}
	private void StopSwaying()
	{
		if (_currentDoor != null && _currentDoor.CurrentState == DoorState.Swaying)
		{
			// Stop swaying and leave door in opened state
			_currentDoor.TryStopSwaying(DoorState.Opened);
		}
	}

	// ========================================================================
	// Example: Event Listening
	// ========================================================================

	private void SubscribeToDoorEvents(IDoor door)
	{
		// Example: Audio system listens to door state changes
		door.OnDoorStateChanged += OnDoorStateChanged;
		door.OnLockStateChanged += OnLockStateChanged;
	}
	private void UnsubscribeFromDoorEvents(IDoor door)
	{
		door.OnDoorStateChanged -= OnDoorStateChanged;
		door.OnLockStateChanged -= OnLockStateChanged;
	}
	private void OnDoorStateChanged(DoorState newState)
	{
		Debug.Log($"Door state changed to: {newState}");

		switch (newState)
		{
			case DoorState.Opening:
				// Play door creak audio
				break;
			case DoorState.Opened:
				// Play door open complete sound
				break;
			case DoorState.Closing:
				// Play door closing audio
				break;
			case DoorState.Closed:
				// Play door slam sound
				break;
			case DoorState.Swaying:
				// Play eerie creaking loop
				break;
		}
	}
	private void OnLockStateChanged(DoorLockState newState, LockSide side)
	{
		Debug.Log($"Lock state changed to: {newState} on {side} side");

		// Play lock/unlock sound effect
		// Update UI to show lock state
	}
}