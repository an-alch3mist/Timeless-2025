```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_stateMachineApproach Simple ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false
  doorSwayStart (trigger) = false
  doorSwayStop (trigger) = false
  doorLockedJiggle (trigger) = false
  doorBlockedJiggle (trigger) = false
  lockInside (trigger) = false
  lockOutside (trigger) = false
  lockCommon (trigger) = false
  unlockInside (trigger) = false
  unlockOutside (trigger) = false
  unlockCommon (trigger) = false
  isDoorOpen (bool) = false
  isInsideLocked (bool) = false
  isOutsideLocked (bool) = false
  isDoorSwaying (bool) = false

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
│   │ ├ [doorBlockedJiggle = true] (hasExitTime:☑ | exitTime:0.75 | transition duration:0.25s) → doorBlockedClosedJiggle
│   │ ├ [doorSwayStart = true] (hasExitTime:☐ | exitTime:0.75 | transition duration:0.20s) → doorSwayLoopAnim
│   │ └ [isDoorOpen = true] (hasExitTime:☐ | exitTime:0.75 | transition duration:0.50s) → doorOpenedIdleAnim
│   ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│   │ └ [auto] (hasExitTime:☑ | exitTime:1.00 | transition duration:0.01s) → doorClosedIdleAnim
│   ├ doorLockedClosedJiggle | Motion: doorLockedClosedJiggle | Speed: 1.00x
│   │ └ [auto] (hasExitTime:☑ | exitTime:1.00 | transition duration:0.01s) → doorClosedIdleAnim
│   ├ doorOpenedIdleAnim | Motion: doorOpenedIdleAnim | Speed: 1.00x
│   │ ├ [doorClose = true] (hasExitTime:☐ | exitTime:0.75 | transition duration:0.01s) → doorClosingAnim
│   │ ├ [doorBlockedJiggle = true] (hasExitTime:☐ | exitTime:0.75 | transition duration:0.01s) → doorBlockedOpenedJiggle
│   │ └ [doorSwayStart = true] (hasExitTime:☐ | exitTime:0.75 | transition duration:0.30s) → doorSwayLoopAnim
│   ├ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x
│   │ └ [auto] (hasExitTime:☑ | exitTime:1.00 | transition duration:0.01s) → doorOpenedIdleAnim
│   └ doorSwayLoopAnim | Motion: doorSwayLoopAnim | Speed: 1.00x
│     ├ [doorSwayStop = true && isDoorOpen = false] (hasExitTime:☐ | exitTime:0.50 | transition duration:0.25s) → doorClosedIdleAnim
│     └ [doorSwayStop = true && isDoorOpen = true] (hasExitTime:☐ | exitTime:0.69 | transition duration:0.20s) → doorOpenedIdleAnim

├ -> Layer 1: LAYER 1: Inside Lock (Weight: 1.0, Override Blending)
│   Weight: 1.00 | Blending: Override | IK: False | Sync: None

│   Entry:
│     └ (default transition) → insideUnlockedIdle(The Default State)
│   States Info (4):
│   ├ insideLockedIdle | Motion: insideLockedIdle | Speed: 1.00x
│   │ └ [unlockInside = true] (hasExitTime:☐ | exitTime:0.75 | transition duration:0.01s) → insideUnlockingAnim
│   ├ insideLockingAnim | Motion: insideLockingAnim | Speed: 1.00x
│   │ └ [auto] (hasExitTime:☑ | exitTime:1.00 | transition duration:0.00s) → insideLockedIdle
│   ├ insideUnlockedIdle | Motion: insideUnlockedIdle | Speed: 1.00x [DEFAULT]
│   │ ├ [lockInside = true] (hasExitTime:☐ | exitTime:0.75 | transition duration:0.01s) → insideLockingAnim
│   │ └ [isInsideLocked = true] (hasExitTime:☐ | exitTime:0.75 | transition duration:0.20s) → insideLockedIdle
│   └ insideUnlockingAnim | Motion: insideUnlockingAnim | Speed: 1.00x
│     └ [auto] (hasExitTime:☑ | exitTime:1.00 | transition duration:0.00s) → insideUnlockedIdle

└ -> Layer 2: LAYER 2: Outside Lock (Weight: 1.0, Override Blending)
    Weight: 1.00 | Blending: Override | IK: False | Sync: None

    Entry:
      └ (default transition) → outsideUnlockedIdle(The Default State)
    States Info (8):
    ├ outsideLockedIdle | Motion: outsideLockedIdle | Speed: 1.00x
    │ └ [unlockOutside = true] (hasExitTime:☐ | exitTime:0.75 | transition duration:0.01s) → outsideUnlockingAnim
    ├ outsideLockingAnim | Motion: outsideLockingAnim | Speed: 1.00x
    │ └ [auto] (hasExitTime:☑ | exitTime:1.00 | transition duration:0.01s) → outsideLockedIdle
    ├ outsideUnlockedIdle | Motion: outsideUnlockedIdle | Speed: 1.00x [DEFAULT]
    │ ├ [lockOutside = true] (hasExitTime:☐ | exitTime:0.75 | transition duration:0.01s) → outsideLockingAnim
    │ └ [isOutsideLocked = true] (hasExitTime:☐ | exitTime:0.75 | transition duration:0.20s) → outsideLockedIdle
    ├ outsideUnlockingAnim | Motion: outsideUnlockingAnim | Speed: 1.00x
    │ └ [auto] (hasExitTime:☑ | exitTime:1.00 | transition duration:0.00s) → outsideUnlockedIdle
    ├ commonLockedIdleAnim | Motion: commonLockedIdleAnim | Speed: 1.00x
    ├ commonLockingAnim | Motion: commonLockingAnim | Speed: 1.00x
    ├ commonUnlockedIdle | Motion: commonUnlockedIdle | Speed: 1.00x
    └ commonUnlockingAnim | Motion: commonUnlockingAnim | Speed: 1.00x


```

