using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using SPACE_UTIL;

namespace SPACE_GAME
{
	/*
		load at begining scene,
		save can be made anytime
	*/
	[DefaultExecutionOrder(-50)] // just after UnityEngine.InputSystem Init
	public class GameStore : MonoBehaviour
	{
		[SerializeField] InputActionAsset _IA;
		public static InputActionAsset IA;
		public static PlayerStats playerStats;

		private void Awake()
		{
			Debug.Log(C.method(this, "white"));
			GameStore.IA = this._IA;
			GameStore.LoadAllSavedGameDataJsonToField();
		}
		static void LoadAllSavedGameDataJsonToField()
		{
			GameStore.IA.LoadBindingOverridesFromJson(LOG.LoadGameData(GameDataType.inputActionAsset));
			GameStore.playerStats = LOG.LoadGameData<PlayerStats>(GameDataType.playerStats);
		}

		#region check usage(with playerStats as example)
		private float currGameTime = 0f; // counter: reset at start of game
		private void Update()
		{
			currGameTime += Time.unscaledDeltaTime;
		}

		private void OnApplicationQuit()
		{
			Debug.Log(C.method(this, "orange"));
			GameStore.playerStats.gameTime += currGameTime;
			GameStore.playerStats.HISTORY.Add(currGameTime);
			GameStore.playerStats.Save();
		}
		#endregion
	}


	// ================ GLOBAL CLASS ============== //
	[System.Serializable]
	public class PlayerStats
	{
		public float gameTime = 0f;
		public List<float> HISTORY = new List<float>();
		public void Save()
		{
			LOG.SaveGameData(GameDataType.playerStats, this.ToJson());
		}
	}

	// ================ GLOBAL CLASS ============== //

	// ========== GLOBAL ENUM ============= //
	public enum GameDataType
	{
		inputActionAsset,
		playerStats,
		doorRegeistryData, // Added for door system save/load
	}

	
}

/*
```

---

## Animator Controller Flow

### **Approach 1: Layered State Machine (Recommended)**

** Why Layers?** Locks and door movement are independent — player can lock/unlock without opening door.
 ```
═══════════════════════════════════════════════════════════
LAYER 0: Door Movement (Weight: 1.0)
═══════════════════════════════════════════════════════════

Parameters:
  doorOpen(Trigger)
  doorClose(Trigger)
  doorBlockedJiggle(Trigger)
  isDoorOpen(Bool) — synced from DoorState

States:
├─ Entry → doorClosedIdle(default)
│
├─ doorClosedIdle
│  ├─ [doorOpen trigger] → doorOpeningAnim
│  └─ [doorBlockedJiggle trigger] → doorBlockedClosedJiggle → doorClosedIdle
│
├─ doorOpeningAnim (exitTime: 1.0) → doorOpenedIdle
│
├─ doorOpenedIdle
│  ├─ [doorClose trigger] → doorClosingAnim
│  └─ [doorBlockedJiggle trigger] → doorBlockedOpenedJiggle → doorOpenedIdle
│
└─ doorClosingAnim (exitTime: 1.0) → doorClosedIdle

═══════════════════════════════════════════════════════════
LAYER 1: Inside Lock (Weight: 1.0, Additive Blending)
═══════════════════════════════════════════════════════════

Parameters:
  lockInside (Trigger)
  unlockInside (Trigger)
  isInsideLocked (Bool) — synced from DoorLockStateInside

States:
├─ Entry → insideUnlockedIdle (default)
│
├─ insideUnlockedIdle
│  └─ [lockInside trigger] → insideLockingAnim → insideLockedIdle
│
└─ insideLockedIdle
   └─ [unlockInside trigger] → insideUnlockingAnim → insideUnlockedIdle

═══════════════════════════════════════════════════════════
LAYER 2: Outside Lock (Weight: 1.0, Additive Blending)
═══════════════════════════════════════════════════════════

Parameters:
  lockOutside (Trigger)
  unlockOutside (Trigger)
  isOutsideLocked (Bool) — synced from DoorLockStateOutside

States:
├─ Entry → outsideUnlockedIdle (default)
│
├─ outsideUnlockedIdle
│  └─ [lockOutside trigger] → outsideLockingAnim → outsideLockedIdle
│
└─ outsideLockedIdle
   └─ [unlockOutside trigger] → outsideUnlockingAnim → outsideUnlockedIdle

═══════════════════════════════════════════════════════════
LAYER 3: Common Lock (Weight: 1.0, Additive) — OPTIONAL
═══════════════════════════════════════════════════════════
// Only use this layer if usesCommonLock == true
// Disable Layers 1 & 2 when using common lock

Parameters:
  lockCommon (Trigger)
  unlockCommon (Trigger)

States:
├─ Entry → commonUnlockedIdle (default)
│
├─ commonUnlockedIdle
│  └─ [lockCommon trigger] → commonLockingAnim → commonLockedIdle
│
└─ commonLockedIdle
   └─ [unlockCommon trigger] → commonUnlockingAnim → commonUnlockedIdle
```

---

## Why Layered Approach?

✅ **Independent animations**: Lock handle rotates while door stays closed  
✅ **No state explosion**: 2 states per layer instead of 8 combined states  
✅ **Easy to extend**: Add swaying/blocking as Layer 4  
✅ **Additive blending**: Lock animations don't interfere with door movement  

---

## Animation Hierarchy Mapping

Based on your hierarchy:
```
door_hinged (Animator attached here)
└─ door (animated pivot)
   ├─ collider/trigger
   ├─ door panel (animated Y-rotation for opening/closing)
   │  └─ visual (mesh)
   ├─ doorhandleOutside (animated Z-rotation for lock/unlock)
   │  └─ visual (handle mesh)
   ├─ doorhandleInside (animated Z-rotation for lock/unlock)
   │  └─ visual (handle mesh)
   └─ doorFrame (static)
      └─ visual (frame mesh)
	// ========== GLOBAL ENUM ============= //
*/
