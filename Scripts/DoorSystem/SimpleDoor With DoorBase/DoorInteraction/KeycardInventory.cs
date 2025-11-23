namespace SPACE_GAME_1
{
	using UnityEngine;
	using System.Collections.Generic;
	using SPACE_UTIL;

	// ============================================================================
	// SIMPLE INVENTORY SYSTEM FOR KEYS/KEYCARDS
	// ============================================================================

	/// <summary>
	/// Manages player's collected keys and keycards.
	/// Singleton pattern for easy global access.
	/// </summary>
	public class KeycardInventory : MonoBehaviour
	{
		public static KeycardInventory Instance { get; private set; }

		[Header("Current Inventory")]
		[SerializeField] List<string> _keycards = new List<string>();

		void Awake()
		{
			// Singleton pattern
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

		// ========================================================================
		// PUBLIC API
		// ========================================================================

		/// <summary>
		/// Add a key/keycard to inventory
		/// </summary>
		public void AddKeycard(string keycardId)
		{
			if (!_keycards.Contains(keycardId))
			{
				_keycards.Add(keycardId);
				Debug.Log($"[Inventory] Acquired: {keycardId}".colorTag("lime"));
			}
			else
			{
				Debug.Log($"[Inventory] Already have: {keycardId}".colorTag("yellow"));
			}
		}

		/// <summary>
		/// Check if player has a specific key/keycard
		/// </summary>
		public bool HasKeycard(string keycardId)
		{
			return _keycards.Contains(keycardId);
		}

		/// <summary>
		/// Remove a key/keycard (for consumable keys)
		/// </summary>
		public void RemoveKeycard(string keycardId)
		{
			if (_keycards.Remove(keycardId))
				Debug.Log($"[Inventory] Used/Lost: {keycardId}".colorTag("grey"));
		}

		/// <summary>
		/// Get all keycards (for UI display)
		/// </summary>
		public List<string> GetAllKeycards() => new List<string>(_keycards);
	}

	// ============================================================================
	// KEYCARD PICKUP ITEM
	// ============================================================================

	/// <summary>
	/// Collectible keycard item in the world.
	/// Attach to GameObject with collider (trigger enabled).
	/// </summary>
	public class KeycardPickup : MonoBehaviour
	{
		[Header("Keycard Configuration")]
		[SerializeField] string _keycardId = "office_keycard";
		[SerializeField] string _displayName = "Office Keycard";
		[Tooltip("If true, keycard is removed from world after pickup")]
		[SerializeField] bool _destroyOnPickup = true;

		void OnTriggerEnter(Collider other)
		{
			// Check if player picked it up
			if (other.CompareTag("Player"))
			{
				// Add to inventory
				KeycardInventory.Instance?.AddKeycard(_keycardId);

				// Show pickup message (you'd use your UI system here)
				Debug.Log($"Picked up: {_displayName}".colorTag("lime"));

				// Remove from world
				if (_destroyOnPickup)
					Destroy(gameObject);
				else
					gameObject.SetActive(false); // Or disable pickup script
			}
		}
	}

	// ============================================================================
	// SMART DOOR INTERACTION WITH KEYCARD SUPPORT
	// ============================================================================

	// ============================================================================
	// USAGE EXAMPLES
	// ============================================================================
	/*
	 * 
	 * SCENARIO 1: Basic Key Door
	 * --------------------------
	 * 1. Door GameObject → Add SimpleDoorHinged component
	 * 2. Inspector settings:
	 *    - Key ID: "rusty_key"
	 *    - Requires Keycard: false (it's a physical key, not electronic)
	 * 3. Create key pickup GameObject
	 *    - Add KeycardPickup component
	 *    - Keycard ID: "rusty_key"
	 *    - Display Name: "Rusty Iron Key"
	 * 
	 * 
	 * SCENARIO 2: Electronic Keycard Door
	 * ------------------------------------
	 * 1. Door GameObject → Add SimpleDoorHinged component
	 * 2. Inspector settings:
	 *    - Key ID: "security_level_3"
	 *    - Requires Keycard: true
	 *    - Uses Common Lock: true
	 * 3. Create keycard pickup
	 *    - Keycard ID: "security_level_3"
	 *    - Display Name: "Security Level 3 Access Card"
	 * 
	 * 
	 * SCENARIO 3: Auto-Locking Security Door
	 * ---------------------------------------
	 * 1. Door settings:
	 *    - Key ID: "lab_keycard"
	 *    - Requires Keycard: true
	 *    - Auto Lock Delay: 5.0 (locks after 5 seconds)
	 * 2. Player unlocks → door stays unlocked for 5s → auto-locks
	 * 
	 * 
	 * SCENARIO 4: Master Key System
	 * ------------------------------
	 * // In your custom door script:
	 * public override DoorActionResult TryUnlock(LockSide side, string unlockKey)
	 * {
	 *     // Check for master key first
	 *     if (KeycardInventory.Instance?.HasKeycard("master_key") ?? false)
	 *     {
	 *         unlockKey = keyId; // Master key bypasses check
	 *     }
	 *     
	 *     return base.TryUnlock(side, unlockKey);
	 * }
	 * 
	 */
}