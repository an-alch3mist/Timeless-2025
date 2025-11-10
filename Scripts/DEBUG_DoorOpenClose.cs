using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SPACE_UTIL;
using SPACE_GAME;

namespace SPACE_CHECK
{
	public class DEBUG_DoorOpenClose : MonoBehaviour
	{
		[SerializeField] Door_0 door;
		private void Update()
		{
			if(INPUT.M.InstantDown(0))
			{
				StopAllCoroutines();
				StartCoroutine(STIMULATE());
			}
			if(INPUT.M.InstantDown(1))
			{
				this.door.LogCurrAnimState();
			}
		}

		IEnumerator STIMULATE()
		{
			Debug.Log(C.method(null, "cyan"));
			door.toggle();
			yield return null;
		}
	}
}