using UnityEngine;
using UnityEngine.InputSystem;

using SPACE_UTIL;

namespace SPACE_GAME
{
	/// <summary>
	/// Handles player interaction with doors via raycast.
	/// Attach to player camera or FPS controller.
	/// Detects doors in crosshair, shows HUD, processes input.
	/// </summary>
	public class DoorInteractionController : MonoBehaviour
	{
		[Header("Raycast Settings")]
		[SerializeField] private float interactionDistance = 3f;
		[SerializeField] private LayerMask doorLayer = -1; // All layers
		[SerializeField] private Transform rayOrigin; // Camera transform

		[Header("Input")]
		[SerializeField] private KeyCode interactKey = KeyCode.E;

		[Header("Debug")]
		[SerializeField] private bool showDebugRay = true;

		// ===== PRIVATE FIELDS ===== //
		private IDoor currentDoor = null;
		private DoorHUDManager hudManager;

		// ===== UNITY LIFECYCLE ===== //
		private void Awake()
		{
			if (rayOrigin == null)
				rayOrigin = Camera.main != null ? Camera.main.transform : transform;
			hudManager = DoorHUDManager.Instance;
		}
		private void Update()
		{
			// Raycast for doors
			RaycastForDoor();

			// Handle input
			if (currentDoor != null && INPUT.K.InstantDown(interactKey))
				InteractWithDoor();
		}

		// ===== PRIVATE METHODS ===== //
		private void RaycastForDoor()
		{
			Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);

			// Debug visualization
			if (showDebugRay)
			{
				Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.yellow);
			}
			// Perform raycast
			if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, doorLayer))
			{
				// Check if hit object has IDoor component
				IDoor door = hit.collider.GetComponent<IDoor>();
				if (door == null)
					door = hit.collider.GetComponentInParent<IDoor>(); // Check parent

				if (door != null)
				{
					// New door detected
					if (currentDoor != door)
					{
						currentDoor = door;
						UpdateHUD();
					}
					return;
				}
			}
			// No door detected, clear current door
			if (currentDoor != null)
			{
				currentDoor = null;
				if (hudManager != null)
					hudManager.HidePrompt();
			}
		}

		private void UpdateHUD()
		{
			if (hudManager == null || currentDoor == null) return;

			// Show appropriate prompt based on door state
			switch (currentDoor.State)
			{
				case DoorState.Closed:
					if (currentDoor.IsLocked)
						hudManager.ShowLockedPrompt(currentDoor, "Key"); // TODO: Get key name from door
					else
						hudManager.ShowOpenPrompt(currentDoor);
					break;

				case DoorState.Opened:
					hudManager.ShowClosePrompt(currentDoor);
					break;

				case DoorState.Opening:
					hudManager.ShowOpeningPrompt();
					break;

				case DoorState.Closing:
					hudManager.ShowClosingPrompt();
					break;

				case DoorState.Locked:
					hudManager.ShowLockedPrompt(currentDoor, "Key");
					break;

				case DoorState.Swaying:
					hudManager.ShowPrompt(currentDoor, "E to Approach", PromptIcon.Warning);
					break;

				case DoorState.Blocked:
					hudManager.ShowPrompt(currentDoor, "Door Blocked", PromptIcon.Warning);
					break;
			}
		}

		private void InteractWithDoor()
		{
			if (currentDoor == null) return;

			Debug.Log(C.method(this, "cyan", $"Interacting with door. State={currentDoor.State}"));

			// Attempt to open or close based on current state
			switch (currentDoor.State)
			{
				case DoorState.Closed:
				case DoorState.Locked:
					currentDoor.TryOpen();
					break;

				case DoorState.Opened:
					currentDoor.TryClose();
					break;

				case DoorState.Swaying:
					// Stop swaying and open
					currentDoor.TryOpen();
					break;

				default:
					// Door is transitioning, ignore input
					Debug.Log(C.method(this, "yellow", "Door is busy, ignoring input"));
					break;
			}

			// Update HUD immediately
			UpdateHUD();
		}
	}
}