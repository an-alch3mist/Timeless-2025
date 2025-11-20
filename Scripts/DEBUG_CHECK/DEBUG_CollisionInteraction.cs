using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SPACE_UTIL;

namespace SPACE_CHECK
{
	public class DEBUG_CollisionInteraction : MonoBehaviour
	{
		public List<DEBUG_Door> DOOR_INTERACTABLE = new List<DEBUG_Door>();
		[SerializeField]List<GameObject> DOOR_OBJ = new List<GameObject>();

		private void Update()
		{
			if(INPUT.M.InstantDown(0))
			{
				// if door interactable count is not = 0
				// if ray cast hit it
			}
			/*
			// find the appropriate door based on ray cast
			if (INPUT.K.InstantDown(KeyCode.E))
				if (this.currDoor != null)
				{
					// TODO: also the ray cast with distance toward the door(from first person camera)
					this.currDoor.Interact(this.isInside);
				}
			*/
		}
	}
}
