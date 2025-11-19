using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SPACE_UTIL;

namespace SPACE_CHECK
{
	public class DEBUG_CollisionTrigger : MonoBehaviour
	{
		[SerializeField] bool isInside = false;

		private void OnTriggerEnter(Collider other)
		{
			Debug.Log(C.method(this, "cyan", adMssg: other.getFullPath()));
			if(other.tag == TagType.player.ToString())
			{
				/*
				var interaction = other.Q()
					.up<DEBUG_CollisionInteraction>().gf()
					.gc<DEBUG_CollisionInteraction>();

				interaction.currDoor = this.Q().up<DEBUG_Door>().gf().gc<DEBUG_Door>();
				interaction.isInside = this.isInside;
				*/

				// the root of collider -> where the collision interaction(player gameObject root) is attached to
				other.Q().upCompoGf<DEBUG_CollisionInteraction>().currDoor /* equivalent to .up<T>.gf<T>() */ = this.Q().upCompoGf<DEBUG_Door>();
				other.Q().upCompoGf<DEBUG_CollisionInteraction>().isInside = this.isInside;
			}
		}
		private void OnTriggerExit(Collider other)
		{
			Debug.Log(C.method(this, "cyan", adMssg: other.getFullPath()));
			if (other.tag == TagType.player.ToString())
			{
				/*
				var interaction = other.Q()
					.up<DEBUG_CollisionInteraction>().gf()
					.gc<DEBUG_CollisionInteraction>();

				interaction.currDoor = null;
				interaction.isInside = this.isInside;
				*/

				other.Q().upCompoGf<DEBUG_CollisionInteraction>().currDoor = null;
			}
		}
		/*
		private void OnTriggerStay(Collider other)
		{
			Debug.Log(C.method(this, "cyan"));
		}
		*/
	} 
}
