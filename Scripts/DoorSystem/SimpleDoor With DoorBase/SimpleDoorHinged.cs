namespace SPACE_GAME_1
{
	using UnityEngine;
	using SPACE_UTIL;

	/// <summary>
	/// Simple hinged door implementation.
	/// Inherits all core functionality from DoorBase.
	/// This is a COMPLETE, PRODUCTION-READY script - just attach and configure!
	/// </summary>
	public class SimpleDoorHinged : DoorBase
	{
		[Header("Hinged Door Specific Settings")]
		[SerializeField] float _swingAngle = 90f;
		[SerializeField] bool _hingeOnRight = true;
		[Tooltip("Play creak sound when door is old/damaged")]
		[SerializeField] bool _playCreakSound = false;

		// That's it! All door logic is inherited from DoorBase.
		// The script works immediately with zero additional code needed.

		// ========================================================================
		// OPTIONAL: Override methods for custom behavior
		// ========================================================================

		/*
		// Example 1: Custom sound on open
		public override DoorActionResult TryOpen()
		{
			var result = base.TryOpen();

			if (result == DoorActionResult.Success && _playCreakSound)
			{
				// Play creaky door sound
				AudioSource.PlayClipAtPoint(creakClip, transform.position);
			}

			return result;
		}
		*/

		/*
		// Example 2: Custom animation callback
		public override void OnAnimationComplete(AnimationEventType eventType)
		{
			base.OnAnimationComplete(eventType);

			if (eventType == AnimationEventType.DoorOpeningComplete)
			{
				// Spawn dust particles, shake camera, etc.
			}
		}
		*/

		/*
		// Example 3: Prevent opening if player too close to hinge side
		public override DoorActionResult TryOpen()
		{
			if (IsPlayerBlockingHinge())
			{
				Debug.Log("Step back from the hinge!".colorTag("yellow"));
				return DoorActionResult.ObstructionDetected;
			}

			return base.TryOpen();
		}

		bool IsPlayerBlockingHinge()
		{
			// Your collision check logic here
			return false;
		}
		*/
	}

	// ============================================================================
	// SETUP INSTRUCTIONS - Copy to scene
	// ============================================================================
	/*
	 * STEP 1: GameObject Setup
	 * -------------------------
	 * ./doorHingedSimple/                    ← Root GameObject
	 * ├─ Components:
	 * │  ├─ SimpleDoorHinged (this script)
	 * │  ├─ DoorAnimEventForwarder
	 * │  └─ Animator
	 * ├─ trigger/
	 * │  ├─ door outside trigger (BoxCollider)
	 * │  └─ door inside trigger (BoxCollider)
	 * ├─ door/
	 * │  ├─ frame (MeshRenderer, BoxCollider)
	 * │  ├─ handleOutside (MeshRenderer, BoxCollider)
	 * │  └─ handleInside (MeshRenderer, BoxCollider)
	 * └─ doorFrame/
	 *    ├─ visual frame left
	 *    └─ visual frame right
	 * 
	 * 
	 * STEP 2: Inspector Configuration
	 * --------------------------------
	 * SimpleDoorHinged component:
	 * 
	 * [CONFIGURATION]
	 * ├─ Animator: (assign your animator)
	 * ├─ Key Id: "office_key"              ← Leave empty for no key
	 * ├─ Requires Keycard: false           ← true for electronic locks
	 * ├─ Uses Common Lock: false           ← true for single lock (gate/keypad)
	 * ├─ Can Be Locked: true               ← false for doors that can't lock
	 * ├─ Max Closing Retries: 5
	 * └─ Auto Lock Delay: 0                ← Set to 5.0 for 5-second auto-lock
	 * 
	 * [INITIAL STATE]
	 * ├─ Init Is Blocked: false
	 * ├─ Init Door State: Closed           ← Or Opened
	 * ├─ Init Inside Lock State: Unlocked  ← Or Locked/UnlockedJam
	 * └─ Init Outside Lock State: Unlocked
	 * 
	 * [HINGED DOOR SPECIFIC]
	 * ├─ Swing Angle: 90
	 * ├─ Hinge On Right: true
	 * └─ Play Creak Sound: false
	 * 
	 * 
	 * STEP 3: Animator Controller
	 * ----------------------------
	 * Use: doorOpenCloseAnimController_stateMachineApproach
	 * 
	 * Parameters needed (auto-detected by trySetBool/trySetTrigger):
	 * ├─ Triggers: doorOpen, doorClose, doorLockedJiggle, etc.
	 * └─ Bools: isDoorOpen, isInsideLocked, isOutsideLocked
	 * 
	 * [AUDIO CLIPS]
	 * ├─ Door Open Sound: doorSwayOpenClose (trim first 0.5s or use full)
	 * ├─ Door Close Sound: doorSlam
	 * ├─ Door Lock Sound: doorLock
	 * ├─ Door Unlock Sound: doorUnlock
	 * ├─ Door Locked Jiggle Sound: doorLockedJiggle
	 * ├─ Door Knock Sound: doorKnocks (optional - future use)
	 * └─ Door Sway Loop Sound: doorSwayOpenClose (full 3s clip)
	 * 
	 * [AUDIO SOURCE]
	 * ├─ Auto-created if not assigned (happens in Awake)
	 * ├─ Play On Awake: false
	 * ├─ Spatial Blend: 1.0 (3D sound)
	 * └─ Volume: 1.0
	 * 
	 * 
	 * STEP 4: Add Interaction Script
	 * --------------------------------
	 * Option A: Use provided DoorInteractionWithKeycard.cs
	 * Option B: Create custom interaction in your player controller:
	 * 
	 * if (Input.GetKeyDown(KeyCode.E))
	 * {
	 *     SimpleDoorHinged door = hitDoor.GetComponent<SimpleDoorHinged>();
	 *     var result = door.TryOpen(); // or TryClose()
	 *     
	 *     if (result == DoorActionResult.Locked)
	 *         ShowMessage("Door is locked!");
	 * }
	 * 
	 * 
	 * STEP 5: Optional - Add Keycard System
	 * --------------------------------------
	 * 1. Add KeycardInventory.cs to a persistent GameObject
	 * 2. Add KeycardPickup.cs to keycard items in world
	 * 3. Set door's "Key Id" to match keycard's "Keycard Id"
	 * 4. Done! Door will check inventory automatically
	 */

	// ============================================================================
	// COMMON CONFIGURATIONS
	// ============================================================================
	/*
	 * 
	 * CONFIG 1: Basic Door (no lock)
	 * -------------------------------
	 * Key Id: (empty)
	 * Can Be Locked: false
	 * Uses Common Lock: false
	 * Auto Lock Delay: 0
	 * → Simple door that opens/closes freely
	 * 
	 * 
	 * CONFIG 2: Locked Door with Physical Key
	 * ----------------------------------------
	 * Key Id: "rusty_key"
	 * Requires Keycard: false
	 * Can Be Locked: true
	 * Uses Common Lock: true (if single lock visible from both sides)
	 * Init Inside Lock State: Locked
	 * → Player needs to find "rusty_key" item to unlock
	 * 
	 * 
	 * CONFIG 3: Electronic Keycard Door
	 * ----------------------------------
	 * Key Id: "security_level_3"
	 * Requires Keycard: true
	 * Can Be Locked: true
	 * Uses Common Lock: true
	 * Auto Lock Delay: 5.0
	 * → Swipe card to unlock, auto-locks after 5 seconds
	 * 
	 * 
	 * CONFIG 4: Double-Sided Lock (bathroom/bedroom)
	 * -----------------------------------------------
	 * Uses Common Lock: false
	 * Init Inside Lock State: Unlocked
	 * Init Outside Lock State: Unlocked
	 * → Can lock from inside OR outside independently
	 * 
	 * 
	 * CONFIG 5: Safe Room (auto-lock for safety)
	 * -------------------------------------------
	 * Key Id: "safe_room_key"
	 * Auto Lock Delay: 3.0
	 * Init Door State: Closed
	 * Init Inside Lock State: Unlocked
	 * → Player enters, door auto-locks after 3s (protection from monsters)
	 * 
	 * 
	 * CONFIG 6: Jammed Lock (can't be locked)
	 * ----------------------------------------
	 * Init Inside Lock State: UnlockedJam
	 * Can Be Locked: true
	 * → Lock is broken/jammed, can't be locked by player
	 * → Useful for forcing player through certain path
	 */
}