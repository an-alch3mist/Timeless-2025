```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_stateMachineApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false
  doorBlockedJiggle (trigger) = false
  doorLockedJiggle (trigger) = false
  isDoorOpen (bool) = false
  lockInside (trigger) = false
  unlockInside (trigger) = false
  isInsideLocked (bool) = false
  lockOutside (trigger) = false
  unlockOutside (trigger) = false
  isOutsideLocked (bool) = false

Animation Layers (3):
├ -> Layer 0: Layer 0: Door Movemenet(Base Layer)
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│   Entry:
│     └ (default transition) → doorClosedIdleAnim(The Default State)
│   States Info (9):
│   ├ New State | Motion: (no motion) | Speed: 1.00x
│   ├ doorBlockedClosedJiggle | Motion: doorBlockedClosedJiggle | Speed: 1.00x
│   │ └ [auto] (hasExitTime:☑ | exitTime:1.00 | transition duration:0.01s) → doorClosedIdleAnim
│   ├ doorBlockedOpenedJiggle | Motion: doorBlockedOpenedJiggle | Speed: 1.00x
│   │ └ [auto] (hasExitTime:☑ | exitTime:1.00 | transition duration:0.01s) → doorOpenedIdleAnim
│   ├ doorClosedIdleAnim | Motion: doorClosedIdleAnim | Speed: 1.00x [DEFAULT]
│   │ ├ [doorOpen = true] (hasExitTime:☐ | exitTime:0.75 | transition duration:0.01s) → doorOpeningAnim
│   │ ├ [doorLockedJiggle = true] (hasExitTime:☑ | exitTime:0.75 | transition duration:0.25s) → doorLockedClosedJiggle
│   │ └ [doorBlockedJiggle = true] (hasExitTime:☑ | exitTime:0.75 | transition duration:0.25s) → doorBlockedClosedJiggle
│   ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│   │ └ [auto] (hasExitTime:☑ | exitTime:1.00 | transition duration:0.01s) → doorClosedIdleAnim
│   ├ doorLockedClosedJiggle | Motion: doorLockedClosedJiggle | Speed: 1.00x
│   │ └ [auto] (hasExitTime:☑ | exitTime:1.00 | transition duration:0.01s) → doorClosedIdleAnim
│   ├ doorOpenedIdleAnim | Motion: doorOpenedIdleAnim | Speed: 1.00x
│   │ ├ [doorClose = true] (hasExitTime:☐ | exitTime:0.75 | transition duration:0.01s) → doorClosingAnim
│   │ └ [doorBlockedJiggle = true] (hasExitTime:☐ | exitTime:0.75 | transition duration:0.01s) → doorBlockedOpenedJiggle
│   ├ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x
│   │ └ [auto] (hasExitTime:☑ | exitTime:1.00 | transition duration:0.01s) → doorOpenedIdleAnim
│   └ doorSwayLoopAnim | Motion: doorSwayLoopAnim | Speed: 1.00x

├ -> Layer 1: LAYER 1: Inside Lock (Weight: 1.0, Additive Blending)
│   Weight: 1.00 | Blending: Additive | IK: False | Sync: None

│   Entry:
│     └ (default transition) → insideUnlockedIdle(The Default State)
│   States Info (4):
│   ├ insideLockedIdle | Motion: insideLockedIdle | Speed: 1.00x
│   │ └ [unlockInside = true] (hasExitTime:☐ | exitTime:0.75 | transition duration:0.01s) → insideUnlockingAnim
│   ├ insideLockingAnim | Motion: insideLockingAnim | Speed: 1.00x
│   │ └ [auto] (hasExitTime:☑ | exitTime:1.00 | transition duration:0.01s) → insideLockedIdle
│   ├ insideUnlockedIdle | Motion: insideUnlockedIdle | Speed: 1.00x [DEFAULT]
│   │ └ [lockInside = true] (hasExitTime:☐ | exitTime:0.75 | transition duration:0.01s) → insideLockingAnim
│   └ insideUnlockingAnim | Motion: insideUnlockingAnim | Speed: 1.00x
│     └ [auto] (hasExitTime:☑ | exitTime:1.00 | transition duration:0.01s) → insideUnlockedIdle

└ -> Layer 2: LAYER 2: Outside Lock (Weight: 1.0, Additive Blending)
    Weight: 1.00 | Blending: Additive | IK: False | Sync: None

    Entry:
      └ (default transition) → outsideUnlockedIdle(The Default State)
    States Info (4):
    ├ outsideLockedIdle | Motion: outsideLockedIdle | Speed: 1.00x
    │ └ [unlockOutside = true] (hasExitTime:☐ | exitTime:0.75 | transition duration:0.01s) → outsideUnlockingAnim
    ├ outsideLockingAnim | Motion: outsideLockingAnim | Speed: 1.00x
    │ └ [auto] (hasExitTime:☑ | exitTime:1.00 | transition duration:0.01s) → outsideLockedIdle
    ├ outsideUnlockedIdle | Motion: outsideUnlockedIdle | Speed: 1.00x [DEFAULT]
    │ └ [lockOutside = true] (hasExitTime:☐ | exitTime:0.75 | transition duration:0.01s) → outsideLockingAnim
    └ outsideUnlockingAnim | Motion: outsideUnlockingAnim | Speed: 1.00x
      └ [auto] (hasExitTime:☑ | exitTime:1.00 | transition duration:0.01s) → outsideUnlockedIdle


```

