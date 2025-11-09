using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SPACE_UTIL;

namespace SPACE_GAME
{
	/*
		# High-Level Architecture
		```
		┌─────────────────────────────────────────────────────────┐
		│  IDoor (interface)                                      │
		│  - DoorState State { get; }                             │
		│  - bool TryOpen() / TryClose() / TryUnlock()            │
		│  - event Action<DoorState> OnStateChanged               │
		└─────────────────────────────────────────────────────────┘
							▲
							│ inherits
		┌─────────────────────────────────────────────────────────┐
		│  DoorBase : MonoBehaviour (abstract)                    │
		│  - Animator animator                                    │
		│  - DoorConfigSO config                                  │
		│  - Coroutine autoCloseRoutine                           │
		│  - IEnumerator OpenRoutine() / CloseRoutine()           │
		│  - bool IsObstructed()                                  │
		└─────────────────────────────────────────────────────────┘
				 ▲                    ▲                   ▲
				 │                    │                   │
		┌────────┴────────┐  ┌────────┴────────┐  ┌───────┴────────┐
		│ DoorHinged      │  │ DoorSliding     │  │ DoorDouble     │
		│ (rotate pivot)  │  │ (translate pos) │  │ (sync 2 doors) │
		└─────────────────┘  └─────────────────┘  └────────────────┘
		```
	*/
	
	// ==================== ENUMS ==================== //
	/// <summary>
	/// All possible door states. Add to GameStore.cs if you want global enum.
	/// </summary>
	public enum DoorState
	{
		Closed,     // Fully closed, can be opened
		Opening,    // Animation playing, cannot interrupt
		Opened,     // Fully open, can be closed
		Closing,    // Animation playing, can be interrupted by obstruction

		Locked,     // Closed + locked, requires key/code
		Swaying,    // Idle horror animation, can transition to Opening
		Blocked,    // Obstruction detected during auto-close
	}

	// ==================== INTERFACE ==================== //
	/// <summary>
	/// Contract for all door types. Implemented by DoorBase.
	/// </summary>
	public interface IDoor
	{
		// ===== PUBLIC PROPERTIES ===== //
		DoorState State { get; }
		bool IsLocked { get; }
		bool IsOpen { get; } // Quick check: State == Opened

		// ===== PUBLIC API ===== //
		bool TryOpen(bool force = false);
		bool TryClose(bool force = false);
		bool TryUnlock(string keyId = null);
		void Lock();

		// ===== EVENTS ===== //
		event Action<DoorState> OnStateChanged;
		event Action OnDoorOpened;   // Fired when door reaches Opened state
		event Action OnDoorClosed;   // Fired when door reaches Closed state
		event Action OnDoorLocked;   // Fired when TryOpen fails due to lock
	}

	// ==================== BASE CLASS (ABSTRACT) ==================== //
	/// <summary>
	/// Base class for all doors. Handles state machine, audio, auto-close, save/load.
	/// Inherit from this and override PerformOpen/PerformClose for custom animation logic.
	/// </summary>
	public abstract class DoorBase : MonoBehaviour, IDoor
	{
		// ===== SERIALIZED FIELDS ===== //
		[Header("References")]
		[SerializeField] protected Animator animator;
		[SerializeField] protected Transform physicsPivot; // For rotation/translation
		[SerializeField] protected Collider blockingCollider;
		[SerializeField] protected Collider interactionTrigger;

		[Header("Configuration")]
		[SerializeField] protected DoorConfigSO config;

		[Header("State")]
		[SerializeField] protected DoorState currentState = DoorState.Closed;
		[SerializeField] protected bool isLocked = false;
		[SerializeField] protected string requiredKeyId = ""; // Empty = no key needed

		[Header("Auto-Close")]
		[SerializeField] protected bool autoCloseEnabled = false;
		[SerializeField] protected float autoCloseDelay = 3f;
		[SerializeField] protected float obstructionCheckRadius = 0.3f;
		[SerializeField] protected LayerMask obstructionLayers = -1; // All layers
		[SerializeField] protected int maxAutoCloseRetries = 5;

		// ===== PRIVATE FIELDS ===== //
		private Coroutine openRoutine;
		private Coroutine closeRoutine;
		private Coroutine autoCloseRoutine;
		private int autoCloseAttempts = 0;
		private float playerStuckTimer = 0f;
		
		// ===== PROPERTIES (INTERFACE) ===== //
		public DoorState State => currentState;
		public bool IsLocked => isLocked;
		public bool IsOpen => currentState == DoorState.Opened;

		// ===== EVENTS (INTERFACE) ===== //
		public event Action<DoorState> OnStateChanged;
		public event Action OnDoorOpened;
		public event Action OnDoorClosed;
		public event Action OnDoorLocked;

