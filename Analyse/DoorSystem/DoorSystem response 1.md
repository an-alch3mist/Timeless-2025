# First Person Door System - Complete Implementation Guide

## 📁 File Structure (3 Files)

```
SPACE_GAME/
├── DoorCore.cs              // Interfaces, enums, events, data
├── Door.cs                  // Main door MonoBehaviour
└── DoorInteractionZone.cs   // Trigger zones for player detection
```

---

## 🏗️ Architecture Overview

### Design Principles
- **Minimal Files**: Only 3 files total (2 MonoBehaviours, 1 core)
- **Interface-Driven**: `IDoor` interface for polymorphism
- **Event-Based**: UnityEvents for gameplay integration
- **Save/Load Ready**: Serializable state management via UTIL.cs LOG system
- **Animation-Driven**: Layered animator approach for independent lock/door animations
- **Supernatural Support**: Block, unblock, and swaying mechanics

### Key Features
✅ Dual-sided locks (inside/outside) OR common lock (single handle)  
✅ Blocking by supernatural entities  
✅ Door swaying effect  
✅ Animation event support for precise timing  
✅ Player side detection (inside/outside)  
✅ Locked/blocked feedback (jiggle animations)  
✅ Full save/load integration  
✅ Debug visualization with DrawManager.cs  
---

## 🎯 Hierarchy Setup

```
door_hinged (Animator attached here)
└─ door (empty, pivot at bottom-right corner from outside)
   ├─ collider
   ├─ interactionTriggerOutside (DoorInteractionZone, BoxCollider isTrigger=true)
   ├─ interactionTriggerInside (DoorInteractionZone, BoxCollider isTrigger=true)
   ├─ doorPanel (empty, animated for rotation)
   │  └─ visual (MeshRenderer)
   ├─ doorhandleOutside (animated for lock/unlock)
   │  └─ visual (MeshRenderer)
   └─ doorhandleInside (animated for lock/unlock)
      └─ visual (MeshRenderer)
doorFrame (static, visual only)
   ├─ collider
   └─ visual (MeshRenderer)
```

### Component Assignment
1. **door_hinged**: Attach `Door.cs` + `Animator`
2. **interactionTriggerOutside**: Attach `DoorInteractionZone.cs` (set side=outside)
3. **interactionTriggerInside**: Attach `DoorInteractionZone.cs` (set side=inside)

---

## 🎬 Animator Controller Setup

### Layered State Machine Approach

```
═══════════════════════════════════════════════════════════
LAYER 0: Door Movement (Weight: 1.0, Override Blending)
═══════════════════════════════════════════════════════════

Parameters:
  doorOpen (Trigger)
  doorClose (Trigger)
  doorBlockedJiggle (Trigger)
  doorLockedJiggle (Trigger)
  isDoorOpen (Bool) — synced from code

States:
├─ doorClosedIdle (default)
│  ├─ [doorOpen trigger] → doorOpeningAnim
│  ├─ [doorBlockedJiggle trigger] → doorBlockedClosedJiggle → doorClosedIdle
│  └─ [doorLockedJiggle trigger] → doorLockedClosedJiggle → doorClosedIdle
│
├─ doorOpeningAnim (exitTime: 1.0) → doorOpenedIdle
│  └─ Animation Event: OnAnimationComplete() at last keyframe
│
├─ doorOpenedIdle
│  ├─ [doorClose trigger] → doorClosingAnim
│  └─ [doorBlockedJiggle trigger] → doorBlockedOpenedJiggle → doorOpenedIdle
│
└─ doorClosingAnim (exitTime: 1.0) → doorClosedIdle
   └─ Animation Event: OnAnimationComplete() at last keyframe

═══════════════════════════════════════════════════════════
LAYER 1: Inside Lock (Weight: 1.0, Additive Blending)
═══════════════════════════════════════════════════════════

Parameters:
  lockInside (Trigger)
  unlockInside (Trigger)
  isInsideLocked (Bool) — synced from code

States:
├─ insideUnlockedIdle (default)
│  └─ [lockInside trigger] → insideLockingAnim → insideLockedIdle
│     └─ Animation Event: OnAnimationComplete() at last keyframe
│
└─ insideLockedIdle
   └─ [unlockInside trigger] → insideUnlockingAnim → insideUnlockedIdle
      └─ Animation Event: OnAnimationComplete() at last keyframe

═══════════════════════════════════════════════════════════
LAYER 2: Outside Lock (Weight: 1.0, Additive Blending)
═══════════════════════════════════════════════════════════

Parameters:
  lockOutside (Trigger)
  unlockOutside (Trigger)
  isOutsideLocked (Bool) — synced from code

States:
├─ outsideUnlockedIdle (default)
│  └─ [lockOutside trigger] → outsideLockingAnim → outsideLockedIdle
│     └─ Animation Event: OnAnimationComplete() at last keyframe
│
└─ outsideLockedIdle
   └─ [unlockOutside trigger] → outsideUnlockingAnim → outsideUnlockedIdle
      └─ Animation Event: OnAnimationComplete() at last keyframe

═══════════════════════════════════════════════════════════
LAYER 3: Common Lock (Weight: 1.0, Additive) — OPTIONAL
═══════════════════════════════════════════════════════════
// Only enable if usesCommonLock == true
// Disable Layers 1 & 2 when using common lock

Parameters:
  lockCommon (Trigger)
  unlockCommon (Trigger)

States:
├─ commonUnlockedIdle (default)
│  └─ [lockCommon trigger] → commonLockingAnim → commonLockedIdle
│     └─ Animation Event: OnAnimationComplete() at last keyframe
│
└─ commonLockedIdle
   └─ [unlockCommon trigger] → commonUnlockingAnim → commonUnlockedIdle
      └─ Animation Event: OnAnimationComplete() at last keyframe
```

