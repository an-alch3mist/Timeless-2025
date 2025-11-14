using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SPACE_UTIL;
using System;

// Prototype + Crucial
public class SimpleDoorHinged : MonoBehaviour, IDoor
{
	[Header("just to log")]
	[TextArea(minLines: 15, 20)] string _getStr;

	[Header("CONFIGURATION CONSTANT THROUGH OUT GAME VAL")]
	[SerializeField] bool _usesCommonLock = false;
	[SerializeField] bool _canBeLocked = true;
	[SerializeField] int _maxClosingRetries = 5;

	[Header("READ-ONLY PROPERTIES INIT VAL")]
	[SerializeField] bool _initIsBlocked = false;
	[SerializeField] DoorState _initDoorState = DoorState.Opened;
	[SerializeField] DoorLockState _initDoorLockInside = DoorLockState.Unlocked, _initDoorLockOutside = DoorLockState.Unlocked;

	public bool usesCommonLock { get; set; }
	public bool canBeLocked { get; set; }
	public bool initInsideUnlockedJammed { get; set; }
	public bool initOutsideUnlockedJammed { get; set; }
	public int maxClosingRetries { get; set; }

	public bool isBlocked { get; set; }
	public DoorState currDoorState { get; private set; }
	public DoorLockState currInsideLockState { get; private set; }
	public DoorLockState currOutsideLockState { get; private set; }

	public bool isAnimatingDoorPanel { get; set; } // not required for now
	public bool isAnimatingDoorLockInside { get; set; } // not required for now
	public bool isAnimatingDoorLockOutside { get; set; } // not required for now
	public bool isAnimatingDoorLockCommon { get; set; } // not required for now

	public event Action<DoorState> OnDoorStateChanged; // for audio and other visual cues
	public event Action<DoorLockState, LockSide> OnLockStateChanged; // for audio and other visuals cues

	[SerializeField] Animator _animator;
	public void Init()
	{
		this.usesCommonLock = this._usesCommonLock;
		this.canBeLocked = this.canBeLocked;
		this.maxClosingRetries = this._maxClosingRetries;
		this.initInsideUnlockedJammed = (this._initDoorLockInside == DoorLockState.UnlockedJam);
		this.initOutsideUnlockedJammed = (this._initDoorLockOutside == DoorLockState.UnlockedJam);

		this.isBlocked = this._initIsBlocked;
		this.currDoorState = this._initDoorState;
		this.currInsideLockState = this._initDoorLockInside;
		this.currOutsideLockState = this._initDoorLockOutside;

		// Sync animator bools to match initial state - this makes animator jump to correct idle states
		// WITHOUT playing transition animations (bools set initial layer states)
		this._animator.trySetBool(
			DoorAnimParamType.isDoorOpen,
			val: _initDoorState == DoorState.Opened);

		this._animator.trySetBool(
			DoorAnimParamType.isInsideLocked,
			val: _initDoorLockInside == DoorLockState.Locked);

		this._animator.trySetBool(
			DoorAnimParamType.isOutsideLocked,
			val: _initDoorLockOutside == DoorLockState.Locked);

		// Force animator to evaluate immediately so door appears in correct state on first frame
		this._animator.Update(0f);
	}

