using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SPACE_UTIL;
using SPACE_GAME;

namespace SPACE_CHECK
{
	public class Door_0 : MonoBehaviour
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
					animator.trySetTrigger(AnimParamType.doorLockedJiggle);
					return this.isDoorOpen;
				}
				else
				{
					animator.trySetTrigger(AnimParamType.doorOpen);
					this.isDoorOpen = !this.isDoorOpen;
					return this.isDoorOpen;
				}
			}
			else
			{
				animator.trySetTrigger(AnimParamType.doorClose);
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