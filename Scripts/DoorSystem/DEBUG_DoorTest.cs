using System.Collections;
using UnityEngine;

using SPACE_UTIL;
using SPACE_GAME;

namespace SPACE_CHECK
{
	/// <summary>
	/// Comprehensive door testing script.
	/// Attach to any GameObject in scene for quick testing.
	/// </summary>
	public class DEBUG_DoorTest : MonoBehaviour
	{
		// ====================================================================
		// SERIALIZED FIELDS
		// ====================================================================

		[Header("Target Door")]
		[SerializeField] Door door;

		[Header("Test Settings")]
		[SerializeField] bool autoFindDoor = true;
		[SerializeField] DoorSide testSide = DoorSide.inside;

		[Header("Input (Mouse)")]
		[Tooltip("0=Left, 1=Right, 2=Middle")]
		[Range(0, 2)]
		[SerializeField] int mouseButton = 0;

		[Header("Input (Keyboard)")]
		[SerializeField] KeyCode openCloseKey = KeyCode.E;
		[SerializeField] KeyCode lockInsideKey = KeyCode.Alpha1;
		[SerializeField] KeyCode unlockInsideKey = KeyCode.Alpha2;
		[SerializeField] KeyCode lockOutsideKey = KeyCode.Alpha3;
		[SerializeField] KeyCode unlockOutsideKey = KeyCode.Alpha4;
		[SerializeField] KeyCode blockKey = KeyCode.B;
		[SerializeField] KeyCode unblockKey = KeyCode.N;
		[SerializeField] KeyCode swayKey = KeyCode.S;
		[SerializeField] KeyCode stopSwayKey = KeyCode.X;
		[SerializeField] KeyCode printDoorStateKey = KeyCode.P;

		// ====================================================================
		// UNITY LIFECYCLE
		// ====================================================================

		void Start()
		{
			Debug.Log(C.method(this, "white"));

			if (autoFindDoor && door == null)
			{
				door = FindObjectOfType<Door>();

				if (door != null)
				{
					Debug.Log(C.method(this, "lime", adMssg: $"auto-found door: {door.name}"));
				}
				else
				{
					Debug.Log(C.method(this, "red", adMssg: "no door found in scene!"));
				}
			}
			// Subscribe to events
			DoorEvents.OnDoorStateChanged.AddListener(OnDoorStateChanged);
			DoorEvents.OnDoorLockStateChanged.AddListener(OnDoorLockStateChanged);
			DoorEvents.OnDoorBlocked.AddListener(OnDoorBlocked);
			DoorEvents.OnDoorLockedJiggle.AddListener(OnDoorLockedJiggle);
			DoorEvents.OnDoorSwayingStart.AddListener(OnDoorSwayingStart);
			DoorEvents.OnDoorSwayingStop.AddListener(OnDoorSwayingStop);

			PrintControls();
		}

		void Update()
		{
			if (door == null) return;

			// Mouse click: toggle door
			if (INPUT.M.InstantDown(mouseButton)) ToggleDoor();

			// Keyboard inputs
			if (INPUT.K.InstantDown(openCloseKey)) ToggleDoor();
			if (INPUT.K.InstantDown(lockInsideKey)) door.TryLockInside();
			if (INPUT.K.InstantDown(unlockInsideKey)) door.TryUnlockInside();
			if (INPUT.K.InstantDown(lockOutsideKey)) door.TryLockOutside();
			if (INPUT.K.InstantDown(unlockOutsideKey)) door.TryUnlockOutside();
			if (INPUT.K.InstantDown(blockKey)) door.TryBlock();
			if (INPUT.K.InstantDown(unblockKey)) door.TryUnblock();
			if (INPUT.K.InstantDown(swayKey)) door.TryDoorSwaying();
			if (INPUT.K.InstantDown(stopSwayKey)) door.TryDoorStopSwaying(DoorState.opened);
			if (INPUT.K.InstantDown(printDoorStateKey)) this.PrintDoorState();
		}

		// ====================================================================
		// PRIVATE API
		// ====================================================================

		void ToggleDoor()
		{
			if (door.doorState == DoorState.closed || door.doorState == DoorState.closing)
			{
				door.TryOpen(testSide);
			}
			else if (door.doorState == DoorState.opened || door.doorState == DoorState.opening)
			{
				door.TryClose();
			}
		}