	/*
	// DoorInteraction.cs -> 
			TryPerform....() -> 
				* .animating = true // not required for now
				* doorState = performing(such as opening, closing etc)
				play animation --(animEventForwarder)--> 
					this.OnAnimationComplete() 
						* .animating = false // not required for now
						* set new door state
						* set bool based on new door state(if required)
	*/
	public void OnAnimationComplete(AnimationEventType eventType)
	{
		Debug.Log(C.method(this, "cyan", adMssg: $"{eventType}".colorTag("lime")));
		if (eventType == AnimationEventType.DoorOpeningComplete)
		{
			this.isAnimatingDoorPanel = false; // not required for now
			this.currDoorState = DoorState.Opened;
			#region isDoorOpen
			this._animator.trySetBool(DoorAnimParamType.isDoorOpen, val: currDoorState == DoorState.Opened); //disable init, useful later for swaying destination 
			#endregion
		}
		else if (eventType == AnimationEventType.DoorClosingComplete)
		{
			this.isAnimatingDoorPanel = false; // not required for now
			this.currDoorState = DoorState.Closed;
			#region isDoorOpen
			this._animator.trySetBool(DoorAnimParamType.isDoorOpen, val: currDoorState == DoorState.Opened); // disable init, useful later for swaying destination
			#endregion
		}

		else if(eventType == AnimationEventType.InsideLockingComplete)
		{
			this.currInsideLockState = DoorLockState.Locked;
			#region isInsideLocked
			this._animator.trySetBool(DoorAnimParamType.isInsideLocked, this.currInsideLockState == DoorLockState.Locked); // disable after init
			#endregion
		}
		else if (eventType == AnimationEventType.InsideUnlockingComplete)
		{
			this.currInsideLockState = DoorLockState.Unlocked;
			#region isInsideLocked
			this._animator.trySetBool(DoorAnimParamType.isInsideLocked, this.currInsideLockState == DoorLockState.Locked); // disable after init
			#endregion
		}

		else if (eventType == AnimationEventType.OutsideLockingComplete)
		{
			this.currOutsideLockState = DoorLockState.Locked;
			#region isOutsideLocked
			this._animator.trySetBool(DoorAnimParamType.isOutsideLocked, this.currOutsideLockState == DoorLockState.Locked); // disable after init
			#endregion
		}
		else if (eventType == AnimationEventType.OutsideUnlockingComplete)
		{
			this.currOutsideLockState = DoorLockState.Unlocked;
			#region isOutsideLocked
			this._animator.trySetBool(DoorAnimParamType.isOutsideLocked, this.currOutsideLockState == DoorLockState.Locked); // disable after init
			#endregion
		}
	}

	#region public API: Try Perform
	// flow: 
	// canont perform
	// success
	//	set state
	//	set trigger
	// canont perform

	public DoorActionResult TryOpen()
	{
		Debug.Log(C.method(this, "lime"));

		// cannot perform
		if (this.isBlocked)
		{
			this._animator.trySetTrigger(DoorAnimParamType.doorBlockedJiggle);
			return DoorActionResult.Blocked;
		}
		// success
		if (this.currDoorState == DoorState.Closed)
		{
			// cannot if locked
			if (this.currInsideLockState == DoorLockState.Locked || this.currOutsideLockState == DoorLockState.Locked) // in other words open works even in locking or unlocking process(animation process) i,e should be completely locked via  (OnAnimationComplete).
			{
				this._animator.trySetTrigger(DoorAnimParamType.doorLockedJiggle);
				return DoorActionResult.Locked;
			}
			//
			this.currDoorState = DoorState.Opening;
			this._animator.trySetTrigger(DoorAnimParamType.doorOpen); // set trigger
			return DoorActionResult.Success;
		}
		// cannot perform
		if (this.currDoorState == DoorState.Opened) return DoorActionResult.AlreadyInState;
		if (this.currDoorState == DoorState.Opening || this.currDoorState == DoorState.Closing || this.currDoorState == DoorState.Swaying) return DoorActionResult.AnimationInProgress;
		return DoorActionResult.Failure;
	}
	public DoorActionResult TryClose()
	{
		Debug.Log(C.method(this, "lime"));

		// cannot perform
		if (this.isBlocked)
			return DoorActionResult.Blocked;
		// success
		if (this.currDoorState == DoorState.Opened)
		{
			// try to unlock if locked -> door is closing is independedent of lock/unLock even if it s a not .success
			TryUnlock(LockSide.Inside);
			TryUnlock(LockSide.Outside);
			//
			this.currDoorState = DoorState.Closing;
			this._animator.trySetTrigger(DoorAnimParamType.doorClose); // set trigger
			return DoorActionResult.Success;
		}
		// cannot perform
		if (this.currDoorState == DoorState.Closed) return DoorActionResult.AlreadyInState;
		if (this.currDoorState == DoorState.Opening || this.currDoorState == DoorState.Closing || this.currDoorState == DoorState.Swaying) return DoorActionResult.AnimationInProgress;
		return DoorActionResult.Failure;
	}