		// ===== UNITY LIFECYCLE ===== //
		protected virtual void Awake()
		{
			if (animator == null) animator = GetComponent<Animator>();
			if (config == null) Debug.Log(C.method(this, "red", "DoorConfigSO is null! Assign in Inspector."));
		}
		protected virtual void Start()
		{
			// Load saved state from GameStore if applicable
			LoadState();
		}
		protected virtual void OnDestroy()
		{
			SaveState();
		}

		void Update()
		{
			if (currentState == DoorState.Closed || currentState == DoorState.Closing)
			{
				if (IsPlayerInsideCollider())
				{
					playerStuckTimer += Time.deltaTime;
					if (playerStuckTimer > 1f)
					{
						Debug.Log(C.method(this, "orange", "Player stuck, force-opening door"));
						TryOpen(force: true);
						playerStuckTimer = 0f;
					}
				}
				else
				{
					playerStuckTimer = 0f;
				}
			}
		}


		// ===== PUBLIC API (INTERFACE IMPLEMENTATION) ===== //
		public bool TryOpen(bool force = false)
		{
			Debug.Log(C.method(this, "cyan", $"TryOpen() called. State={currentState}, Locked={isLocked}, Force={force}"));

			// Check if locked
			if (isLocked && !force)
			{
				Debug.Log(C.method(this, "yellow", "Door is locked!"));
				OnDoorLocked?.Invoke();
				PlayLockedFeedback();
				return false;
			}

			// Check if already open/opening
			if (currentState == DoorState.Opened || currentState == DoorState.Opening)
			{
				Debug.Log(C.method(this, "yellow", "Door already open/opening"));
				return false;
			}

			// Stop any ongoing close routine
			if (closeRoutine != null)
			{
				StopCoroutine(closeRoutine);
				closeRoutine = null;
			}

			// Start open routine
			openRoutine = StartCoroutine(OpenRoutine());
			return true;
		}
		public bool TryClose(bool force = false)
		{
			Debug.Log(C.method(this, "cyan", $"TryClose() called. State={currentState}, Force={force}"));

			// Check if already closed/closing
			if (currentState == DoorState.Closed || currentState == DoorState.Closing)
			{
				Debug.Log(C.method(this, "yellow", "Door already closed/closing"));
				return false;
			}

			// Check obstruction (unless forced)
			if (!force && IsObstructed())
			{
				Debug.Log(C.method(this, "yellow", "Door closing blocked by obstruction"));
				SetState(DoorState.Blocked);
				return false;
			}

			// Stop any ongoing open routine
			if (openRoutine != null)
			{
				StopCoroutine(openRoutine);
				openRoutine = null;
			}

			// Stop auto-close if running
			if (autoCloseRoutine != null)
			{
				StopCoroutine(autoCloseRoutine);
				autoCloseRoutine = null;
			}

			// Start close routine
			closeRoutine = StartCoroutine(CloseRoutine());
			return true;
		}
		public bool TryUnlock(string keyId = null)
		{
			if (!isLocked)
			{
				Debug.Log(C.method(this, "yellow", "Door already unlocked"));
				return true;
			}

			// Check if keyId matches (if required)
			if (!string.IsNullOrEmpty(requiredKeyId) && requiredKeyId != keyId)
			{
				Debug.Log(C.method(this, "red", $"Wrong key! Required: {requiredKeyId}, Got: {keyId}"));
				return false;
			}

			isLocked = false;
			Debug.Log(C.method(this, "lime", "Door unlocked!"));
			PlayUnlockSound();
			return true;
		}
		public void Lock()
		{
			if (currentState != DoorState.Closed)
			{
				Debug.Log(C.method(this, "yellow", "Cannot lock door while open"));
				return;
			}

			isLocked = true;
			SetState(DoorState.Locked);
			Debug.Log(C.method(this, "lime", "Door locked"));
		}

