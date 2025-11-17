using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SPACE_UTIL;

namespace SPACE_CHECK
{
	public class DEBUG_CollisionTrigger : MonoBehaviour
	{
		[SerializeField] bool insideType = false;
		[SerializeField] DEBUG_CHECK debugCheck;
		private void OnTriggerEnter(Collider other)
		{
			Debug.Log(C.method(this, "cyan", adMssg: this.getFullPath()));
			debugCheck.doorName = this.getFullPath();
			debugCheck.playerInsideOrOutsideorNone = (this.insideType)? "inside": "outside";
		}
		private void OnTriggerExit(Collider other)
		{
			Debug.Log(C.method(this, "cyan", adMssg: this.getFullPath()));
			debugCheck.doorName = "";
			debugCheck.playerInsideOrOutsideorNone = "none";
		}
		/*
		private void OnTriggerStay(Collider other)
		{
			Debug.Log(C.method(this, "cyan"));
		}
		*/
	} 
}
