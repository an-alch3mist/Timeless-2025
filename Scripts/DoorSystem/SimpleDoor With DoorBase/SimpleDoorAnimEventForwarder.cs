using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SPACE_UTIL;

namespace SPACE_GAME_1
{
	public class SimpleDoorAnimEventForwarder : MonoBehaviour
	{
		[SerializeField] DoorBase _doorBase;
		private void Awake()
		{
			Debug.Log(C.method(this));
		}

		public void AEOnDoorOpenComplete() => this._doorBase.OnAnimationComplete(AnimationEventType.DoorOpeningComplete);
		public void AEOnDoorCloseComplete() => this._doorBase.OnAnimationComplete(AnimationEventType.DoorClosingComplete);

		public void AEOnInsideLockComplete() => this._doorBase.OnAnimationComplete(AnimationEventType.InsideLockingComplete);
		public void AEOnInsideUnlockComplete() => this._doorBase.OnAnimationComplete(AnimationEventType.InsideUnlockingComplete);
		public void AEOnOutsideLockComplete() => this._doorBase.OnAnimationComplete(AnimationEventType.OutsideLockingComplete);
		public void AEOnOutsideUnlockComplete() => this._doorBase.OnAnimationComplete(AnimationEventType.OutsideUnlockingComplete);
	}
}