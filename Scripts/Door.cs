using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SPACE_UTIL;

namespace SPACE_GAME
{
	public class Door : MonoBehaviour
	{
		bool isDoorOpen = false;
		public bool toggle()
		{
			this.isDoorOpen = !this.isDoorOpen;
			Animator animator = this.GetComponent<Animator>();

			if(this.isDoorOpen == true) animator.TrySetTrigger(AnimParamType.doorOpen);
			if(this.isDoorOpen == false) animator.TrySetTrigger(AnimParamType.doorClose);
			return this.isDoorOpen;
		}

		public void LogCurrAnimState()
		{
			Animator animator = this.GetComponent<Animator>();
			var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
			Debug.Log($"Current State: {stateInfo.IsName("doorOpenAnim")} | Is Playing: {stateInfo.normalizedTime}".colorTag("cyan"));
		}
	}

}