using UnityEngine;
using System.Collections;
using SPACE_UTIL;

public class SimpleDoorInteraction : MonoBehaviour
{
	[SerializeField] SimpleDoorHinged _simpleDoorHinged;
	[SerializeField] KeyCode _keyCodeDoorOpen = KeyCode.O, _keyCodeDoorClose = KeyCode.C;
	[SerializeField] KeyCode _keyCodeLockInside = KeyCode.L, _keyCodeUnlockInside = KeyCode.U;
	[SerializeField] KeyCode _keyCodeLockOutside = KeyCode.Alpha1, _keyCodeUnlockOutside = KeyCode.Alpha0;

	private void Update()
	{
		if (INPUT.K.InstantDown(this._keyCodeDoorOpen))
		{
			var result =  this._simpleDoorHinged.TryOpen();
			Debug.Log(result.ToString().colorTag("grey"));
		}
		if (INPUT.K.InstantDown(this._keyCodeDoorClose))
		{
			var result = this._simpleDoorHinged.TryClose();
			Debug.Log(result.ToString().colorTag("grey"));
		}

		if(INPUT.K.InstantDown(this._keyCodeLockInside))
		{
			var result = this._simpleDoorHinged.TryLock(LockSide.Inside);
			Debug.Log(result.ToString().colorTag("grey"));
		}
		if (INPUT.K.InstantDown(this._keyCodeUnlockInside))
		{
			var result = this._simpleDoorHinged.TryUnlock(LockSide.Inside);
			Debug.Log(result.ToString().colorTag("grey"));
		}

		if (INPUT.K.InstantDown(this._keyCodeLockOutside))
		{
			var result = this._simpleDoorHinged.TryLock(LockSide.Outside);
			Debug.Log(result.ToString().colorTag("grey"));
		}
		if (INPUT.K.InstantDown(this._keyCodeUnlockOutside))
		{
			var result = this._simpleDoorHinged.TryUnlock(LockSide.Outside);
			Debug.Log(result.ToString().colorTag("grey"));
		}
	}

}
