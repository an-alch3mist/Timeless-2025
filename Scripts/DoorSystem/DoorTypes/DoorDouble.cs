using System.Collections;
using UnityEngine;

using SPACE_UTIL;

namespace SPACE_GAME
{
	/// <summary>
	/// Double door controller that synchronizes two DoorHinged instances.
	/// Perfect for grand entrances, church doors, mansion doors.
	/// </summary>
	public class DoorDouble : DoorBase
	{
		[Header("Double Door Settings")]
		[SerializeField] private DoorHinged leftDoor;
		[SerializeField] private DoorHinged rightDoor;
		[SerializeField] private bool synchronizeState = true; // Keep both doors in sync

		// ===== UNITY LIFECYCLE ===== //

		protected override void Awake()
		{
			base.Awake();

			if (leftDoor == null || rightDoor == null)
			{
				Debug.Log(C.method(this, "red", "Left or Right door is null! Assign both hinged doors in Inspector."));
			}

			// Subscribe to child door events
			if (leftDoor != null)
			{
				leftDoor.OnStateChanged += OnChildDoorStateChanged;
			}
			if (rightDoor != null)
			{
				rightDoor.OnStateChanged += OnChildDoorStateChanged;
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			// Unsubscribe from child door events
			if (leftDoor != null)
			{
				leftDoor.OnStateChanged -= OnChildDoorStateChanged;
			}
			if (rightDoor != null)
			{
				rightDoor.OnStateChanged -= OnChildDoorStateChanged;
			}
		}

		// ===== ABSTRACT METHOD IMPLEMENTATION ===== //

		protected override IEnumerator PerformOpen()
		{
			// Open both doors simultaneously
			if (leftDoor != null) leftDoor.TryOpen();
			if (rightDoor != null) rightDoor.TryOpen();

			// Wait for both to finish (check their states)
			while ((leftDoor != null && leftDoor.State == DoorState.Opening) ||
				   (rightDoor != null && rightDoor.State == DoorState.Opening))
			{
				yield return null;
			}
		}

		protected override IEnumerator PerformClose()
		{
			// Close both doors simultaneously
			if (leftDoor != null) leftDoor.TryClose();
			if (rightDoor != null) rightDoor.TryClose();

			// Wait for both to finish
			while ((leftDoor != null && leftDoor.State == DoorState.Closing) ||
				   (rightDoor != null && rightDoor.State == DoorState.Closing))
			{
				yield return null;
			}
		}

		// ===== EVENT HANDLERS ===== //

		private void OnChildDoorStateChanged(DoorState newState)
		{
			if (!synchronizeState) return;

			// Keep parent state in sync with children
			// If both doors are opened, parent is opened
			// If either is opening, parent is opening

			if (leftDoor != null && rightDoor != null)
			{
				if (leftDoor.State == DoorState.Opened && rightDoor.State == DoorState.Opened)
				{
					SetState(DoorState.Opened);
				}
				else if (leftDoor.State == DoorState.Closed && rightDoor.State == DoorState.Closed)
				{
					SetState(DoorState.Closed);
				}
				else if (leftDoor.State == DoorState.Opening || rightDoor.State == DoorState.Opening)
				{
					SetState(DoorState.Opening);
				}
				else if (leftDoor.State == DoorState.Closing || rightDoor.State == DoorState.Closing)
				{
					SetState(DoorState.Closing);
				}
			}
		}

		// ===== PUBLIC API OVERRIDES ===== //

		public new bool TryUnlock(string keyId = null)
		{
			bool success = base.TryUnlock(keyId);
			if (success)
			{
				// Unlock both child doors
				if (leftDoor != null) leftDoor.TryUnlock(keyId);
				if (rightDoor != null) rightDoor.TryUnlock(keyId);
			}
			return success;
		}

		public new void Lock()
		{
			base.Lock();

			// Lock both child doors
			if (leftDoor != null) leftDoor.Lock();
			if (rightDoor != null) rightDoor.Lock();
		}
	}
}