		void PrintControls()
		{
			string controls = @"
=== DOOR TEST CONTROLS ===
[E] or [Mouse Left] - Toggle Open/Close
[1] - Lock Inside
[2] - Unlock Inside
[3] - Lock Outside
[4] - Unlock Outside
[B] - Block Door
[N] - Unblock Door
[S] - Start Swaying
[X] - Stop Swaying
==========================";

			Debug.Log(controls.colorTag("cyan"));
		}

		// ====================================================================
		// EVENT HANDLERS
		// ====================================================================

		void OnDoorStateChanged(string doorId, DoorState newState)
		{
			Debug.Log(C.method(this, "lime", adMssg: $"Door[{doorId}] state -> {newState}"));
		}

		void OnDoorLockStateChanged(string doorId, DoorLockState newLockState, DoorSide side)
		{
			Debug.Log(C.method(this, "lime", adMssg: $"Door[{doorId}] {side} lock -> {newLockState}"));
		}

		void OnDoorBlocked(string doorId)
		{
			Debug.Log(C.method(this, "red", adMssg: $"Door[{doorId}] BLOCKED!"));
		}

		void OnDoorLockedJiggle(string doorId)
		{
			Debug.Log(C.method(this, "yellow", adMssg: $"Door[{doorId}] locked jiggle"));
		}

		void OnDoorSwayingStart(string doorId)
		{
			Debug.Log(C.method(this, "cyan", adMssg: $"Door[{doorId}] swaying started"));
		}

		void OnDoorSwayingStop(string doorId)
		{
			Debug.Log(C.method(this, "cyan", adMssg: $"Door[{doorId}] swaying stopped"));
		}

		// ====================================================================
		// PUBLIC API - AUTOMATED TESTS
		// ====================================================================

		/// <summary>
		/// Run full test sequence. Call from inspector button or other script.
		/// </summary>
		[ContextMenu("Run Full Test Sequence")]
		public void RunFullTestSequence()
		{
			if (door == null)
			{
				Debug.Log(C.method(this, "red", adMssg: "no door assigned!"));
				return;
			}

			StartCoroutine(TestSequence());
		}
		IEnumerator TestSequence()
		{
			Debug.Log(C.method(this, "white", adMssg: "=== TEST SEQUENCE START ==="));

			// Test 1: Open/Close
			Debug.Log("TEST 1: Open/Close".colorTag("cyan"));
			door.TryOpen(testSide);
			yield return new WaitForSeconds(2f);

			door.TryClose();
			yield return new WaitForSeconds(2f);

			// Test 2: Lock Inside
			Debug.Log("TEST 2: Lock Inside".colorTag("cyan"));
			door.TryLockInside();
			yield return new WaitForSeconds(1f);

			// Try to open (should fail)
			bool openResult = door.TryOpen(testSide);
			Debug.Log($"Try open while locked: {(openResult ? "FAIL" : "PASS")}".colorTag(openResult ? "red" : "lime"));
			yield return new WaitForSeconds(1f);

			// Unlock
			door.TryUnlockInside();
			yield return new WaitForSeconds(1f);

			// Test 3: Block
			Debug.Log("TEST 3: Block".colorTag("cyan"));
			door.TryBlock();
			yield return new WaitForSeconds(1f);

			// Try to open (should fail)
			openResult = door.TryOpen(testSide);
			Debug.Log($"Try open while blocked: {(openResult ? "FAIL" : "PASS")}".colorTag(openResult ? "red" : "lime"));
			yield return new WaitForSeconds(1f);

			// Unblock
			door.TryUnblock();
			yield return new WaitForSeconds(1f);

			// Test 4: Swaying
			Debug.Log("TEST 4: Swaying".colorTag("cyan"));
			door.TryDoorSwaying();
			yield return new WaitForSeconds(3f);

			door.TryDoorStopSwaying(DoorState.closed);
			yield return new WaitForSeconds(1f);

			Debug.Log(C.method(this, "lime", adMssg: "=== TEST SEQUENCE COMPLETE ==="));
		}

		/// <summary>
		/// Print current door state to console
		/// </summary>
		[ContextMenu("Print Door State")]
		public void PrintDoorState()
		{
			if (door == null)
			{
				Debug.Log(C.method(this, "red", adMssg: "no door assigned!"));
				return;
			}

			string doorStateStr = $@"
=== DOOR STATE ===
Door State: {door.doorState}
Lock Inside: {door.doorLockStateInside}
Lock Outside: {door.doorLockStateOutside}
Blocked: {door.blocked}
Uses Common Lock: {door.usesCommonLock}
==================";

			Debug.Log(doorStateStr.colorTag("cyan"));
			LOG.AddLog(doorStateStr);
		}
	}
}