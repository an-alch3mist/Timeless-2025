using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SPACE_UTIL;

namespace SPACE_GAME_0
{
	public class SimpleDoorAnimEventForwarder : MonoBehaviour
	{
		IDoor idoor;
		private void Awake()
		{
			Debug.Log(C.method(this));
			// var ref = transform.GC<IDoor>(); // wont work for interface
			this.idoor = this.GetComponent<IDoor>();
			if (this.idoor == null)
				Debug.Log(C.method(this, "red", adMssg: $"found no IDoor compoenent type attached to {this.gameObject.name}"));
		}

		public void AEOnDoorOpenComplete() => this.idoor.OnAnimationComplete(AnimationEventType.DoorOpeningComplete);
		public void AEOnDoorCloseComplete() => this.idoor.OnAnimationComplete(AnimationEventType.DoorClosingComplete);

		public void AEOnInsideLockComplete() => this.idoor.OnAnimationComplete(AnimationEventType.InsideLockingComplete);
		public void AEOnInsideUnlockComplete() => this.idoor.OnAnimationComplete(AnimationEventType.InsideUnlockingComplete);
		public void AEOnOutsideLockComplete() => this.idoor.OnAnimationComplete(AnimationEventType.OutsideLockingComplete);
		public void AEOnOutsideUnlockComplete() => this.idoor.OnAnimationComplete(AnimationEventType.OutsideUnlockingComplete);
	}
}