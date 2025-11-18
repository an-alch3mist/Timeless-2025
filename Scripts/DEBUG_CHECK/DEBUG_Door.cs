using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SPACE_UTIL;

namespace SPACE_CHECK
{
	public class DEBUG_Door : MonoBehaviour
	{
		public void Interact(bool isInsideLock)
		{
			Debug.Log(C.method(this, "lime", adMssg: (isInsideLock) ? "inside lock interact" : "outside lock interact"));
		}
	}
}