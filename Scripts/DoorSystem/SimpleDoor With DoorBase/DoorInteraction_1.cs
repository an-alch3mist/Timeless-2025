using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SPACE_UTIL;

namespace SPACE_GAME_1
{
	public class DoorInteraction_1 : MonoBehaviour
	{
		[SerializeField] LayerMask _doorSideMask;
		[SerializeField] Camera _fpCam;
		[SerializeField] float _rayDist = 3f;

		// [SerializeField] string name = "wellletsseeinsidehmm", term = "inside";
		private void Update()
		{
			/*
			if(INPUT.M.InstantDown(0))
			{
				bool isMatch = this.name.anyMatch($@"{this.term}");
				Debug.Log(isMatch.ToString().colorTag("cyan"));
			}
			*/
			Ray ray = this._fpCam.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out RaycastHit hit, this._rayDist, this._doorSideMask) == true)
			{
				var doorBase = hit.transform.Q().upCompoGf<SimpleDoorHinged>();
				Debug.Log(doorBase);

				if (INPUT.K.InstantDown(KeyCode.E))
				{
					// Debug.Log("hit door mask".colorTag("cyan"));
					if (hit.transform.name.anyMatch(@"inside"))
						Debug.Log("inside".colorTag("cyan"));
					else if (hit.transform.name.anyMatch(@"outside"))
						Debug.Log("outside".colorTag(C.colorStr.aqua));
					
					if (doorBase.currDoorState == DoorState.Opened) doorBase.TryClose();
					else if (doorBase.currDoorState == DoorState.Closed) doorBase.TryOpen();
				}
				if (INPUT.K.InstantDown(KeyCode.L))
				{
					// Debug.Log("hit door mask".colorTag("cyan"));
					if (hit.transform.name.anyMatch(@"inside"))
					{
						Debug.Log("inside".colorTag("cyan"));
						if (doorBase.currInsideLockState == DoorLockState.Unlocked) doorBase.TryLock(LockSide.Inside);
						else if (doorBase.currInsideLockState == DoorLockState.Locked) doorBase.TryUnlock(LockSide.Inside);
					}
					else if (hit.transform.name.anyMatch(@"outside"))
					{
						Debug.Log("outside".colorTag(C.colorStr.aqua));
						if (doorBase.currOutsideLockState == DoorLockState.Unlocked) doorBase.TryLock(LockSide.Outside);
						else if (doorBase.currOutsideLockState == DoorLockState.Locked) doorBase.TryUnlock(LockSide.Outside);
					}
				}
			}
		}
	}
}