	// TODO: TryLock, TryUnlock for common lock (which is set at very begining with .usesCommonLock), approach to handle currInsideLockState, currOutSideLockState ?
	public DoorActionResult TryLock(LockSide side)
	{
		Debug.Log(C.method(this, "lime"));

		#region commonLock
		if(this.usesCommonLock == true)
		{
			return performCommonLock();
		}
		#endregion

		if (side == LockSide.Inside)
		{
			// cannot perform
			if (this.initInsideUnlockedJammed == true)
				return DoorActionResult.UnlockedJam; // jammed
			// success
			if (this.currInsideLockState == DoorLockState.Unlocked)
			{
				this.currInsideLockState = DoorLockState.Locking;
				this._animator.trySetTrigger(DoorAnimParamType.lockInside); // set trigger
				return DoorActionResult.Success;
			}
			// cannot perform
			if (this.currInsideLockState == DoorLockState.Locked) return DoorActionResult.AlreadyInState;
			if (this.currInsideLockState == DoorLockState.Locking || this.currInsideLockState == DoorLockState.Unlocking) return DoorActionResult.AnimationInProgress;
			return DoorActionResult.Failure;
		}
		else if (side == LockSide.Outside)
		{
			// cannot perform
			if (this.initOutsideUnlockedJammed == true)
				return DoorActionResult.UnlockedJam; // jammed
			// success
			if (this.currOutsideLockState == DoorLockState.Unlocked)
			{
				this.currOutsideLockState = DoorLockState.Locking;
				this._animator.trySetTrigger(DoorAnimParamType.lockOutside); // set trigger
				return DoorActionResult.Success;
			}
			// cannot perform
			if (this.currOutsideLockState == DoorLockState.Locked) return DoorActionResult.AlreadyInState;
			if (this.currOutsideLockState == DoorLockState.Locking || this.currOutsideLockState == DoorLockState.Unlocking) return DoorActionResult.AnimationInProgress;
			return DoorActionResult.Failure;
		}
		return DoorActionResult.Failure;
	}
	#region ad
	DoorActionResult performCommonLock()
	{
		// doesnt depend on LockSide side.
		// since i shall jam one of the locks and hide it, when uses common lock is true
		// one of animation(say open lock) extends in both side to make appear as common lock(visible from both sides)

		// cannot perform
		bool allLocksUnlockedJammed = true;
		if (this.initInsideUnlockedJammed == false) allLocksUnlockedJammed = false;
		if (this.initOutsideUnlockedJammed == false) allLocksUnlockedJammed = false;
		if (allLocksUnlockedJammed) // both jammed
			return DoorActionResult.UnlockedJam;
		// success
		if (
			(this.currInsideLockState == DoorLockState.Unlocked || this.initInsideUnlockedJammed == true) &&
			(this.currOutsideLockState == DoorLockState.Unlocked || this.initInsideUnlockedJammed == true))
		{
			if (this.initInsideUnlockedJammed)
				this.currInsideLockState = DoorLockState.Locking;
			this._animator.trySetTrigger(DoorAnimParamType.lockInside); // set trigger
			this.currOutsideLockState = DoorLockState.Locking;

			// runs in parellel since both on different animation layers
			this._animator.trySetTrigger(DoorAnimParamType.lockInside); // set trigger
		}
		// cannot perform
		return DoorActionResult.Failure;
	}

	DoorActionResult performCommonUnLock()
	{

	}
	#endregion

