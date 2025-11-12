using UnityEngine;
using SPACE_UTIL;

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
	public void AnimEvent_Check()
	{
		Debug.Log(C.method(null, "grey", adMssg: "anim event"));
	}
	/// <summary>Call at END of doorOpeningAnim (REQUIRED)</summary>
	public void AnimEvent_DoorOpeningComplete()
	{
		Debug.Log(C.method(this, "grey", adMssg: "animeEvent"));
		_door?.OnAnimationComplete(AnimationEventType.DoorOpeningComplete);
	}
	/// <summary>Call at END of doorClosingAnim (REQUIRED)</summary>
	public void AnimEvent_DoorClosingComplete()
	{
		Debug.Log(C.method(this, "grey", adMssg: "animeEvent"));
		_door?.OnAnimationComplete(AnimationEventType.DoorClosingComplete);
	}

	// ========================================================================
	// Inside Lock Events - Add these to inside lock animation clips
	// ========================================================================
	/// <summary>Call at END of insideLockingAnim (REQUIRED)</summary>
	public void AnimEvent_InsideLockingComplete()
	{
		Debug.Log(C.method(this, "grey", adMssg: "animeEvent"));
		_door?.OnAnimationComplete(AnimationEventType.InsideLockingComplete);
	}
	/// <summary>Call at END of insideUnlockingAnim (REQUIRED)</summary>
	public void AnimEvent_InsideUnlockingComplete()
	{
		Debug.Log(C.method(this, "grey", adMssg: "animeEvent"));
		_door?.OnAnimationComplete(AnimationEventType.InsideUnlockingComplete);
	}
	// ========================================================================
	// Outside Lock Events - Add these to outside lock animation clips
	// ========================================================================
	/// <summary>Call at END of outsideLockingAnim (REQUIRED)</summary>
	public void AnimEvent_OutsideLockingComplete()
	{
		Debug.Log(C.method(this, "grey", adMssg: "animeEvent"));
		_door?.OnAnimationComplete(AnimationEventType.OutsideLockingComplete);
	}
	/// <summary>Call at END of outsideUnlockingAnim (REQUIRED)</summary>
	public void AnimEvent_OutsideUnlockingComplete()
	{
		Debug.Log(C.method(this, "grey", adMssg: "animeEvent"));
		_door?.OnAnimationComplete(AnimationEventType.OutsideUnlockingComplete);
	}
	// ========================================================================
	// Common Lock Events - Add these to common lock animation clips
	// ========================================================================

	/// <summary>Call at END of commonLockingAnim (REQUIRED)</summary>
	public void AnimEvent_CommonLockingComplete()
	{
		Debug.Log(C.method(this, "grey", adMssg: "animeEvent"));
		_door?.OnAnimationComplete(AnimationEventType.CommonLockingComplete);
	}
	/// <summary>Call at END of commonUnlockingAnim (REQUIRED)</summary>
	public void AnimEvent_CommonUnlockingComplete()
	{
		Debug.Log(C.method(this, "grey", adMssg: "animeEvent"));
		_door?.OnAnimationComplete(AnimationEventType.CommonUnlockingComplete);
	}
	// ========================================================================
	// Supernatural Events - Add these to sway animation clips
	// ========================================================================

	/// <summary>Call when transitioning OUT of doorSwayLoopAnim (REQUIRED)</summary>
	public void AnimEvent_DoorSwayStopped()
	{
		Debug.Log(C.method(this, "grey", adMssg: "animeEvent"));
		_door?.OnAnimationComplete(AnimationEventType.DoorSwayStopped);
	}
}

/*
	/// <summary>Call at START of insideLockingAnim (optional)</summary>
	public void AnimEvent_InsideLockingStarted()
	{
		Debug.Log(C.method(this, "grey", adMssg: "animeEvent"));
		_door?.OnAnimationComplete(AnimationEventType.InsideLockingStarted);
	}
	/// <summary>Call at START of doorOpeningAnim (optional)</summary>
	public void AnimEvent_DoorOpeningStarted()
	{
		Debug.Log(C.method(null, "grey", adMssg: "anim event"));
		_door?.OnAnimationComplete(AnimationEventType.DoorOpeningStarted);
	}
	/// <summary>Call at START of doorClosingAnim (optional)</summary>
	public void AnimEvent_DoorClosingStarted()
	{
		Debug.Log(C.method(this, "grey", adMssg: "animeEvent"));
		_door?.OnAnimationComplete(AnimationEventType.DoorClosingStarted);
	}
	/// <summary>Call at START of commonLockingAnim (optional)</summary>
	public void AnimEvent_CommonLockingStarted()
	{
		Debug.Log(C.method(this, "grey", adMssg: "animeEvent"));
		_door?.OnAnimationComplete(AnimationEventType.CommonLockingStarted);
	}
	/// <summary>Call at START of commonUnlockingAnim (optional)</summary>
	public void AnimEvent_CommonUnlockingStarted()
	{
		Debug.Log(C.method(this, "grey", adMssg: "animeEvent"));
		_door?.OnAnimationComplete(AnimationEventType.CommonUnlockingStarted);
	}
	/// <summary>Call at START of doorSwayLoopAnim (optional)</summary>
	public void AnimEvent_DoorSwayStarted()
	{
		Debug.Log(C.method(this, "grey", adMssg: "animeEvent"));
		_door?.OnAnimationComplete(AnimationEventType.DoorSwayStarted);
	}
	/// <summary>Call at START of insideUnlockingAnim (optional)</summary>
	public void AnimEvent_InsideUnlockingStarted()
	{
		Debug.Log(C.method(this, "grey", adMssg: "animeEvent"));
		_door?.OnAnimationComplete(AnimationEventType.InsideUnlockingStarted);
	}
	/// <summary>Call at START of outsideLockingAnim (optional)</summary>
	public void AnimEvent_OutsideLockingStarted()
	{
		Debug.Log(C.method(this, "grey", adMssg: "animeEvent"));
		_door?.OnAnimationComplete(AnimationEventType.OutsideLockingStarted);
	}
	/// <summary>Call at START of outsideUnlockingAnim (optional)</summary>
	public void AnimEvent_OutsideUnlockingStarted()
	{
		Debug.Log(C.method(this, "grey", adMssg: "animeEvent"));
		_door?.OnAnimationComplete(AnimationEventType.OutsideUnlockingStarted);
	}

*/