		// ===== COROUTINES (PRIVATE) ===== //
		private IEnumerator OpenRoutine()
		{
			SetState(DoorState.Opening);

			// Play unlock sound if just unlocked
			if (isLocked) PlayUnlockSound();

			// Trigger animation
			if (animator != null) animator.TrySetTrigger(AnimParamType.doorOpen);

			// Play opening sound (via animation event or here)
			PlayOpenSound();

			// Perform custom open logic (rotation, translation, etc.)
			yield return StartCoroutine(PerformOpen());

			// Door fully opened
			SetState(DoorState.Opened); OnDoorOpened?.Invoke();

			// Start auto-close timer if enabled
			if (autoCloseEnabled) autoCloseRoutine = StartCoroutine(AutoCloseRoutine());
		}
		private IEnumerator CloseRoutine()
		{
			SetState(DoorState.Closing);

			// Trigger animation
			if (animator != null)
				animator.TrySetTrigger(AnimParamType.doorClose);

			// Play closing sound
			PlayCloseSound();

			// Perform custom close logic
			yield return StartCoroutine(PerformClose());

			// Door fully closed
			SetState(DoorState.Closed);
			OnDoorClosed?.Invoke();
		}
		private IEnumerator AutoCloseRoutine()
		{
			Debug.Log(C.method(this, "cyan", $"Auto-close starts in {autoCloseDelay}s"));
			yield return new WaitForSeconds(autoCloseDelay);

			autoCloseAttempts = 0;
			while (autoCloseAttempts < maxAutoCloseRetries)
			{
				if (!IsObstructed())
				{
					Debug.Log(C.method(this, "lime", "Auto-close: no obstruction, closing now"));
					TryClose();
					yield break;
				}

				autoCloseAttempts++;
				Debug.Log(C.method(this, "yellow", $"Auto-close retry {autoCloseAttempts}/{maxAutoCloseRetries}"));
				yield return new WaitForSeconds(1f);
			}

			// Force close after max retries
			Debug.Log(C.method(this, "orange", "Auto-close: forcing close after retries"));
			TryClose(force: true);

			// yield return new WaitUntil(() => 1 == 2);
		}

		// ===== ABSTRACT METHODS (MUST OVERRIDE) ===== //

		/// <summary>
		/// Override this to define custom open animation logic.
		/// Example: rotate pivot, translate position, etc.
		/// Must yield until animation completes.
		/// </summary>
		protected abstract IEnumerator PerformOpen();

		/// <summary>
		/// Override this to define custom close animation logic.
		/// </summary>
		protected abstract IEnumerator PerformClose();

		// ===== HELPER METHODS (PROTECTED) ===== //
		protected void SetState(DoorState newState)
		{
			if (currentState == newState) return;

			DoorState oldState = currentState;
			currentState = newState;

			Debug.Log(C.method(this, "cyan", $"State: {oldState} → {newState}"));
			OnStateChanged?.Invoke(newState);
		}
		protected bool IsObstructed()
		{
			if (blockingCollider == null) return false;

			// Check for overlapping colliders in blocking collider bounds
			Collider[] hits = Physics.OverlapBox(
				blockingCollider.bounds.center,
				blockingCollider.bounds.extents,
				Quaternion.identity,
				obstructionLayers,
				QueryTriggerInteraction.Ignore
			);

			// Filter out self
			foreach (Collider hit in hits)
			{
				if (hit != blockingCollider && hit.transform.root != transform.root)
				{
					Debug.Log(C.method(this, "yellow", $"Obstruction detected: {hit.name}"));
					return true;
				}
			}

			return false;
		}
		protected bool IsPlayerInsideCollider()
		{
			// Check if player's center is inside door bounds
			Transform player = Camera.main != null ? Camera.main.transform : null;
			if (player == null) return false;

			return blockingCollider.bounds.Contains(player.position);
		}

		// ===== AUDIO (PROTECTED) ===== //
		protected void PlayOpenSound()
		{
			if (config != null && config.openSound != null)
				AudioSource.PlayClipAtPoint(config.openSound, transform.position, config.volume);
		}
		protected void PlayCloseSound()
		{
			if (config != null && config.closeSound != null)
				AudioSource.PlayClipAtPoint(config.closeSound, transform.position, config.volume);
		}
		protected void PlayUnlockSound()
		{
			if (config != null && config.unlockSound != null)
				AudioSource.PlayClipAtPoint(config.unlockSound, transform.position, config.volume);
		}
		protected void PlayLockedFeedback()
		{
			// Play locked rattle sound + jiggle animation
			if (config != null && config.lockedSound != null)
				AudioSource.PlayClipAtPoint(config.lockedSound, transform.position, config.volume);

			if (animator != null)
				animator.TrySetTrigger(AnimParamType.doorLocked);
		}

		// ===== SAVE/LOAD (PROTECTED) ===== //
		protected virtual void SaveState()
		{
			// Example: Save door state to GameStore
			// DoorSaveData data = new DoorSaveData
			// {
			//     doorId = gameObject.name,
			//     state = currentState,
			//     isLocked = isLocked,
			// };
			// LOG.SaveGameData(GameDataType.doorStates, data.ToJson());
		}
		protected virtual void LoadState()
		{
			// Example: Load door state from GameStore
			// DoorSaveData data = LOG.LoadGameData<DoorSaveData>(GameDataType.doorStates);
			// if (data != null && data.doorId == gameObject.name)
			// {
			//     currentState = data.state;
			//     isLocked = data.isLocked;
			// }
		}
	}
	
	// ==================== SAVE DATA ==================== //
	[System.Serializable]
	public class DoorSaveData
	{
		public string doorId;
		public DoorState state;
		public bool isLocked;
		public string requiredKeyId;
	}
}