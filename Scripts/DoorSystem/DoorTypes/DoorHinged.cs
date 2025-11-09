using System.Collections;
using UnityEngine;

using SPACE_UTIL;

namespace SPACE_GAME
{
	/// <summary>
	/// Hinged door that rotates around a pivot point.
	/// Supports inward/outward swing based on player position.
	/// </summary>
	public class DoorHinged : DoorBase
	{
		[Header("Hinged Door Settings")]
		[SerializeField] private Transform rotationPivot; // Child object at hinge edge
		[SerializeField] private float openAngle = 90f; // Degrees to rotate
		[SerializeField] private bool autoDetectSwingDirection = true;
		[SerializeField] private float openSpeed = 2f; // Rotation speed multiplier

		private float targetAngle = 0f; // 0 = closed, openAngle = open
		private Vector3 closedRotation;

		// ===== UNITY LIFECYCLE ===== //

		protected override void Awake()
		{
			base.Awake();

			if (rotationPivot == null)
			{
				Debug.Log(C.method(this, "red", "rotationPivot is null! Assign in Inspector or create pivot child."));
				rotationPivot = transform; // Fallback to self
			}

			closedRotation = rotationPivot.localEulerAngles;
		}

		// ===== ABSTRACT METHOD IMPLEMENTATION ===== //

		protected override IEnumerator PerformOpen()
		{
			// Determine swing direction if auto-detect enabled
			if (autoDetectSwingDirection)
			{
				DetectSwingDirection();
			}

			targetAngle = openAngle;

			// Smoothly rotate to open position
			float elapsedTime = 0f;
			float duration = config != null ? config.openDuration : 1f;

			Vector3 startRot = rotationPivot.localEulerAngles;
			Vector3 endRot = closedRotation + new Vector3(0f, targetAngle, 0f);

			while (elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime * openSpeed;
				float t = Mathf.Clamp01(elapsedTime / duration);

				// Ease out for smooth deceleration
				t = 1f - Mathf.Pow(1f - t, 3f);

				rotationPivot.localEulerAngles = Vector3.Lerp(startRot, endRot, t);
				yield return null;
			}

			// Ensure final position is exact
			rotationPivot.localEulerAngles = endRot;
		}

		protected override IEnumerator PerformClose()
		{
			targetAngle = 0f;

			// Smoothly rotate to closed position
			float elapsedTime = 0f;
			float duration = config != null ? config.closeDuration : 1f;

			Vector3 startRot = rotationPivot.localEulerAngles;
			Vector3 endRot = closedRotation;

			while (elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime * openSpeed;
				float t = Mathf.Clamp01(elapsedTime / duration);

				// Ease in for smooth acceleration
				t = Mathf.Pow(t, 3f);

				rotationPivot.localEulerAngles = Vector3.Lerp(startRot, endRot, t);
				yield return null;
			}

			// Ensure final position is exact
			rotationPivot.localEulerAngles = endRot;
		}

		// ===== PRIVATE HELPERS ===== //
		private void DetectSwingDirection()
		{
			// Find player position relative to door
			Transform player = Camera.main != null ? Camera.main.transform : null;
			if (player == null)
			{
				Debug.Log(C.method(this, "yellow", "No main camera found, using default swing direction"));
				return;
			}
			// Calculate which side of door plane player is on
			Vector3 doorForward = transform.forward;
			Vector3 toPlayer = (player.position - transform.position).normalized;

			float dot = Vector3.Dot(doorForward, toPlayer);

			// If player is behind door, swing inward (positive angle)
			// If player is in front, swing outward (negative angle)
			if (dot < 0f)
			{
				openAngle = Mathf.Abs(openAngle); // Swing inward
			}
			else
			{
				openAngle = -Mathf.Abs(openAngle); // Swing outward
			}
			Debug.Log(C.method(this, "cyan", $"Swing direction: {(openAngle > 0 ? "inward" : "outward")}"));
		}
	}
}