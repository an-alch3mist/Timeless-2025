using UnityEngine;

/// <summary>
/// Forwards Unity Animation Events to IDoor implementation.
/// Unity Animation Events only support primitive types, so we use individual methods.
/// Attach this component to the same GameObject as your IDoor implementation (e.g. DoorHinged).
/// 
/// SETUP: In Animation Clips, add Animation Events that call these methods at appropriate frames.
/// Example: doorOpeningAnim.anim - add event at LAST frame calling AnimEvent_DoorOpeningComplete()
/// </summary>
public class DoorAnimationEventForwarder : MonoBehaviour
{
	private IDoor _door;
	private void Awake()
	{
		// Get IDoor component on same GameObject
		_door = this.GetComponent<IDoor>();
		if (_door == null)
		{
			Debug.LogError($"[DoorAnimationEventForwarder] No IDoor component found on {gameObject.name}! " +
						  "This component must be on the same GameObject as DoorHinged (or other IDoor implementation).", this);
			enabled = false;
		}
	}

	// ========================================================================
	// Door Movement Events - Add these to door animation clips
	// ========================================================================

	/// <summary>Call at START of doorOpeningAnim (optional)</summary>
	public void AnimEvent_DoorOpeningStarted() =>
		_door?.OnAnimationComplete(AnimationEventType.DoorOpeningStarted);

	/// <summary>Call at END of doorOpeningAnim (REQUIRED)</summary>
	public void AnimEvent_DoorOpeningComplete() =>
		_door?.OnAnimationComplete(AnimationEventType.DoorOpeningComplete);

	/// <summary>Call at START of doorClosingAnim (optional)</summary>
	public void AnimEvent_DoorClosingStarted() =>
		_door?.OnAnimationComplete(AnimationEventType.DoorClosingStarted);

	/// <summary>Call at END of doorClosingAnim (REQUIRED)</summary>
	public void AnimEvent_DoorClosingComplete() =>
		_door?.OnAnimationComplete(AnimationEventType.DoorClosingComplete);

	// ========================================================================
	// Inside Lock Events - Add these to inside lock animation clips
	// ========================================================================

	/// <summary>Call at START of insideLockingAnim (optional)</summary>
	public void AnimEvent_InsideLockingStarted() =>
		_door?.OnAnimationComplete(AnimationEventType.InsideLockingStarted);

	/// <summary>Call at END of insideLockingAnim (REQUIRED)</summary>
	public void AnimEvent_InsideLockingComplete() =>
		_door?.OnAnimationComplete(AnimationEventType.InsideLockingComplete);

	/// <summary>Call at START of insideUnlockingAnim (optional)</summary>
	public void AnimEvent_InsideUnlockingStarted() =>
		_door?.OnAnimationComplete(AnimationEventType.InsideUnlockingStarted);

	/// <summary>Call at END of insideUnlockingAnim (REQUIRED)</summary>
	public void AnimEvent_InsideUnlockingComplete() =>
		_door?.OnAnimationComplete(AnimationEventType.InsideUnlockingComplete);

	// ========================================================================
	// Outside Lock Events - Add these to outside lock animation clips
	// ========================================================================

	/// <summary>Call at START of outsideLockingAnim (optional)</summary>
	public void AnimEvent_OutsideLockingStarted() =>
		_door?.OnAnimationComplete(AnimationEventType.OutsideLockingStarted);

	/// <summary>Call at END of outsideLockingAnim (REQUIRED)</summary>
	public void AnimEvent_OutsideLockingComplete() =>
		_door?.OnAnimationComplete(AnimationEventType.OutsideLockingComplete);

	/// <summary>Call at START of outsideUnlockingAnim (optional)</summary>
	public void AnimEvent_OutsideUnlockingStarted() =>
		_door?.OnAnimationComplete(AnimationEventType.OutsideUnlockingStarted);

	/// <summary>Call at END of outsideUnlockingAnim (REQUIRED)</summary>
	public void AnimEvent_OutsideUnlockingComplete() =>
		_door?.OnAnimationComplete(AnimationEventType.OutsideUnlockingComplete);

	// ========================================================================
	// Common Lock Events - Add these to common lock animation clips
	// ========================================================================

	/// <summary>Call at START of commonLockingAnim (optional)</summary>
	public void AnimEvent_CommonLockingStarted() =>
		_door?.OnAnimationComplete(AnimationEventType.CommonLockingStarted);

	/// <summary>Call at END of commonLockingAnim (REQUIRED)</summary>
	public void AnimEvent_CommonLockingComplete() =>
		_door?.OnAnimationComplete(AnimationEventType.CommonLockingComplete);

	/// <summary>Call at START of commonUnlockingAnim (optional)</summary>
	public void AnimEvent_CommonUnlockingStarted() =>
		_door?.OnAnimationComplete(AnimationEventType.CommonUnlockingStarted);

	/// <summary>Call at END of commonUnlockingAnim (REQUIRED)</summary>
	public void AnimEvent_CommonUnlockingComplete() =>
		_door?.OnAnimationComplete(AnimationEventType.CommonUnlockingComplete);

	// ========================================================================
	// Supernatural Events - Add these to sway animation clips
	// ========================================================================

	/// <summary>Call at START of doorSwayLoopAnim (optional)</summary>
	public void AnimEvent_DoorSwayStarted() =>
		_door?.OnAnimationComplete(AnimationEventType.DoorSwayStarted);

	/// <summary>Call when transitioning OUT of doorSwayLoopAnim (REQUIRED)</summary>
	public void AnimEvent_DoorSwayStopped() =>
		_door?.OnAnimationComplete(AnimationEventType.DoorSwayStopped);
}