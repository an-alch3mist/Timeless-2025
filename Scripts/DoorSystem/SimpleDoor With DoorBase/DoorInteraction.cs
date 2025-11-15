namespace SPACE_GAME_1
{
	using UnityEngine;
	using SPACE_UTIL;

	/// <summary>
	/// Trigger-based door interaction system.
	/// Detects which side of door player is on (inside/outside triggers).
	/// Press E to open/close, Press L to lock/unlock from correct side.
	/// </summary>
	public class DoorInteraction : MonoBehaviour
	{
		[Header("Door Reference")]
		[SerializeField] DoorBase _door;

		[Header("Trigger References - Assign from hierarchy")]
		[SerializeField] Collider _doorOutsideTrigger;
		[SerializeField] Collider _doorInsideTrigger;

		[Header("Input Keys")]
		[SerializeField] KeyCode _keyInteract = KeyCode.E;      // Open/Close
		[SerializeField] KeyCode _keyLockToggle = KeyCode.L;    // Lock/Unlock

		[Header("UI Feedback (Optional)")]
		[SerializeField] string _promptOpenDoor = "Press E to Open";
		[SerializeField] string _promptCloseDoor = "Press E to Close";
		[SerializeField] string _promptLockDoor = "Press L to Lock";
		[SerializeField] string _promptUnlockDoor = "Press L to Unlock";
		[SerializeField] string _promptNeedKeycard = "Need Keycard: {0}";

		// Runtime state
		private bool _playerInOutsideTrigger = false;
		private bool _playerInInsideTrigger = false;
		private GameObject _playerObject = null;

		void Start()
		{
			// Auto-find door if not assigned
			if (_door == null)
				_door = GetComponent<DoorBase>();

			// Auto-find triggers if not assigned
			if (_doorOutsideTrigger == null)
				_doorOutsideTrigger = transform.Find("trigger/door outside trigger")?.GetComponent<Collider>();
			if (_doorInsideTrigger == null)
				_doorInsideTrigger = transform.Find("trigger/door inside trigger")?.GetComponent<Collider>();

			// Setup triggers
			if (_doorOutsideTrigger != null)
			{
				_doorOutsideTrigger.isTrigger = true;
				var triggerHandler = _doorOutsideTrigger.gameObject.AddComponent<DoorTriggerHandler>();
				triggerHandler.Initialize(this, TriggerSide.Outside);
			}

			if (_doorInsideTrigger != null)
			{
				_doorInsideTrigger.isTrigger = true;
				var triggerHandler = _doorInsideTrigger.gameObject.AddComponent<DoorTriggerHandler>();
				triggerHandler.Initialize(this, TriggerSide.Inside);
			}
		}

		void Update()
		{
			// Only process input if player is near door
			if (!IsPlayerNearDoor()) return;

			// Handle door interaction (E key)
			if (INPUT.K.InstantDown(_keyInteract))
			{
				HandleDoorInteraction();
			}

			// Handle lock interaction (L key)
			if (INPUT.K.InstantDown(_keyLockToggle))
			{
				HandleLockInteraction();
			}
		}

		// ========================================================================
		// CORE INTERACTION LOGIC
		// ========================================================================

		void HandleDoorInteraction()
		{
			DoorActionResult result;

			// Smart toggle: Open if closed, Close if opened
			if (_door.currDoorState == DoorState.Closed)
			{
				result = _door.TryOpen();
				LogResult("TryOpen", result);

				// Show feedback
				if (result == DoorActionResult.Locked)
					ShowFeedback("Door is locked!");
				else if (result == DoorActionResult.Blocked)
					ShowFeedback("Something is blocking the door...");
			}
			else if (_door.currDoorState == DoorState.Opened)
			{
				result = _door.TryClose();
				LogResult("TryClose", result);
			}
			else
			{
				// Door is animating
				ShowFeedback("Wait for door to finish moving...");
			}
		}

		void HandleLockInteraction()
		{
			// Determine which side player is on
			LockSide playerSide = GetPlayerSide();

			// If common lock, always use LockSide.Any
			if (_door.usesCommonLock)
				playerSide = LockSide.Any;

			// Get current lock state for this side
			DoorLockState currentLockState = GetLockStateForSide(playerSide);

			// Check if player has required keycard
			if (_door.requiresKeycard && _door.keyId != "")
			{
				bool hasKeycard = KeycardInventory.Instance?.HasKeycard(_door.keyId) ?? false;

				if (!hasKeycard)
				{
					ShowFeedback(string.Format(_promptNeedKeycard, _door.keyId));
					return;
				}
			}

			DoorActionResult result;

			// Smart toggle: Lock if unlocked, Unlock if locked
			if (currentLockState == DoorLockState.Unlocked)
			{
				result = _door.TryLock(playerSide);
				LogResult($"TryLock({playerSide})", result);

				if (result == DoorActionResult.UnlockedJam)
					ShowFeedback("Lock is jammed!");
			}
			else if (currentLockState == DoorLockState.Locked)
			{
				result = _door.TryUnlock(playerSide, _door.keyId);
				LogResult($"TryUnlock({playerSide})", result);

				if (result == DoorActionResult.WrongKeyToUnlock)
					ShowFeedback($"Wrong keycard! Need: {_door.keyId}");
			}
			else
			{
				// Lock is animating
				ShowFeedback("Wait for lock mechanism to finish...");
			}
		}

		// ========================================================================
		// TRIGGER CALLBACKS (called from DoorTriggerHandler)
		// ========================================================================

		public void OnPlayerEnterTrigger(TriggerSide side, GameObject player)
		{
			_playerObject = player;

			if (side == TriggerSide.Outside)
			{
				_playerInOutsideTrigger = true;
				Debug.Log("[Door] Player entered OUTSIDE trigger".colorTag("cyan"));
			}
			else if (side == TriggerSide.Inside)
			{
				_playerInInsideTrigger = true;
				Debug.Log("[Door] Player entered INSIDE trigger".colorTag("cyan"));
			}

			// Show UI prompt
			UpdateUIPrompt();
		}

		public void OnPlayerExitTrigger(TriggerSide side, GameObject player)
		{
			if (side == TriggerSide.Outside)
			{
				_playerInOutsideTrigger = false;
				Debug.Log("[Door] Player exited OUTSIDE trigger".colorTag("grey"));
			}
			else if (side == TriggerSide.Inside)
			{
				_playerInInsideTrigger = false;
				Debug.Log("[Door] Player exited INSIDE trigger".colorTag("grey"));
			}

			// Hide UI prompt if player left both triggers
			if (!IsPlayerNearDoor())
			{
				_playerObject = null;
				HideUIPrompt();
			}
			else
			{
				UpdateUIPrompt();
			}
		}

		// ========================================================================
		// HELPER METHODS
		// ========================================================================

		bool IsPlayerNearDoor()
		{
			return _playerInOutsideTrigger || _playerInInsideTrigger;
		}

		LockSide GetPlayerSide()
		{
			if (_playerInOutsideTrigger && !_playerInInsideTrigger)
				return LockSide.Outside;
			else if (_playerInInsideTrigger && !_playerInOutsideTrigger)
				return LockSide.Inside;
			else if (_playerInOutsideTrigger && _playerInInsideTrigger)
				return LockSide.Outside; // Default to outside if in both (shouldn't happen)
			else
				return LockSide.Outside; // Fallback
		}

		DoorLockState GetLockStateForSide(LockSide side)
		{
			if (_door.usesCommonLock || side == LockSide.Outside)
				return _door.currOutsideLockState;
			else
				return _door.currInsideLockState;
		}

		void UpdateUIPrompt()
		{
			if (!IsPlayerNearDoor()) return;

			string prompt = "";

			// Door open/close prompt
			if (_door.currDoorState == DoorState.Closed)
				prompt = _promptOpenDoor;
			else if (_door.currDoorState == DoorState.Opened)
				prompt = _promptCloseDoor;

			// Lock prompt
			LockSide side = GetPlayerSide();
			DoorLockState lockState = GetLockStateForSide(side);

			if (lockState == DoorLockState.Unlocked)
				prompt += $"\n{_promptLockDoor}";
			else if (lockState == DoorLockState.Locked)
				prompt += $"\n{_promptUnlockDoor}";

			ShowPrompt(prompt);
		}

		void ShowPrompt(string message)
		{
			// Replace with your UI system
			Debug.Log($"[UI Prompt] {message}".colorTag("white"));
		}

		void HideUIPrompt()
		{
			// Replace with your UI system
			Debug.Log("[UI Prompt] Hidden".colorTag("grey"));
		}

		void ShowFeedback(string message)
		{
			// Replace with your UI system (pop-up message)
			Debug.Log($"[Door Feedback] {message}".colorTag("yellow"));
		}

		void LogResult(string action, DoorActionResult result)
		{
			string color = result == DoorActionResult.Success ? "lime" : "yellow";
			Debug.Log($"[{action}] → {result}".colorTag(color));
		}

		// ========================================================================
		// GIZMO VISUALIZATION
		// ========================================================================

		void OnDrawGizmosSelected()
		{
			if (_doorOutsideTrigger != null)
			{
				Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // Red = Outside
				Gizmos.matrix = _doorOutsideTrigger.transform.localToWorldMatrix;
				if (_doorOutsideTrigger is BoxCollider box)
					Gizmos.DrawCube(box.center, box.size);
			}

			if (_doorInsideTrigger != null)
			{
				Gizmos.color = new Color(0f, 1f, 0f, 0.3f); // Green = Inside
				Gizmos.matrix = _doorInsideTrigger.transform.localToWorldMatrix;
				if (_doorInsideTrigger is BoxCollider box)
					Gizmos.DrawCube(box.center, box.size);
			}
		}
	}

	// ============================================================================
	// HELPER COMPONENT - Handles trigger enter/exit events
	// ============================================================================

	public enum TriggerSide
	{
		Outside,
		Inside
	}

	public class DoorTriggerHandler : MonoBehaviour
	{
		private DoorInteraction _doorInteraction;
		private TriggerSide _side;

		public void Initialize(DoorInteraction doorInteraction, TriggerSide side)
		{
			_doorInteraction = doorInteraction;
			_side = side;
		}

		void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				_doorInteraction?.OnPlayerEnterTrigger(_side, other.gameObject);
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				_doorInteraction?.OnPlayerExitTrigger(_side, other.gameObject);
			}
		}
	}

	// ============================================================================
	// SETUP INSTRUCTIONS
	// ============================================================================
	/*
	 * HIERARCHY SETUP (matches your provided structure):
	 * ---------------------------------------------------
	 * 
	 * ./doorHingedSimple/
	 * ├─ Components:
	 * │  ├─ SimpleDoorHinged (DoorBase)
	 * │  ├─ SimpleDoorAnimEventForwarder
	 * │  ├─ DoorInteraction (THIS SCRIPT) ← Add this!
	 * │  └─ Animator
	 * │
	 * ├─ trigger/
	 * │  ├─ door outside trigger (BoxCollider, isTrigger=true)
	 * │  │  └─ DoorTriggerHandler (auto-added by script)
	 * │  └─ door inside trigger (BoxCollider, isTrigger=true)
	 * │     └─ DoorTriggerHandler (auto-added by script)
	 * │
	 * ├─ door/
	 * │  ├─ frame (MeshRenderer, BoxCollider)
	 * │  ├─ handleOutside (MeshRenderer, BoxCollider)
	 * │  └─ handleInside (MeshRenderer, BoxCollider)
	 * │
	 * └─ doorFrame/
	 *    ├─ visual frame left (MeshRenderer, BoxCollider)
	 *    └─ visual frame right (MeshRenderer, BoxCollider)
	 * 
	 * 
	 * INSPECTOR CONFIGURATION:
	 * ------------------------
	 * DoorInteraction component:
	 * 
	 * [DOOR REFERENCE]
	 * └─ Door: (auto-found if on same GameObject)
	 * 
	 * [TRIGGER REFERENCES]
	 * ├─ Door Outside Trigger: trigger/door outside trigger
	 * └─ Door Inside Trigger: trigger/door inside trigger
	 * 
	 * [INPUT KEYS]
	 * ├─ Key Interact: E
	 * └─ Key Lock Toggle: L
	 * 
	 * [UI FEEDBACK]
	 * ├─ Prompt Open Door: "Press E to Open"
	 * ├─ Prompt Close Door: "Press E to Close"
	 * ├─ Prompt Lock Door: "Press L to Lock"
	 * ├─ Prompt Unlock Door: "Press L to Unlock"
	 * └─ Prompt Need Keycard: "Need Keycard: {0}"
	 * 
	 * 
	 * TRIGGER SETUP:
	 * --------------
	 * Both triggers should be BoxColliders with:
	 * ├─ Is Trigger: ✅ true
	 * ├─ Size: Adjust to cover interaction area
	 * └─ Center: Position relative to door
	 * 
	 * Outside trigger (red in gizmo):
	 * ├─ Center: (0, 0, -1) or adjust to outside area
	 * └─ Size: (2, 2, 0.5)
	 * 
	 * Inside trigger (green in gizmo):
	 * ├─ Center: (0, 0, 1) or adjust to inside area
	 * └─ Size: (2, 2, 0.5)
	 * 
	 * 
	 * PLAYER SETUP:
	 * -------------
	 * ⚠️ IMPORTANT: Player GameObject MUST have tag "Player"
	 * 
	 * 1. Select player GameObject
	 * 2. Inspector → Tag → Player
	 * 3. Player needs Collider (not trigger) for detection
	 * 
	 * 
	 * HOW IT WORKS:
	 * -------------
	 * 1. Player enters "door outside trigger"
	 *    → UI shows: "Press E to Open / Press L to Unlock"
	 *    → Press E → TryOpen() from outside
	 *    → Press L → TryUnlock(LockSide.Outside)
	 * 
	 * 2. Player enters "door inside trigger"
	 *    → UI shows: "Press E to Open / Press L to Lock"
	 *    → Press E → TryOpen() from inside
	 *    → Press L → TryLock(LockSide.Inside)
	 * 
	 * 3. If door uses common lock:
	 *    → Press L → Always uses LockSide.Any
	 *    → Both triggers work the same way
	 * 
	 * 4. If door requires keycard:
	 *    → Checks KeycardInventory.Instance for keyId
	 *    → Shows "Need Keycard: office_keycard" if missing
	 * 
	 * 
	 * DEBUGGING:
	 * ----------
	 * - Gizmos show trigger areas (Red=Outside, Green=Inside)
	 * - Console logs show trigger enter/exit events
	 * - Console logs show all interaction results
	 * - Check player has "Player" tag!
	 * - Check triggers have "Is Trigger" enabled!
	 */
}