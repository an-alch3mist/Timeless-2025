using UnityEngine;

using SPACE_UTIL;

namespace SPACE_GAME
{
	/// <summary>
	/// Interaction zone for door sides (inside/outside).
	/// Attach to interactionTriggerInside and interactionTriggerOutside GameObjects.
	/// Requires: BoxCollider/SphereCollider with isTrigger=true
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class DoorInteractionZone : MonoBehaviour
	{
		// ====================================================================
		// SERIALIZED FIELDS
		// ====================================================================

		[Header("Zone Configuration")]
		[SerializeField] DoorSide zoneSide = DoorSide.inside;

		[Header("References")]
		[Tooltip("Leave empty to auto-find parent Door component")]
		[SerializeField] Door targetDoor;

		[Header("Player Detection")]
		[SerializeField] string playerTag = "Player";

		[Header("Debug")]
		[SerializeField] bool showDebugLogs = false;
		[SerializeField] bool drawZoneGizmos = true;

		// ====================================================================
		// PRIVATE FIELDS
		// ====================================================================

		private Collider triggerCollider;
		private bool playerInZone = false;

		// ====================================================================
		// UNITY LIFECYCLE
		// ====================================================================

		void Awake()
		{
			if (showDebugLogs)
				Debug.Log(C.method(this, "white"));

			triggerCollider = this.GetComponent<Collider>();

			if (!triggerCollider.isTrigger)
			{
				Debug.Log(C.method(this, "yellow", adMssg: "Collider should be trigger! Fixing..."));
				triggerCollider.isTrigger = true;
			}

			// Auto-find parent Door if not assigned
			if (targetDoor == null)
			{
				targetDoor = GetComponentInParent<Door>();

				if (targetDoor == null)
				{
					Debug.Log(C.method(this, "red", adMssg: "No Door component found in parent!"));
				}
			}
		}

		void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag(playerTag))
			{
				playerInZone = true;

				if (targetDoor != null)
				{
					targetDoor.SetPlayerSide(zoneSide);
				}

				if (showDebugLogs)
				{
					Debug.Log(C.method(this, "cyan", adMssg: $"player entered {zoneSide} zone"));
				}
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.CompareTag(playerTag))
			{
				playerInZone = false;

				if (targetDoor != null)
				{
					targetDoor.SetPlayerSide(null);
				}

				if (showDebugLogs)
				{
					Debug.Log(C.method(this, "cyan", adMssg: $"player exited {zoneSide} zone"));
				}
			}
		}

		void OnDrawGizmos()
		{
			if (!drawZoneGizmos) return;

			Collider col = GetComponent<Collider>();
			if (col == null) return;

			// Color based on side
			Gizmos.color = zoneSide == DoorSide.inside ?
				new Color(0f, 1f, 0f, 0.3f) :  // Green for inside
				new Color(1f, 0f, 0f, 0.3f);   // Red for outside

			if (col is BoxCollider box)
			{
				Gizmos.matrix = transform.localToWorldMatrix;
				Gizmos.DrawCube(box.center, box.size);
			}
			else if (col is SphereCollider sphere)
			{
				Gizmos.DrawSphere(transform.position + sphere.center, sphere.radius);
			}
			else if (col is CapsuleCollider capsule)
			{
				// Simple sphere approximation for gizmo
				Gizmos.DrawSphere(transform.position + capsule.center, capsule.radius);
			}
		}

		// ====================================================================
		// PUBLIC API
		// ====================================================================

		/// <summary>
		/// Check if player is currently in this zone
		/// </summary>
		public bool IsPlayerInZone()
		{
			return playerInZone;
		}

		/// <summary>
		/// Get the side this zone represents
		/// </summary>
		public DoorSide GetZoneSide()
		{
			return zoneSide;
		}
	}
}