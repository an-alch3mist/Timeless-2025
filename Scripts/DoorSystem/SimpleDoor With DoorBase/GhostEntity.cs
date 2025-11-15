namespace SPACE_GAME_1
{
	using UnityEngine;
	using System.Collections;
	using SPACE_UTIL;

	/// <summary>
	/// Example supernatural entity that can control doors.
	/// Demonstrates door swaying, blocking, and forced actions.
	/// </summary>
	public class GhostEntity : MonoBehaviour
	{
		[Header("Target Door")]
		[SerializeField] DoorBase _targetDoor;

		[Header("Haunting Behavior")]
		[SerializeField] float _swayDuration = 5f;
		[SerializeField] float _hauntingInterval = 10f;
		[SerializeField] bool _startHauntingOnAwake = true;

		[Header("Effects")]
		[SerializeField] ParticleSystem _ghostParticles;
		[SerializeField] Light _flickeringLight;

		void Start()
		{
			Debug.Log(C.method(null, C.colorStr.red));
			if (this._startHauntingOnAwake == true)
				StartCoroutine(HauntingLoop());
		}

		// ========================================================================
		// HAUNTING BEHAVIORS
		// ========================================================================
		IEnumerator HauntingLoop()
		{
			while (true)
			{
				yield return new WaitForSeconds(_hauntingInterval);
				// Random haunting action
				int action = Random.Range(0, 4);

				switch (action)
				{
					case 0:
						StartCoroutine(MakeDoorSway());
						break;
					case 1:
						StartCoroutine(SlamDoorShut());
						break;
					case 2:
						StartCoroutine(LockDoorMenacingly());
						break;
					case 3:
						StartCoroutine(OpenDoorSlowly());
						break;
				}
			}
		}

		// ========================================================================
		// INDIVIDUAL HAUNTING ACTIONS
		// ========================================================================

		/// <summary>
		/// Makes door sway back and forth (creepy!)
		/// </summary>
		IEnumerator MakeDoorSway()
		{
			Debug.Log("[Ghost] Making door sway...".colorTag("cyan"));

			// Start particles
			_ghostParticles?.Play();

			// Start swaying
			var result = _targetDoor.TryStartSwaying();
			if (result == DoorActionResult.Success)
			{
				// Flicker light while swaying
				if (_flickeringLight != null)
					StartCoroutine(FlickerLight(_swayDuration));

				// Sway for duration
				yield return new WaitForSeconds(_swayDuration);

				// Stop swaying - leave door in creepy open position
				_targetDoor.TryStopSwaying(DoorState.Opened);

				Debug.Log("[Ghost] Stopped swaying, door left open...".colorTag("yellow"));
			}

			_ghostParticles?.Stop();
		}

		/// <summary>
		/// Slams door shut violently
		/// </summary>
		IEnumerator SlamDoorShut()
		{
			Debug.Log("[Ghost] Slamming door shut!".colorTag("red"));

			// Wait for dramatic timing
			yield return new WaitForSeconds(0.5f);

			// Force close (bypasses checks)
			var result = _targetDoor.ForceClose();

			if (result == DoorActionResult.Success)
			{
				// Screen shake could go here
				Debug.Log("[Ghost] SLAM!".colorTag("red"));

				// Lock it after slamming
				yield return new WaitForSeconds(0.5f);
				_targetDoor.TryLock(LockSide.Inside);
				_targetDoor.TryLock(LockSide.Outside);
			}
		}

		/// <summary>
		/// Locks door while player is approaching (trap!)
		/// </summary>
		IEnumerator LockDoorMenacingly()
		{
			Debug.Log("[Ghost] Locking door...".colorTag("yellow"));

			// Lock both sides
			_targetDoor.TryLock(LockSide.Inside);
			_targetDoor.TryLock(LockSide.Outside);

			yield return new WaitForSeconds(2f);

			// Optionally unlock after scaring player
			if (Random.value > 0.5f)
			{
				Debug.Log("[Ghost] Just kidding... unlocking".colorTag("grey"));
				_targetDoor.TryUnlock(LockSide.Inside);
				_targetDoor.TryUnlock(LockSide.Outside);
			}
		}

		/// <summary>
		/// Opens door slowly and ominously
		/// </summary>
		IEnumerator OpenDoorSlowly()
		{
			Debug.Log("[Ghost] Opening door slowly...".colorTag("cyan"));

			// Unlock if needed
			if (_targetDoor.currInsideLockState == DoorLockState.Locked)
				_targetDoor.TryUnlock(LockSide.Inside);
			if (_targetDoor.currOutsideLockState == DoorLockState.Locked)
				_targetDoor.TryUnlock(LockSide.Outside);

			yield return new WaitForSeconds(0.5f);

			// Open door
			var result = _targetDoor.TryOpen();

			if (result == DoorActionResult.Success)
			{
				Debug.Log("[Ghost] Door creaks open...".colorTag("grey"));
			}
		}

		/// <summary>
		/// Blocks door supernaturally (cannot be opened/closed)
		/// </summary>
		public void BlockDoor(float duration)
		{
			StartCoroutine(BlockDoorTemporarily(duration));
		}
		IEnumerator BlockDoorTemporarily(float duration)
		{
			Debug.Log("[Ghost] Blocking door with supernatural force!".colorTag("red"));

			_targetDoor.ForceBlock();
			_ghostParticles?.Play();

			yield return new WaitForSeconds(duration);

			_targetDoor.ForceUnblock();
			_ghostParticles?.Stop();

			Debug.Log("[Ghost] Block released".colorTag("grey"));
		}

		// ========================================================================
		// VISUAL EFFECTS
		// ========================================================================
		IEnumerator FlickerLight(float duration)
		{
			if (_flickeringLight == null) yield break;

			float elapsed = 0f;
			bool originalState = _flickeringLight.enabled;

			while (elapsed < duration)
			{
				_flickeringLight.enabled = Random.value > 0.5f;
				yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
				elapsed += Time.deltaTime;
			}

			_flickeringLight.enabled = originalState;
		}

		// ========================================================================
		// PUBLIC API - Call from triggers/events
		// ========================================================================
		public void OnPlayerEnterRoom()
		{
			// Slam door shut when player enters
			StartCoroutine(SlamDoorShut());
		}
		public void OnPlayerNearDoor()
		{
			// Start swaying to spook player
			StartCoroutine(MakeDoorSway());
		}
		public void OnPlayerLookingAtDoor()
		{
			// Open slowly when player is watching
			StartCoroutine(OpenDoorSlowly());
		}
	}

	// ============================================================================
	// USAGE EXAMPLES
	// ============================================================================
	/*
	 * SETUP:
	 * ------
	 * 1. Create empty GameObject: "GhostEntity"
	 * 2. Add GhostEntity script
	 * 3. Assign Target Door in inspector
	 * 4. Optional: Assign particle system and light
	 * 5. Configure haunting behavior:
	 *    - Sway Duration: 5s
	 *    - Haunting Interval: 10s
	 *    - Start Haunting On Awake: true
	 * 
	 * 
	 * TRIGGER-BASED HAUNTING:
	 * -----------------------
	 * // In your trigger volume script:
	 * void OnTriggerEnter(Collider other)
	 * {
	 *     if (other.CompareTag("Player"))
	 *     {
	 *         GhostEntity ghost = FindObjectOfType<GhostEntity>();
	 *         ghost?.OnPlayerEnterRoom(); // Slams door
	 *     }
	 * }
	 * 
	 * 
	 * CUTSCENE HAUNTING:
	 * ------------------
	 * // In your cutscene controller:
	 * IEnumerator HauntingCutscene()
	 * {
	 *     GhostEntity ghost = FindObjectOfType<GhostEntity>();
	 *     
	 *     yield return new WaitForSeconds(1f);
	 *     ghost.StartCoroutine(ghost.MakeDoorSway());
	 *     
	 *     yield return new WaitForSeconds(6f);
	 *     ghost.StartCoroutine(ghost.SlamDoorShut());
	 * }
	 * 
	 * 
	 * BOSS FIGHT MECHANICS:
	 * ---------------------
	 * // Boss can trap player in room
	 * void TrapPlayer()
	 * {
	 *     foreach (var door in roomDoors)
	 *     {
	 *         door.ForceBlock();
	 *         door.TryLock(LockSide.Inside);
	 *         door.TryLock(LockSide.Outside);
	 *     }
	 * }
	 * 
	 * void ReleasePlayer()
	 * {
	 *     foreach (var door in roomDoors)
	 *     {
	 *         door.ForceUnblock();
	 *         door.TryUnlock(LockSide.Inside);
	 *         door.TryUnlock(LockSide.Outside);
	 *     }
	 * }
	 */
}