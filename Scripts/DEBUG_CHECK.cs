using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SPACE_UTIL;
using SPACE_DrawSystem;

using SPACE_GAME;

namespace SPACE_CHECK
{
	public class DEBUG_CHECK : MonoBehaviour
	{
		private void Start()
		{
			Debug.Log(C.method(this));
			this.checkAlAnimParamExist();
		}

		[SerializeField] Animator _animator;
		void checkAlAnimParamExist()
		{
			this._animator.checkAllParamExistInAnimatorController<DoorAnimParamType>();
			return;

			foreach (var doorParam in C.getEnumValues<DoorAnimParamType>())
			{
				if (this._animator.trySetTrigger(doorParam))
					Debug.Log(C.method(null, "cyan", adMssg: $"success setting boolean {doorParam}"));
				else if (this._animator.trySetBool(doorParam, true))
					Debug.Log(C.method(null, "cyan", adMssg: $"success setting boolean {doorParam}"));
			}
		}
	}
}
