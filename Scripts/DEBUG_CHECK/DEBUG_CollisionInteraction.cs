using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SPACE_UTIL;

namespace SPACE_CHECK
{
	public class DEBUG_CollisionInteraction : MonoBehaviour
	{
		public DEBUG_Door currDoor = null;
		public bool isInside = false;

		private void Update()
		{
			if(INPUT.K.InstantDown(KeyCode.E))
				if (this.currDoor != null)
				{
					// TODO: also the ray cast with distance toward the door(from first person camera)
					this.currDoor.Interact(this.isInside);
				}
		}
	} 
}
