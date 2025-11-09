using UnityEngine;

namespace SPACE_GAME
{
	/// <summary>
	/// ScriptableObject for data-driven door configuration.
	/// Create instances via: Create > Door System > Door Config
	/// Assign to DoorBase.config field in Inspector.
	/// </summary>
	[CreateAssetMenu(fileName = "DoorConfig", menuName = "Door System/Door Config", order = 1)]
	public class DoorConfigSO : ScriptableObject
	{
		[Header("Animation Durations")]
		[Tooltip("Time for door to fully open (seconds)")]
		public float openDuration = 1.0f;

		[Tooltip("Time for door to fully close (seconds)")]
		public float closeDuration = 1.0f;

		[Header("Audio Clips")]
		[Tooltip("Played when door starts opening")]
		public AudioClip openSound;

		[Tooltip("Played when door starts closing")]
		public AudioClip closeSound;

		[Tooltip("Played when door is unlocked")]
		public AudioClip unlockSound;

		[Tooltip("Played when player tries to open locked door")]
		public AudioClip lockedSound;

		[Tooltip("Looping sound for swaying doors (horror)")]
		public AudioClip swaySound;

		[Header("Audio Settings")]
		[Range(0f, 1f)]
		[Tooltip("Master volume for all door sounds")]
		public float volume = 0.7f;

		[Header("Physics")]
		[Tooltip("Layers that block door closing (e.g., Player, Rigidbody)")]
		public LayerMask obstructionLayers = -1; // All layers by default

		[Header("Interaction")]
		[Tooltip("Max distance player can interact with door (meters)")]
		public float interactionDistance = 3f;

		[Tooltip("Key item ID required to unlock (empty = no key needed)")]
		public string requiredKeyId = "";

		[Header("Visual Feedback")]
		[Tooltip("Color tint for locked door highlight")]
		public Color lockedTintColor = new Color(1f, 0.3f, 0.3f); // Red tint

		[Tooltip("Color tint for unlocked door highlight")]
		public Color unlockedTintColor = new Color(0.3f, 1f, 0.3f); // Green tint
	}
}