### Why Layered Approach?
- ✅ **Independent Animations**: Lock handle rotates while door stays closed
- ✅ **No State Explosion**: 2 states per layer instead of 8+ combined states
- ✅ **Easy to Extend**: Add swaying/blocking as Layer 4
- ✅ **Additive Blending**: Lock animations blend with door movement

### Animation Clips Needed

**Layer 0 (Door Movement)**:
- `doorOpeningAnim` — door panel rotates 90° (or swing open)
- `doorOpenedIdle` — door stays open
- `doorClosingAnim` — door panel rotates back to 0°
- `doorClosedIdle` — door stays closed
- `doorBlockedClosedJiggle` — slight shake when blocked (closed)
- `doorBlockedOpenedJiggle` — slight shake when blocked (opened)
- `doorLockedClosedJiggle` — slight shake when locked

**Layer 1 (Inside Lock)**:
- `insideLockingAnim` — inside handle rotates (e.g., -45° Z-rotation)
- `insideLockedIdle` — handle stays locked
- `insideUnlockingAnim` — inside handle rotates back to 0°
- `insideUnlockedIdle` — handle stays unlocked

**Layer 2 (Outside Lock)**:
- `outsideLockingAnim` — outside handle rotates
- `outsideLockedIdle` — handle stays locked
- `outsideUnlockingAnim` — outside handle rotates back
- `outsideUnlockedIdle` — handle stays unlocked

**Layer 3 (Common Lock)** — Optional:
- `commonLockingAnim` — single handle locks (both handles if needed)
- `commonLockedIdle` — locked state
- `commonUnlockingAnim` — unlocks
- `commonUnlockedIdle` — unlocked state

---

## 🎮 Usage Examples

### Basic Door Interaction (via Player Script)

```csharp
using UnityEngine;
using SPACE_GAME;
using SPACE_UTIL;

public class PlayerDoorInteraction : MonoBehaviour
{
    [SerializeField] KeyCode interactKey = KeyCode.E;
    private Door currentDoor;
    private DoorSide? currentSide;
    
    void Update()
    {
        // Raycast to find door
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            Door door = hit.collider.GetComponentInParent<Door>();
            if (door != null)
            {
                currentDoor = door;
                
                // Determine player side (check interaction zones)
                // This is automatically handled by DoorInteractionZone
            }
        }
        
        // Interact
        if (INPUT.K.InstantDown(interactKey) && currentDoor != null)
        {
            if (currentDoor.doorState == DoorState.closed)
            {
                currentDoor.TryOpen(DoorSide.inside); // or detect from zone
            }
            else if (currentDoor.doorState == DoorState.opened)
            {
                currentDoor.TryClose();
            }
        }
    }
}
```

