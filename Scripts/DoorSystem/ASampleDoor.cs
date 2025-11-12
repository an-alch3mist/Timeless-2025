using UnityEngine;
using System.Collections;
using System;

using SPACE_UTIL;

namespace SPACE_GAME
{
	public class ASampleDoor : MonoBehaviour, IDoor
	{
		public bool IsBlocked => throw new NotImplementedException();
		public bool UsesCommonLock => throw new NotImplementedException();
		public bool CanBeLocked => throw new NotImplementedException();
		public bool IsAnimatingDoorPanel => throw new NotImplementedException();

		public DoorState currDoorState => throw new NotImplementedException();
		public DoorLockState InsideLockState => throw new NotImplementedException();
		public DoorLockState OutsideLockState => throw new NotImplementedException();

		public int MaxCloseRetries { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public event Action<DoorState> OnDoorStateChanged;
		public event Action<DoorLockState, LockSide> OnLockStateChanged;

		private void Awake()
		{
			Debug.Log(C.method(this));
			this.Init();
		}

		public void Init()
		{
			throw new NotImplementedException();
		}

		public void OnAnimationComplete(AnimationEventType eventType)
		{
			throw new NotImplementedException();
		}

		public DoorActionResult TryBlock()
		{
			throw new NotImplementedException();
		}

		public DoorActionResult TryClose()
		{
			throw new NotImplementedException();
		}

		public DoorActionResult TryLock(LockSide side)
		{
			return DoorActionResult.Locked;
		}

		public DoorActionResult TryOpen()
		{
			throw new NotImplementedException();
		}

		public DoorActionResult TryStartSwaying()
		{
			throw new NotImplementedException();
		}

		public DoorActionResult TryStopSwaying(DoorState targetState = DoorState.Opened)
		{
			throw new NotImplementedException();
		}

		public DoorActionResult TryUnblock()
		{
			throw new NotImplementedException();
		}

		public DoorActionResult TryUnlock(LockSide side)
		{
			throw new NotImplementedException();
		}
	}

}