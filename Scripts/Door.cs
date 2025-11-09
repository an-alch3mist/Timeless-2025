using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SPACE_UTIL;

namespace SPACE_GAME
{
	public class Door : MonoBehaviour
	{
		[SerializeField] bool locked = false;
		bool isDoorOpen = false;
		public bool toggle()
		{
			Animator animator = this.GetComponent<Animator>();

			if (this.isDoorOpen == true)
			{
				if (this.locked)
				{
					animator.TrySetTrigger(AnimParamType.doorLocked);
					return this.isDoorOpen;
				}
				else
				{
					animator.TrySetTrigger(AnimParamType.doorOpen);
					this.isDoorOpen = !this.isDoorOpen;
					return this.isDoorOpen;
				}
			}
			else
			{
				animator.TrySetTrigger(AnimParamType.doorClose);
				this.isDoorOpen = !this.isDoorOpen;
				return this.isDoorOpen;
			}
		}

		public void LogCurrAnimState()
		{
			Animator animator = this.GetComponent<Animator>();
			var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
			Debug.Log($"Current State: {stateInfo.IsName("doorOpenAnim")} | Is Playing: {stateInfo.normalizedTime}".colorTag("cyan"));
		}
	}

}