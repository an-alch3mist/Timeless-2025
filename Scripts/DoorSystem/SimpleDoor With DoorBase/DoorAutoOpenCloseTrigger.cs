using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SPACE_UTIL;

namespace SPACE_GAME_1
{
	public class DoorAutoOpenCloseTrigger : MonoBehaviour
	{
		private void OnTriggerEnter(Collider other)
		{
			Debug.Log(C.method(this, "cyan"));
			if (other.gameObject.name.anyMatch(@"player")) // player 
			{
				DoorBase doorBase = this.gameObject.Q().upCompoGf<DoorBase>();
				Debug.Log(doorBase);
				doorBase.TryOpen();
			}
		}
		private void OnTriggerExit(Collider other)
		{
			Debug.Log(C.method(this, "cyan"));
			if (other.gameObject.name.anyMatch(@"player")) // player
			{
				DoorBase doorBase = this.gameObject.Q().upCompoGf<DoorBase>();
				Debug.Log(doorBase);
				doorBase.TryClose();
			}

		}
	}
}