```projectFolder-hierarchy
=== Asset Type Abbreviations ===
anim = AnimClip
================================

./doorHandle/
├ commonLockedIdleAnim.anim (anim | 0.00s | 60fps)
├ commonLockingAnim.anim (anim | 0.67s | 60fps)
├ commonUnlockedIdle.anim (anim | 0.00s | 60fps)
├ commonUnlockingAnim.anim (anim | 0.52s | 60fps)
├ insideLockedIdle.anim (anim | 0.00s | 60fps)
├ insideLockingAnim.anim (anim | 0.67s | 60fps)
├ insideUnlockedIdle.anim (anim | 0.00s | 60fps)
├ insideUnlockingAnim.anim (anim | 0.58s | 60fps)
├ outsideLockedIdle.anim (anim | 0.00s | 60fps)
├ outsideLockingAnim.anim (anim | 0.83s | 60fps)
├ outsideUnlockedIdle.anim (anim | 0.00s | 60fps)
└ outsideUnlockingAnim.anim (anim | 0.50s | 60fps)

```

```projectFolder-hierarchy
=== Asset Type Abbreviations ===
anim = AnimClip
================================

./Anim/
├ door/
│ ├ doorBlockedClosedJiggle.anim (anim | 1.00s | 60fps)
│ ├ doorBlockedOpenedJiggle.anim (anim | 0.08s | 60fps)
│ ├ doorClosedIdleAnim.anim (anim | 0.00s | 60fps)
│ ├ doorClosingAnim.anim (anim | 0.50s | 60fps)
│ ├ doorLockedClosedJiggle.anim (anim | 0.17s | 60fps)
│ ├ doorOpenedIdleAnim.anim (anim | 0.00s | 60fps)
│ ├ doorOpeningAnim.anim (anim | 0.67s | 60fps)
│ └ doorSwayLoopAnim.anim (anim | 0.38s | 60fps)
└ doorHandle/
  ├ commonLockedIdleAnim.anim (anim | 0.00s | 60fps)
  ├ commonLockingAnim.anim (anim | 0.67s | 60fps)
  ├ commonUnlockedIdle.anim (anim | 0.00s | 60fps)
  ├ commonUnlockingAnim.anim (anim | 0.52s | 60fps)
  ├ insideLockedIdle.anim (anim | 0.00s | 60fps)
  ├ insideLockingAnim.anim (anim | 0.67s | 60fps)
  ├ insideUnlockedIdle.anim (anim | 0.00s | 60fps)
  ├ insideUnlockingAnim.anim (anim | 0.58s | 60fps)
  ├ outsideLockedIdle.anim (anim | 0.00s | 60fps)
  ├ outsideLockingAnim.anim (anim | 0.83s | 60fps)
  ├ outsideUnlockedIdle.anim (anim | 0.00s | 60fps)
  └ outsideUnlockingAnim.anim (anim | 0.50s | 60fps)

```