### Lock/Unlock from UI

```csharp
using UnityEngine;
using UnityEngine.UI;
using SPACE_GAME;

public class DoorLockUI : MonoBehaviour
{
    [SerializeField] Door door;
    [SerializeField] Button lockInsideBtn;
    [SerializeField] Button unlockInsideBtn;
    
    void Start()
    {
        lockInsideBtn.onClick.AddListener(() => door.TryLockInside());
        unlockInsideBtn.onClick.AddListener(() => door.TryUnlockInside());
    }
}
```

### Supernatural Door Control (Horror Game)

```csharp
using System.Collections;
using UnityEngine;
using SPACE_GAME;

public class SupernaturalDoorController : MonoBehaviour
{
    [SerializeField] Door targetDoor;
    
    public void StartHorrorSequence()
    {
        StartCoroutine(HorrorSequence());
    }
    
    IEnumerator HorrorSequence()
    {
        // Block the door
        targetDoor.TryBlock();
        yield return new WaitForSeconds(2f);
        
        // Start swaying
        targetDoor.TryDoorSwaying();
        yield return new WaitForSeconds(5f);
        
        // Stop swaying and open door
        targetDoor.TryDoorStopSwaying(DoorState.opened);
        yield return new WaitForSeconds(1f);
        
        // Unblock
        targetDoor.TryUnblock();
    }
}
```

### Event Subscription (Global Manager)

```csharp
using UnityEngine;
using SPACE_GAME;
using SPACE_UTIL;

public class DoorEventManager : MonoBehaviour
{
    void Awake()
    {
        DoorEvents.OnDoorStateChanged.AddListener(OnDoorStateChanged);
        DoorEvents.OnDoorBlocked.AddListener(OnDoorBlocked);
        DoorEvents.OnDoorLockedJiggle.AddListener(OnDoorLockedJiggle);
    }
    
    void OnDoorStateChanged(string doorId, DoorState newState)
    {
        Debug.Log(C.method(this, "cyan", $"Door {doorId} -> {newState}"));
        
        // Play audio
        if (newState == DoorState.opened)
        {
            AudioClip clip = R.get<AudioClip>(ResourceType.audio__doorOpen);
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
    }
    
    void OnDoorBlocked(string doorId)
    {
        Debug.Log(C.method(this, "red", $"Door {doorId} is BLOCKED!"));
    }
    
    void OnDoorLockedJiggle(string doorId)
    {
        Debug.Log(C.method(this, "yellow", $"Door {doorId} is locked!"));
    }
}
```

---

## 💾 Save/Load System

### Automatic State Persistence

The system automatically saves door state when:
- Lock state changes
- Door opens/closes
- Door gets blocked/unblocked
- Swaying starts/stops

### Manual Save/Load

```csharp
// Save single door
door.SaveState(); // Called automatically, but can be manual

// Load state on game start (automatic in Door.Start())

// Save all doors in scene
DoorRegistry registry = new DoorRegistry();
foreach (Door door in FindObjectsOfType<Door>())
{
    registry.doors.Add(new DoorData(
        door.doorId,
        door.doorState,
        door.doorLockStateInside,
        door.doorLockStateOutside,
        door.blocked
    ));
}
registry.Save();

// Load all doors
DoorRegistry loaded = DoorRegistry.Load();
```

---

## 🐛 Debug Features

### Inspector Options
- **Show Debug Logs**: Detailed console logging
- **Draw Debug Lines**: Visual lines using DrawManager.cs
- **Use Animation Events**: Toggle between event-based and time-based animation completion

### Debug Visualization
When `drawDebugLines` is enabled:
- White line = door position
- Red line = door is blocked

### Debug Commands

```csharp
// Force state change
door._doorState = DoorState.opened; // Only for testing!
door.SyncAnimatorBools();

// Check current animation
AnimatorStateInfo stateInfo = door.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
Debug.Log($"Current State: {stateInfo.IsName("doorOpeningAnim")}");
```

---

## 🔧 Troubleshooting

### Door Not Opening
1. Check animator parameters exist (doorOpen trigger)
2. Verify door is unlocked (both inside and outside)
3. Ensure door is not blocked
4. Check `isAnimating` is false

