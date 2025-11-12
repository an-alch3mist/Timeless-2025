using UnityEngine;
using System.Collections;
using SPACE_UTIL;

public class SimpleDoorInteraction : MonoBehaviour
{
	[SerializeField] DoorHinged _doorHinged;
	[TextArea(8, 10)]
	[SerializeField] string _console = @"";


	private void Update()
	{
		if (INPUT.K.InstantDown(KeyCode.O))
		{
			var result =  this._doorHinged.TryOpen();
			Debug.Log(result.ToString().colorTag("cyan"));
		}
		if (INPUT.K.InstantDown(KeyCode.C))
		{
			var result = this._doorHinged.TryClose();
			Debug.Log(result.ToString().colorTag("cyan"));
		}

		this.logDoor();
	}


	void logDoor()
	{
		/*
			{this._doorHinged.IsBlocked}
			{this._doorHinged.UsesCommonLock}
			{this._doorHinged.CanBeLocked}
			{this._doorHinged.IsAnimating}
			{this._doorHinged.CurrentState}
			{this._doorHinged.InsideLockState}
			{this._doorHinged.OutsideLockState}
		*/
		this._console = $@"==== door fields: ====
IsAnimating: {this._doorHinged.IsAnimatingDoorPanel}
IsBlocked: {this._doorHinged.IsBlocked}
UsesCommonLock: {this._doorHinged.UsesCommonLock}
CanBeLocked: {this._doorHinged.CanBeLocked}
currDoorState: {this._doorHinged.currDoorState}
InsideLockState: {this._doorHinged.InsideLockState}
OutsideLockState: {this._doorHinged.OutsideLockState}";
	}

}
