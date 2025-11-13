using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SPACE_UTIL;

public class SimpleDoorAnimEventForwarder : MonoBehaviour
{
	IDoor Idoor;
	private void Awake()
	{
		Debug.Log(C.method(this));
		this.Idoor = this.GetComponent<IDoor>();
		Debug.Log(this.Idoor);
	}

	public void AEOnDoorOpenComplete() => this.Idoor.OnAnimationComplete(AnimationEventType.DoorOpeningComplete);
	public void AEOnDoorCloseComplete() => this.Idoor.OnAnimationComplete(AnimationEventType.DoorClosingComplete);

	public void AEOnInsideLockComplete() => this.Idoor.OnAnimationComplete(AnimationEventType.InsideLockingComplete);
	public void AEOnInsideUnlockComplete() => this.Idoor.OnAnimationComplete(AnimationEventType.InsideUnlockingComplete);
	public void AEOnOutsideLockComplete() => this.Idoor.OnAnimationComplete(AnimationEventType.OutsideLockingComplete);
	public void AEOnOutsideUnlockComplete() => this.Idoor.OnAnimationComplete(AnimationEventType.OutsideUnlockingComplete);
}