### Locks Not Working
1. Verify animator layers 1, 2 (or 3) are set up
2. Check `usesCommonLock` setting matches animator setup
3. Ensure triggers (lockInside, unlockInside, etc.) exist

### Animation Events Not Firing
1. Add `OnAnimationComplete()` to animation clip as Animation Event
2. Place event at last keyframe of animation
3. Ensure `useAnimationEvents` is enabled in inspector
4. Check Door component is attached to same GameObject as Animator

### Player Detection Not Working
1. Verify Player has "Player" tag
2. Check DoorInteractionZone colliders have `isTrigger = true`
3. Ensure player has Rigidbody or CharacterController
4. Check zone colliders have proper size/position

---

## 📋 Checklist for New Door

- [ ] Create hierarchy with door_hinged root
- [ ] Add Animator component to door_hinged
- [ ] Attach Door.cs to door_hinged
- [ ] Create animator controller with 4 layers
- [ ] Add all animation clips to animator
- [ ] Add DoorInteractionZone components to trigger zones
- [ ] Set trigger colliders on interaction zones
- [ ] Add Animation Events to animation clips (OnAnimationComplete)
- [ ] Assign doorId in Door component
- [ ] Configure usesCommonLock setting
- [ ] Test lock/unlock/open/close in Play mode

---

## 🎨 Animation Tips

### Door Opening Animation (90° swing)
```
Keyframes for doorPanel transform:
Frame 0:   Rotation Y = 0°
Frame 30:  Rotation Y = 90°
```

### Lock Animation (handle rotation)
```
Keyframes for doorhandleInside transform:
Frame 0:   Rotation Z = 0° (unlocked)
Frame 10:  Rotation Z = -45° (locked)
```

### Jiggle Animation (feedback)
```
Keyframes for door transform:
Frame 0:   Position X = 0
Frame 5:   Position X = 0.05
Frame 10:  Position X = -0.05
Frame 15:  Position X = 0
```

---

## 🚀 Advanced Features

### Custom Lock Logic

```csharp
public class KeycardDoor : Door
{
    [SerializeField] string requiredKeycard = "keycard_red";
    
    public override bool TryOpen(DoorSide fromSide)
    {
        // Check inventory for keycard
        if (!PlayerInventory.HasItem(requiredKeycard))
        {
            Debug.Log("Need keycard!");
            return false;
        }
        
        return base.TryOpen(fromSide);
    }
}
```

### Multiple Doors Linked

```csharp
public class LinkedDoors : MonoBehaviour
{
    [SerializeField] Door[] doors;
    
    public void OpenAll()
    {
        foreach (Door door in doors)
        {
            door.TryOpen(DoorSide.inside);
        }
    }
}
```

---

## 📚 API Reference

### IDoor Interface

```csharp
// Properties
bool blocked { get; }
bool usesCommonLock { get; }
DoorState doorState { get; }
DoorLockState doorLockStateInside { get; }
DoorLockState doorLockStateOutside { get; }

// Lock Operations
bool TryLockInside();
bool TryLockOutside();
bool TryUnlockInside();
bool TryUnlockOutside();

// Door Operations
bool TryOpen(DoorSide fromSide);
bool TryClose();

// Supernatural Operations
bool TryBlock();
bool TryUnblock();
bool TryDoorSwaying();
bool TryDoorStopSwaying(DoorState doorStateAfterStop = DoorState.opened);
```

### DoorEvents

```csharp
OnDoorStateChanged(string doorId, DoorState newState)
OnDoorLockStateChanged(string doorId, DoorLockState newLockState, DoorSide side)
OnDoorBlocked(string doorId)
OnDoorLockedJiggle(string doorId)
OnDoorSwayingStart(string doorId)
OnDoorSwayingStop(string doorId)
```

---

## 🎯 Performance Notes

- Door uses coroutines for animations (minimal overhead)
- State changes trigger save automatically (can disable if needed)
- Debug lines use DrawManager.cs Line.create() with caching
- Animation events are more efficient than polling state

---

## 🔮 Future Enhancements

1. **Multi-Panel Doors**: Support for double doors
2. **Sliding Doors**: Alternative animation system
3. **Sound Effects**: Integrated audio playback
4. **Particle Effects**: Dust, sparks, etc.
5. **Destructible Doors**: Health system integration
6. **Key Requirements**: Inventory integration

---

**End of Implementation Guide**