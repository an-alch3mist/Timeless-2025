using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SPACE_UTIL;

namespace SPACE_CHECK
{
	public class DEBUG_CollisionTrigger : MonoBehaviour
	{
		private void OnTriggerEnter(Collider other)
		{
			Debug.Log(C.method(this, "cyan", adMssg: this.getFullPath()));
		}
		private void OnTriggerExit(Collider other)
		{
			Debug.Log(C.method(this, "cyan", adMssg: this.getFullPath()));
		}
		/*
		private void OnTriggerStay(Collider other)
		{
			Debug.Log(C.method(this, "cyan"));
		}
		*/
	} 
}