	public DoorActionResult TryUnlock(LockSide side)
	{
		Debug.Log(C.method(this, "lime"));

		#region commonLock
		if (this.usesCommonLock == true)
		{
			// since i shall jam one of the locks and hide it, when uses common lock is true
			// one of animation(say open lock) extends in both side to make appear as common lock(visible from both sides)
			if (this.TryUnlock(LockSide.Inside) == DoorActionResult.Success || this.TryUnlock(LockSide.Outside) == DoorActionResult.Success)
				return DoorActionResult.Success;
			return DoorActionResult.Failure;
		}
		#endregion

		if (side == LockSide.Inside)
		{
			// cannot perform
			/*
			if (this.insideUnlockedJammed == true)
				return DoorActionResult.UnlockedJam;
			*/
			// success
			if (this.currInsideLockState == DoorLockState.Locked)
			{
				this.currInsideLockState = DoorLockState.Unlocking;
				this._animator.trySetTrigger(DoorAnimParamType.unlockInside); // set trigger
				return DoorActionResult.Success;
			}
			// cannot perform
			if (this.currInsideLockState == DoorLockState.Unlocked) return DoorActionResult.AlreadyInState;
			if (this.currInsideLockState == DoorLockState.Locking || this.currInsideLockState == DoorLockState.Unlocking) return DoorActionResult.AnimationInProgress;
			return DoorActionResult.Failure;
		}
		else if (side == LockSide.Outside)
		{
			// cannot perform
			/*
			if (this.outsideUnlockedJammed == true)
				return DoorActionResult.UnlockedJam;
			*/
			// success
			if (this.currOutsideLockState == DoorLockState.Locked)
			{
				this.currOutsideLockState = DoorLockState.Unlocking;
				this._animator.trySetTrigger(DoorAnimParamType.unlockOutside); // set trigger
				return DoorActionResult.Success;
			}
			// cannot perform
			if (this.currOutsideLockState == DoorLockState.Unlocked) return DoorActionResult.AlreadyInState;
			if (this.currOutsideLockState == DoorLockState.Locking || this.currOutsideLockState == DoorLockState.Unlocking) return DoorActionResult.AnimationInProgress;
			return DoorActionResult.Failure;
		}
		return DoorActionResult.Failure;
	}

	public DoorActionResult TryBlock()
	{
		Debug.Log(C.method(this, "lime"));
		this.isBlocked = true;
		return DoorActionResult.Blocked;
	}
	public DoorActionResult TryUnblock()
	{
		Debug.Log(C.method(this, "lime"));
		this.isBlocked = false;
		return DoorActionResult.Success;

	}

	public DoorActionResult TryStartSwaying()
	{
		Debug.Log(C.method(this, "lime"));
		return DoorActionResult.Success;
	}
	public DoorActionResult TryStopSwaying(DoorState targetState = DoorState.Opened)
	{
		Debug.Log(C.method(this, "lime"));
		return DoorActionResult.Success;
	}
	#endregion

	#region getStr
	public string getStr
	{
		get
		{
			string str = $@" getStr():
usesCommonLock: {usesCommonLock}
canBeLocked: {canBeLocked}
maxClosingRetries: {maxClosingRetries}
initInsideUnlockedJammed: {initInsideUnlockedJammed}
initOutsideUnlockedJammed: {initOutsideUnlockedJammed}

isBlocked: {isBlocked}
currDoorState: {currDoorState}
currInsideLockState: {currInsideLockState}
currOutsideLockState: {currOutsideLockState}

isAnimatingDoorPanel: {isAnimatingDoorPanel}
isAnimatingDoorLockInside: {isAnimatingDoorLockInside}
isAnimatingDoorLockOutside: {isAnimatingDoorLockOutside}
isAnimatingDoorLockCommon: {isAnimatingDoorLockCommon}

OnDoorStateChanged: {OnDoorStateChanged}
OnLockStateChanged: {OnLockStateChanged};";
			return str;
		}
	}
	#endregion

	#region Unity Life Cycle
	private void Start()
	{
		Debug.Log(C.method(this));
		this.Init();
	}
	private void Update()
	{
		this._getStr = this.getStr;
	}
	#endregion
}
