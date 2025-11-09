using UnityEngine;
using SPACE_UTIL;

namespace SPACE_GAME
{
	/// <summary>
	/// Optional component for horror-style door swaying.
	/// Attach to any DoorBase to enable creepy idle animations.
	/// Triggers when player is nearby but not looking directly at door.
	/// </summary>
	[RequireComponent(typeof(DoorBase))]
	public class DoorSwayBehavior : MonoBehaviour
	{
		[Header("Sway Trigger Conditions")]
		[SerializeField] private float triggerDistanceMin = 5f;
		[SerializeField] private float triggerDistanceMax = 10f;
		[SerializeField] private float gazeAngleThreshold = 30f; // Degrees from look direction
		[SerializeField] private float swayChancePerSecond = 0.1f; // 10% chance per second

		[Header("Sway Animation")]
		[SerializeField] private AudioClip creakSound;
		[SerializeField] private float creakVolume = 0.5f;
		[SerializeField] private bool useAnimator = true; // Use Animator.SetBool("doorSwaying")

		[Header("Debug")]
		[SerializeField] private bool showDebugGizmos = true;

		// ===== PRIVATE FIELDS ===== //
		private DoorBase door;
		private Animator animator;
		private bool isSwaying = false;
		private AudioSource swayAudioSource;

		// ===== UNITY LIFECYCLE ===== //

		private void Awake()
		{
			door = GetComponent<DoorBase>();
			animator = GetComponent<Animator>();

			// Create AudioSource for looping creak sound
			swayAudioSource = gameObject.AddComponent<AudioSource>();
			swayAudioSource.clip = creakSound;
			swayAudioSource.loop = true;
			swayAudioSource.volume = creakVolume;
			swayAudioSource.spatialBlend = 1f; // 3D sound
			swayAudioSource.playOnAwake = false;
		}

		private void Update()
		{
			// Only sway when door is closed
			if (door.State != DoorState.Closed && door.State != DoorState.Locked)
			{
				StopSwaying();
				return;
			}

			// Check trigger conditions
			if (ShouldTriggerSway())
			{
				StartSwaying();
			}
			else if (PlayerLookingAtDoor() || !PlayerInRange())
			{
				StopSwaying();
			}
		}

		// ===== PRIVATE METHODS ===== //

		private bool ShouldTriggerSway()
		{
			if (isSwaying) return false; // Already swaying
			if (!PlayerInRange()) return false;
			if (PlayerLookingAtDoor()) return false;

			// Random chance per frame
			float chance = swayChancePerSecond * Time.deltaTime;
			return Random.value < chance;
		}

		private bool PlayerInRange()
		{
			Transform player = Camera.main != null ? Camera.main.transform : null;
			if (player == null) return false;

			float distance = Vector3.Distance(player.position, transform.position);
			return distance >= triggerDistanceMin && distance <= triggerDistanceMax;
		}

		private bool PlayerLookingAtDoor()
		{
			Transform player = Camera.main != null ? Camera.main.transform : null;
			if (player == null) return false;

			Vector3 toDoor = (transform.position - player.position).normalized;
			float angle = Vector3.Angle(player.forward, toDoor);

			return angle < gazeAngleThreshold;
		}

		private void StartSwaying()
		{
			if (isSwaying) return;

			isSwaying = true;
			Debug.Log(C.method(this, "cyan", "Door started swaying"));

			// Trigger animator parameter
			if (useAnimator && animator != null)
			{
				animator.trySetBool(AnimParamType.doorSwaying, true);
			}

			// Play looping creak sound
			if (swayAudioSource != null && creakSound != null)
			{
				swayAudioSource.Play();
			}
		}

		private void StopSwaying()
		{
			if (!isSwaying) return;

			isSwaying = false;
			Debug.Log(C.method(this, "cyan", "Door stopped swaying"));

			// Stop animator parameter
			if (useAnimator && animator != null)
			{
				animator.trySetBool(AnimParamType.doorSwaying, false);
			}

			// Stop looping sound
			if (swayAudioSource != null)
			{
				swayAudioSource.Stop();
			}
		}

		// ===== GIZMOS (EDITOR ONLY) ===== //

		private void OnDrawGizmosSelected()
		{
			if (!showDebugGizmos) return;

			// Draw trigger distance range
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, triggerDistanceMin);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, triggerDistanceMax);

			// Draw gaze cone from player
			Transform player = Camera.main != null ? Camera.main.transform : null;
			if (player != null)
			{
				Gizmos.color = PlayerLookingAtDoor() ? Color.green : Color.gray;
				Gizmos.DrawLine(player.position, transform.position);
			}
		}
	}
}