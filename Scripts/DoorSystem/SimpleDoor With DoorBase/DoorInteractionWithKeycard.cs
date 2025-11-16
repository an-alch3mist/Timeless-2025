using UnityEngine;
using SPACE_UTIL;

namespace SPACE_GAME_1
{
	/// <summary>
	/// Enhanced door interaction that checks inventory for keycards.
	/// Replaces the basic DoorInteraction.cs
	/// </summary>
	public class DoorInteractionWithKeycard : MonoBehaviour
	{
		[Header("Door Reference")]
		[SerializeField] DoorBase _door;

		[Header("Input Keys")]
		[SerializeField] KeyCode _keyInteract = KeyCode.E;
		[SerializeField] KeyCode _keyLockToggle = KeyCode.L;

		[Header("Lock Side (for separate locks)")]
		[SerializeField] LockSide _lockSide = LockSide.Inside;

		[Header("Feedback")]
		[SerializeField] string _lockedMessage = "This door requires a keycard";
		[SerializeField] string _wrongKeycardMessage = "Wrong keycard - need: {0}";

		void Update()
		{
			HandleDoorInteraction();
			HandleLockInteraction();
		}

		void HandleDoorInteraction()
		{
			if (!INPUT.K.InstantDown(_keyInteract)) return;

			DoorActionResult result;

			if (_door.currDoorState == DoorState.Closed)
			{
				result = _door.TryOpen();

				// If locked and requires keycard, show specific message
				if (result == DoorActionResult.Locked && _door.requiresKeycard)
				{
					ShowFeedback(_lockedMessage);
				}

				LogResult("TryOpen", result);
			}
			else if (_door.currDoorState == DoorState.Opened)
			{
				result = _door.TryClose();
				LogResult("TryClose", result);
			}
		}

		void HandleLockInteraction()
		{
			if (!INPUT.K.InstantDown(_keyLockToggle)) return;

			// Determine target lock side
			LockSide targetSide = _door.usesCommonLock ? LockSide.Any : _lockSide;

			// Get current lock state
			DoorLockState currentLockState;
			if (_door.usesCommonLock || _lockSide == LockSide.Outside)
				currentLockState = _door.currOutsideLockState;
			else
				currentLockState = _door.currInsideLockState;

			DoorActionResult result;

			// Toggle lock
			if (currentLockState == DoorLockState.Unlocked)
			{
				result = _door.TryLock(targetSide);
				LogResult("TryLock", result);
			}
			else if (currentLockState == DoorLockState.Locked)
			{
				// Check if player has the required keycard
				string keycardNeeded = _door.keyId;
				bool hasKeycard = KeycardInventory.Instance?.HasKeycard(keycardNeeded) ?? false;

				if (_door.requiresKeycard && !hasKeycard && keycardNeeded != "")
				{
					ShowFeedback(string.Format(_wrongKeycardMessage, keycardNeeded));
					result = DoorActionResult.WrongKeyToUnlock;
				}
				else
				{
					// Player has keycard (or door doesn't require one)
					result = _door.TryUnlock(targetSide, keycardNeeded);
				}

				LogResult("TryUnlock", result);
			}
		}

		void ShowFeedback(string message)
		{
			// Replace with your UI system
			Debug.Log($"[Door Feedback] {message}".colorTag("yellow"));
		}

		void LogResult(string action, DoorActionResult result)
		{
			string color = result == DoorActionResult.Success ? "lime" : "yellow";
			Debug.Log($"[{action}] → {result}".colorTag(color));
		}
	}
}