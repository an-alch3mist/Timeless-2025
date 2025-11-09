[Entry] -> Closed (default, looped idle)
   --(doorOpen trigger, Has Exit Time = false)--> Opening (open clip, non-loop)
       --(on animation finish: Has Exit Time = true, Exit Time = 1)--> Open (looped idle)
           --(doorClose trigger, Has Exit Time = false)--> Closing (close clip, non-loop)
               --(on animation finish: Has Exit Time = true, Exit Time = 1)--> Closed
```scene-hierarchy
=== Component Abbreviations ===
dmc = MeshFilter | MeshRenderer
smc = SkinnedMeshRenderer
rb = Rigidbody
rb2d = Rigidbody2D
bc = BoxCollider
sc = SphereCollider
cc = CapsuleCollider
mc = MeshCollider
bc2d = BoxCollider2D
cc2d = CircleCollider2D
anim = Animator
animclip = Animation
asrc = AudioSource
alstn = AudioListener
cam = Camera
lgt = Light
canvas = Canvas
cr = CanvasRenderer
sr = ScrollRect
tmp = TextMeshProUGUI | TextMeshPro
btnO = Button | Image | Outline
btn = Button | Image
img = Image
autoFitH = HorizontalLayoutGroup | ContentSizeFitter
autoFitV = VerticalLayoutGroup | ContentSizeFitter
autoFit = ContentSizeFitter
ps = ParticleSystem
psr = ParticleSystemRenderer
tr = TrailRenderer
lr = LineRenderer
================================

=== Asset Type Abbreviations ===
mesh = Mesh
mat = Material
pf = Prefab
tex = Texture
anim = AnimClip
audio = Audio
cs = Script
scene = Scene
txt = TextAsset
================================

./turnYDoor/(scale:1.0 | anim, Door)
├ door (scale:1.0 | no components)
├ door origin (scale:1.0 | no components)
│ ├ handle (scale:(0.1,0.2,0.1) | dmc, bc)
│ └ door block (scale:(1.0,2.0,0.1) | dmc, bc)
├ hinge block (scale:(0.1,2.0,0.1) | dmc, bc)
└ hinge (scale:(0.1,2.0,0.1) | dmc, bc)

```

```animator-controller
=== Animator Controller: doorOpenCloseAnimController_stateMachineApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Default State: doorClosedAnim
│ States (5):
│ ├ New State | Motion: (no motion) | Speed: 1.00x
│ ├ doorClosedAnim | Motion: doorClosedAnim | Speed: 1.00x [DEFAULT]
│ │ └ [doorOpen = true] (exit:0.00 | dur:0.01s) → doorOpeningAnim
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ │ └ [auto] (exit:1.00 | dur:0.01s) → doorClosedAnim
│ ├ doorOpenedAnim | Motion: doorOpenedAnim | Speed: 1.00x
│ │ └ [doorClose = true] (exit:0.00 | dur:0.01s) → doorClosingAnim
│ └ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x
│   └ [auto] (exit:1.00 | dur:0.01s) → doorOpenedAnim


```

```animator-controller
=== Animator Controller: doorOpenCloseAnimController_AnyStateExitApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Any State:
│   ├ [doorOpen = true] (exit:0.75 | dur:0.01s) → doorOpeningAnim
│   ├ [doorClose = true] (exit:0.75 | dur:0.01s) → doorClosingAnim
│ Default State: New State
│ States (5):
│ ├ New State | Motion: (no motion) | Speed: 1.00x [DEFAULT]
│ ├ doorClosedAnim | Motion: doorClosedAnim | Speed: 1.00x
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ ├ doorOpenedAnim | Motion: doorOpenedAnim | Speed: 1.00x
│ └ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x


```

```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_AnyStateExitApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Any State:
│   ├ [doorOpen = true] (exit:0.75 | dur:0.01s) → doorOpeningAnim
│   ├ [doorClose = true] (exit:0.75 | dur:0.01s) → doorClosingAnim
│ Default State: New State
│ States (5):
│ ├ New State | Motion: (no motion) | Speed: 1.00x [DEFAULT]
│ ├ doorClosedAnim | Motion: doorClosedAnim | Speed: 1.00x
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ ├ doorOpenedAnim | Motion: doorOpenedAnim | Speed: 1.00x
│ └ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x


```

```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_AnyStateExitApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Any State:
│   ├ [doorOpen = true] (exit:0.75 | dur:0.01s) → doorOpeningAnim
│   ├ [doorClose = true] (exit:0.75 | dur:0.01s) → doorClosingAnim
│ Default State: New State
│ States (3):
│ ├ New State | Motion: (no motion) | Speed: 1.00x [DEFAULT]
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ └ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x


```

```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_AnyStateExitApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Any State:
│   ├ [doorOpen = true] (exitTime:0.75 | transition:0.01s) → doorOpeningAnim
│   ├ [doorClose = true] (exitTime:0.75 | transition:0.01s) → doorClosingAnim
│ Default State: New State
│ States (3):
│ ├ New State | Motion: (no motion) | Speed: 1.00x [DEFAULT]
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ └ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x


```

```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_AnyStateExitApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Any State:
│   ├ [doorOpen = true] (exitTime:0.75 | transition:0.01s) → doorOpeningAnim
│   ├ [doorClose = true] (exitTime:0.75 | transition:0.01s) → doorClosingAnim
│ Default State: New State
│ States (3):
│ ├ New State | Motion: (no motion) | Speed: 1.00x [DEFAULT]
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ │ └ [auto] (exitTime:0.50 | transition:0.25s) → New StateMachine
│ └ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x
│ Sub-State Machines (1):
│ └ [StateMachine] New StateMachine
│     Default State: doorClosedAnim
│     States (1):
│     └ doorClosedAnim | Motion: doorClosedAnim | Speed: 1.00x [DEFAULT]


```

```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_AnyStateExitApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Any State:
│   ├ [doorOpen = true] (exitTime:0.75 | transition:0.01s) → doorOpeningAnim
│   ├ [doorClose = true] (exitTime:0.75 | transition:0.01s) → doorClosingAnim
│ Default State: New State
│ States (3):
│ ├ New State | Motion: (no motion) | Speed: 1.00x [DEFAULT]
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ │ └ [auto] (exitTime:0.50 | transition:0.25s) → New StateMachine
│ └ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x
│ Sub-State Machines (1):
│ └ [StateMachine] New StateMachine
│     Default State: doorClosedAnim
│     States (1):
│     └ doorClosedAnim | Motion: doorClosedAnim | Speed: 1.00x [DEFAULT]
│       └ [auto] (exitTime:0.75 | transition:0.25s) → Exit


```

```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_stateMachineApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Default State: doorClosedAnim
│ States (5):
│ ├ New State | Motion: (no motion) | Speed: 1.00x
│ ├ doorClosedAnim | Motion: doorClosedAnim | Speed: 1.00x [DEFAULT]
│ │ └ [doorOpen = true] (exitTime:0.00 | transition:0.01s) → doorOpeningAnim
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ │ └ [auto] (exitTime:1.00 | transition:0.01s) → doorClosedAnim
│ ├ doorOpenedAnim | Motion: doorOpenedAnim | Speed: 1.00x
│ │ └ [doorClose = true] (exitTime:0.00 | transition:0.01s) → doorClosingAnim
│ └ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x
│   └ [auto] (exitTime:1.00 | transition:0.01s) → doorOpenedAnim


```

```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_AnyStateExitApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Any State:
│   ├ [doorOpen = true] (exitTime:0.75 | transition:0.01s) → doorOpeningAnim
│   ├ [doorClose = true] (exitTime:0.75 | transition:0.01s) → doorClosingAnim
│   ├ [immediate] (exitTime:0.75 | transition:0.25s) → doorOpenedAnim
│ Default State: New State
│ States (3):
│ ├ New State | Motion: (no motion) | Speed: 1.00x [DEFAULT]
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ │ └ [auto] (exitTime:0.50 | transition:0.25s) → New StateMachine
│ └ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x
│ Sub-State Machines (1):
│ └ [StateMachine] New StateMachine
│     Default State: doorClosedAnim
│     States (2):
│     ├ doorClosedAnim | Motion: doorClosedAnim | Speed: 1.00x [DEFAULT]
│     │ └ [auto] (exitTime:0.75 | transition:0.25s) → Exit
│     └ doorOpenedAnim | Motion: doorOpenedAnim | Speed: 1.00x


```

```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_AnyStateExitApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Any State:
│   ├ [doorOpen = true] (exitTime:0.75 | transition:0.01s) → doorOpeningAnim
│   ├ [doorClose = true] (exitTime:0.75 | transition:0.01s) → doorClosingAnim
│   ├ [immediate] (exitTime:0.75 | transition:0.25s) → doorOpenedAnim
│ Default State: New State
│ States (3):
│ ├ New State | Motion: (no motion) | Speed: 1.00x [DEFAULT]
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ │ └ [auto] (exitTime:0.50 | transition:0.25s) → New StateMachine
│ └ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x
│ Sub-State Machines (1):
│ └ [StateMachine] New StateMachine
│     Default State: doorClosedAnim
│     States (2):
│     ├ doorClosedAnim | Motion: doorClosedAnim | Speed: 1.00x [DEFAULT]
│     │ └ [auto] (exitTime:0.75 | transition:0.25s) → Exit
│     └ doorOpenedAnim | Motion: doorOpenedAnim | Speed: 1.00x


```

```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_AnyStateExitApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false
  blendX (float) = 0.00
  blendY (float) = 0.00

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Any State:
│   ├ [doorOpen = true] (exitTime:0.75 | transition:0.01s) → doorOpeningAnim
│   ├ [doorClose = true] (exitTime:0.75 | transition:0.01s) → doorClosingAnim
│   ├ [immediate] (exitTime:0.75 | transition:0.25s) → doorOpenedAnim
│ Default State: New State
│ States (4):
│ ├ New State | Motion: (no motion) | Speed: 1.00x [DEFAULT]
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ │ └ [auto] (exitTime:0.50 | transition:0.25s) → New StateMachine
│ ├ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x
│ │ └ [auto] (exitTime:0.63 | transition:0.25s) → A Sample Blend Tree
│ └ A Sample Blend Tree | BlendTree: Blend Tree (2D Directional) | Speed: 1.00x
│     ├ BlendParameter: blendX, blendY
│     ├ [pos:(0.00,1.00)] doorClosingAnim (speed:1.00x)
│     └ [pos:(1.00,1.00)] doorOpeningAnim (speed:1.00x)
│ Sub-State Machines (1):
│ └ [StateMachine] New StateMachine
│     Default State: doorClosedAnim
│     States (2):
│     ├ doorClosedAnim | Motion: doorClosedAnim | Speed: 1.00x [DEFAULT]
│     │ └ [auto] (exitTime:0.75 | transition:0.25s) → Exit
│     └ doorOpenedAnim | Motion: doorOpenedAnim | Speed: 1.00x


```

```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_AnyStateExitApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false
  blendX (float) = 0.00
  blendY (float) = 0.00

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Any State:
│   ├ [doorOpen = true] (exitTime:0.75 | transition:0.01s) → doorOpeningAnim
│   ├ [doorClose = true] (exitTime:0.75 | transition:0.01s) → doorClosingAnim
│   ├ [doorOpen = true] (exitTime:0.75 | transition:0.25s) → doorOpenedAnim
│ Default State: New State
│ States (4):
│ ├ New State | Motion: (no motion) | Speed: 1.00x [DEFAULT]
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ │ └ [auto] (exitTime:0.50 | transition:0.25s) → New StateMachine
│ ├ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x
│ │ └ [auto] (exitTime:0.63 | transition:0.25s) → A Sample Blend Tree
│ └ A Sample Blend Tree | BlendTree: Blend Tree (2D Directional) | Speed: 1.00x
│     ├ BlendParameter: blendX, blendY
│     ├ [pos:(0.00,1.00)] doorClosingAnim (speed:1.00x)
│     └ [pos:(1.00,1.00)] doorOpeningAnim (speed:1.00x)
│ Sub-State Machines (1):
│ └ [StateMachine] New StateMachine
│     Default State: doorClosedAnim
│     States (2):
│     ├ doorClosedAnim | Motion: doorClosedAnim | Speed: 1.00x [DEFAULT]
│     │ └ [auto] (exitTime:0.75 | transition:0.25s) → Exit
│     └ doorOpenedAnim | Motion: doorOpenedAnim | Speed: 1.00x


```

```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_AnyStateExitApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false
  blendX (float) = 0.00
  blendY (float) = 0.00

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Any State:
│   ├ [doorOpen = true] (exitTime:0.75 | transition:0.01s) → doorOpeningAnim
│   ├ [doorClose = true] (exitTime:0.75 | transition:0.01s) → doorClosingAnim
│ Default State: New State
│ States (4):
│ ├ New State | Motion: (no motion) | Speed: 1.00x [DEFAULT]
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ ├ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x
│ │ └ [auto] (exitTime:0.63 | transition:0.25s) → A Sample Blend Tree
│ └ A Sample Blend Tree | BlendTree: Blend Tree (2D Directional) | Speed: 1.00x
│     ├ BlendParameter: blendX, blendY
│     ├ [pos:(0.00,1.00)] doorClosingAnim (speed:1.00x)
│     └ [pos:(1.00,1.00)] doorOpeningAnim (speed:1.00x)


```

```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_AnyStateExitApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false
  blendX (float) = 0.00
  blendY (float) = 0.00

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Any State:
│   ├ [doorOpen = true] (exitTime:0.75 | transition:0.01s) → doorOpeningAnim
│   ├ [doorClose = true] (exitTime:0.75 | transition:0.01s) → doorClosingAnim
│ Default State: New State
│ States (9):
│ ├ New State | Motion: (no motion) | Speed: 1.00x [DEFAULT]
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ │ └ [auto] (exitTime:0.50 | transition:0.25s) → doorClosedAnim
│ ├ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x
│ │ └ [auto] (exitTime:0.63 | transition:0.25s) → A Sample Blend Tree
│ ├ A Sample Blend Tree | BlendTree: Blend Tree (2D Directional) | Speed: 1.00x
│ │   ├ BlendParameter: blendX, blendY
│ │   ├ [pos:(0.00,1.00)] doorClosingAnim (speed:1.00x)
│ │   └ [pos:(1.00,1.00)] doorOpeningAnim (speed:1.00x)
│ ├ doorOpenedAnim | Motion: doorOpenedAnim | Speed: 1.00x
│ │ ├ [auto] (exitTime:0.75 | transition:0.25s) → doorClosingAnim 0
│ │ ├ [auto] (exitTime:0.75 | transition:0.25s) → doorOpenedAnim 0
│ │ ├ [auto] (exitTime:0.75 | transition:0.25s) → A Sample Blend Tree
│ │ └ [auto] (exitTime:0.75 | transition:0.25s) → doorOpeningAnim 0
│ ├ doorClosedAnim | Motion: doorClosedAnim | Speed: 1.00x
│ │ └ [auto] (exitTime:0.75 | transition:0.25s) → doorOpeningAnim 0
│ ├ doorClosingAnim 0 | Motion: doorClosingAnim | Speed: 1.00x
│ ├ doorOpenedAnim 0 | Motion: doorOpenedAnim | Speed: 1.00x
│ └ doorOpeningAnim 0 | Motion: doorOpeningAnim | Speed: 1.00x
│   ├ [auto] (exitTime:0.63 | transition:0.25s) → A Sample Blend Tree
│   └ [auto] (exitTime:0.63 | transition:0.25s) → doorOpenedAnim


```

```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_AnyStateExitApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false
  blendX (float) = 0.00
  blendY (float) = 0.00

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Any State:
│   ├ [doorOpen = true] (exitTime:0.75 | transition:0.01s) → doorOpeningAnim
│   ├ [doorClose = true] (exitTime:0.75 | transition:0.01s) → doorClosingAnim
│ Default State: New State
│ States (3):
│ ├ New State | Motion: (no motion) | Speed: 1.00x [DEFAULT]
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ └ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x


```

```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_AnyStateExitApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false
  blendX (float) = 0.00
  blendY (float) = 0.00

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Any State:
│   ├ [doorOpen = true] (exitTime:0.75 | transition:0.01s) → doorOpeningAnim
│   ├ [doorClose = true] (exitTime:0.75 | transition:0.01s) → doorClosingAnim
│ Entry →: New State(The Default State)
│ States (3):
│ ├ New State | Motion: (no motion) | Speed: 1.00x [DEFAULT]
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ └ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x


```

```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_AnyStateExitApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false
  blendX (float) = 0.00
  blendY (float) = 0.00

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Entry:
│   ├ default → New State(The Default State)
│ Any State:
│   ├ [doorOpen = true] (exitTime:0.75 | transition:0.01s) → doorOpeningAnim
│   ├ [doorClose = true] (exitTime:0.75 | transition:0.01s) → doorClosingAnim
│ States (3):
│ ├ New State | Motion: (no motion) | Speed: 1.00x [DEFAULT]
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ └ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x


```

```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_AnyStateExitApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false
  blendX (float) = 0.00
  blendY (float) = 0.00

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Entry:
│   └ (default transiotion) → New State(The Default State)
│ Any State:
│   ├ [doorOpen = true] (exitTime:0.75 | transition:0.01s) → doorOpeningAnim
│   ├ [doorClose = true] (exitTime:0.75 | transition:0.01s) → doorClosingAnim
│ States (3):
│ ├ New State | Motion: (no motion) | Speed: 1.00x [DEFAULT]
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ └ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x


```

```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_AnyStateExitApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false
  blendX (float) = 0.00
  blendY (float) = 0.00

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Entry:
│   └ (default transiotion) → New State(The Default State)
│ Any State:
│   ├ [doorOpen = true] (exitTime:0.75 | transition:0.01s) → doorOpeningAnim
│   └ [doorClose = true] (exitTime:0.75 | transition:0.01s) → doorClosingAnim
│ States (3):
│ ├ New State | Motion: (no motion) | Speed: 1.00x [DEFAULT]
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ └ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x


```

```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_AnyStateExitApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false
  blendX (float) = 0.00
  blendY (float) = 0.00

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Entry:
│   └ (default transiotion) → New State(The Default State)
│ Any State:
│   ├ [doorOpen = true] (exitTime:0.75 | transition:0.01s) → doorOpeningAnim
│   └ [doorClose = true] (exitTime:0.75 | transition:0.01s) → doorClosingAnim
│ States Info (3):
│ ├ New State | Motion: (no motion) | Speed: 1.00x [DEFAULT]
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ └ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x


```

```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_AnyStateExitApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false
  blendX (float) = 0.00
  blendY (float) = 0.00

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Entry:
│   └ (default transiotion) → New State(The Default State)
│ Any State:
│   ├ [doorOpen = true] (exitTime:0.75 | transition:0.01s) → doorOpeningAnim
│   └ [doorClose = true] (exitTime:0.75 | transition:0.01s) → doorClosingAnim
│ States Info (4):
│ ├ New State | Motion: (no motion) | Speed: 1.00x [DEFAULT]
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ │ └ [auto] (exitTime:0.95 | transition:0.25s) → doorClosedAnim
│ ├ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x
│ └ doorClosedAnim | Motion: doorClosedAnim | Speed: 1.00x


```

```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_AnyStateExitApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false
  blendX (float) = 0.00
  blendY (float) = 0.00

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Entry:
│   └ (default transiotion) → New State(The Default State)
│ Any State:
│   ├ [doorOpen = true] (exitTime:0.75 | transition:0.01s) → doorOpeningAnim
│   └ [doorClose = true] (exitTime:0.75 | transition:0.01s) → doorClosingAnim
│ States Info (8):
│ ├ New State | Motion: (no motion) | Speed: 1.00x [DEFAULT]
│ │ └ [auto] (exitTime:0.75 | transition:0.25s) → doorClosedAnim 0
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ │ ├ [auto] (exitTime:0.95 | transition:0.25s) → doorClosedAnim
│ │ └ [auto] (exitTime:0.50 | transition:0.25s) → doorClosedAnim 0
│ ├ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x
│ │ └ [auto] (exitTime:0.63 | transition:0.25s) → doorClosedAnim 0
│ ├ doorClosedAnim | Motion: doorClosedAnim | Speed: 1.00x
│ │ ├ [auto] (exitTime:0.75 | transition:0.25s) → doorClosedAnim 0
│ │ └ [auto] (exitTime:0.75 | transition:0.25s) → doorOpenedAnim
│ ├ doorClosedAnim 0 | Motion: doorClosedAnim | Speed: 1.00x
│ │ └ [auto] (exitTime:0.75 | transition:0.25s) → New State
│ ├ doorClosingAnim 0 | Motion: doorClosingAnim | Speed: 1.00x
│ │ └ [auto] (exitTime:0.50 | transition:0.25s) → doorClosedAnim
│ ├ doorOpenedAnim | Motion: doorOpenedAnim | Speed: 1.00x
│ │ └ [auto] (exitTime:0.75 | transition:0.25s) → doorClosingAnim 0
│ └ doorOpeningAnim 0 | Motion: doorOpeningAnim | Speed: 1.00x
│   └ [auto] (exitTime:0.63 | transition:0.25s) → doorClosedAnim 0


```

```
╔══════════════════════════════════════════════════════════════════════════════════════════════╗
║  Animator Controller: doorOpenCloseAnimController_AnyStateExitApproach                      ║
╚══════════════════════════════════════════════════════════════════════════════════════════════╝

PARAMETERS:
  • doorOpen (trigger) = false
  • doorClose (trigger) = false  
  • blendX (float) = 0.00
  • blendY (float) = 0.00

┌──────────────────────────────────────────────────────────────────────────────────────────────┐
│ LAYER: Base Layer (Weight: 0.00 | Override | No IK)                                          │
└──────────────────────────────────────────────────────────────────────────────────────────────┘


         ┌─────────┐
         │  Entry  │ (green)
         │ (start) │
         └────┬────┘
              │
              │ (default transition)
              ▼
         ┌─────────────────┐
         │   New State     │ [DEFAULT] (orange)
         │   No Motion     │
         │   Speed: 1.00x  │
         └────────┬────────┘
                  │
                  │ [auto @0.75]
                  ▼
         ┌──────────────────┐
         │ doorClosedAnim 0 │◄──────────────────────────────────────┐
         │ Motion: closed   │                                       │
         │ Speed: 1.00x     │                                       │
         └────────┬─────────┘                                       │
                  │                                                 │
                  │ [auto @0.75]                                    │
                  │                                                 │
                  └──────────────────> (loops back to New State)    │
                                                                    │
                                                                    │
    ┌──────────────┐                                                │
    │  Any State   │ (cyan - can interrupt from anywhere)           │
    │              │                                                │
    └───┬──────┬───┘                                                │
        │      │                                                    │
        │      └─[doorClose=true]───┐                               │
        │                           │                               │
        │                           ▼                               │
        │                  ┌──────────────────┐                     │
        │                  │ doorClosingAnim  │                     │
        │                  │ Motion: closing  │                     │
        │                  │ Speed: 1.00x     │                     │
        │                  └────────┬─────────┘                     │
        │                           │                               │
        │                  ┌────────┴────────┐                      │
        │                  │                 │                      │
        │            [auto @0.95]      [auto @0.50]                 │
        │                  │                 │                      │
        │                  ▼                 ▼                      │
        │         ┌───────────────┐   ┌──────────────────┐          │
        │         │ doorClosedAnim│   │ doorClosedAnim 0 │──────────┘
        │         │ Motion: closed│   │ Motion: closed   │
        │         │ Speed: 1.00x  │   │ Speed: 1.00x     │
        │         └───────┬───────┘   └──────────────────┘
        │                 │
        │        ┌────────┴─────────┐
        │        │                  │
        │  [auto @0.75]       [auto @0.75]
        │        │                  │
        │        │                  ▼
        │        │         ┌──────────────────┐
        │        │         │  doorOpenedAnim  │
        │        │         │  Motion: opened  │
        │        │         │  Speed: 1.00x    │
        │        │         └────────┬─────────┘
        │        │                  │
        │        │                  │ [auto @0.75]
        │        │                  │
        │        │                  ▼
        │        │         ┌──────────────────┐
        │        │         │ doorClosingAnim 0│
        │        │         │ Motion: closing  │
        │        │         │ Speed: 1.00x     │
        │        │         └────────┬─────────┘
        │        │                  │
        │        │                  │ [auto @0.50]
        │        │                  │
        │        └──────────────────┴──────> doorClosedAnim
        │
        │
        └─[doorOpen=true]──┐
                           │
                           ▼
                  ┌──────────────────┐
                  │ doorOpeningAnim  │
                  │ Motion: opening  │
                  │ Speed: 1.00x     │
                  └────────┬─────────┘
                           │
                           │ [auto @0.63]
                           ▼
                  ┌──────────────────┐
                  │ doorClosedAnim 0 │ (rejoins main flow)
                  │ Motion: closed   │
                  └──────────────────┘


                  ┌──────────────────┐
                  │ doorOpeningAnim 0│ (orphaned - no incoming transitions!)
                  │ Motion: opening  │
                  │ Speed: 1.00x     │
                  └────────┬─────────┘
                           │
                           │ [auto @0.63]
                           ▼
                  ┌──────────────────┐
                  │ doorClosedAnim 0 │
                  └──────────────────┘


═══════════════════════════════════════════════════════════════════════════════════════════════

FLOW ANALYSIS:

🟢 ENTRY POINT:
   Entry → New State (orange default state)

🔵 ANY STATE INTERRUPTS:
   Any State → doorOpeningAnim  [when doorOpen trigger fires]
   Any State → doorClosingAnim  [when doorClose trigger fires]

📊 STATE FLOW PATHS:

  PATH 1 (Normal Start):
    Entry → New State → doorClosedAnim 0 → (loops back to New State)

  PATH 2 (Door Open Trigger):
    Any State → doorOpeningAnim → doorClosedAnim 0 → New State

  PATH 3 (Door Close Trigger):
    Any State → doorClosingAnim → doorClosedAnim (or doorClosedAnim 0)
    doorClosedAnim can go to:
      • doorClosedAnim 0
      • doorOpenedAnim → doorClosingAnim 0 → doorClosedAnim

⚠️  ISSUES DETECTED:
  1. doorOpeningAnim 0 is ORPHANED (no states transition to it)
  2. Multiple duplicate states with "0" suffix causing confusion
  3. Circular loops: New State ↔ doorClosedAnim 0
  4. doorClosedAnim has 2 auto-transitions competing (race condition)

✓ CORRECT ELEMENTS:
  • Entry properly goes to single default state
  • Any State can interrupt from anywhere
  • Transitions have exit times to allow animations to complete

═══════════════════════════════════════════════════════════════════════════════════════════════
```
```sceneGameObject-hierarchy
=== Component Abbreviations ===
dmc = MeshFilter | MeshRenderer
smc = SkinnedMeshRenderer
rb = Rigidbody
rb2d = Rigidbody2D
bc = BoxCollider
sc = SphereCollider
cc = CapsuleCollider
mc = MeshCollider
bc2d = BoxCollider2D
cc2d = CircleCollider2D
anim = Animator
animclip = Animation
asrc = AudioSource
alstn = AudioListener
cam = Camera
lgt = Light
canvas = Canvas
cr = CanvasRenderer
sr = ScrollRect
tmp = TextMeshProUGUI | TextMeshPro
btnO = Button | Image | Outline
btn = Button | Image
img = Image
autoFitH = HorizontalLayoutGroup | ContentSizeFitter
autoFitV = VerticalLayoutGroup | ContentSizeFitter
autoFit = ContentSizeFitter
ps = ParticleSystem
psr = ParticleSystemRenderer
tr = TrailRenderer
lr = LineRenderer
================================

=== Asset Type Abbreviations ===
mesh = Mesh
mat = Material
pf = Prefab
tex = Texture
anim = AnimClip
audio = Audio
cs = Script
scene = Scene
txt = TextAsset
================================

./turnYDoor/(scale:1.0 | anim, Door)
├ door (scale:1.0 | no components)
├ door origin (scale:1.0 | no components)
│ ├ handle (scale:(0.1,0.2,0.1) | dmc, bc)
│ └ door block (scale:(1.0,2.0,0.1) | dmc, bc)
├ hinge block (scale:(0.1,2.0,0.1) | dmc, bc)
└ hinge (scale:(0.1,2.0,0.1) | dmc, bc)

```

```sceneGameObject-hierarchy
=== Component Abbreviations ===
dmc = MeshFilter | MeshRenderer
smc = SkinnedMeshRenderer
rb = Rigidbody
rb2d = Rigidbody2D
bc = BoxCollider
sc = SphereCollider
cc = CapsuleCollider
mc = MeshCollider
bc2d = BoxCollider2D
cc2d = CircleCollider2D
anim = Animator
animclip = Animation
asrc = AudioSource
alstn = AudioListener
cam = Camera
lgt = Light
canvas = Canvas
cr = CanvasRenderer
sr = ScrollRect
tmp = TextMeshProUGUI | TextMeshPro
btnO = Button | Image | Outline
btn = Button | Image
img = Image
autoFitH = HorizontalLayoutGroup | ContentSizeFitter
autoFitV = VerticalLayoutGroup | ContentSizeFitter
autoFit = ContentSizeFitter
ps = ParticleSystem
psr = ParticleSystemRenderer
tr = TrailRenderer
lr = LineRenderer
================================

=== Asset Type Abbreviations ===
mesh = Mesh
mat = Material
pf = Prefab
tex = Texture
anim = AnimClip
audio = Audio
cs = Script
scene = Scene
txt = TextAsset
================================

./turnYDoor/(scale:1.0 | anim, Door)
├ door (scale:1.0 | no components)
├ door origin (scale:1.0 | no components)
│ ├ handle (scale:(0.1,0.2,0.1) | dmc, bc)
│ └ door block (scale:(1.0,2.0,0.1) | dmc, bc)
├ hinge block (scale:(0.1,2.0,0.1) | dmc, bc)
└ hinge (scale:(0.1,2.0,0.1) | dmc, bc)

```

```animatorController-hierarchy
=== Animator Controller: doorOpenCloseAnimController_stateMachineApproach ===

Parameters:
  doorOpen (trigger) = false
  doorClose (trigger) = false

Layers (1):
├ Layer 0: Base Layer
│   Weight: 0.00 | Blending: Override | IK: False | Sync: None

│ Entry:
│   └ (default transition) → doorClosedAnim(The Default State)
│ States Info (5):
│ ├ New State | Motion: (no motion) | Speed: 1.00x
│ ├ doorClosedAnim | Motion: doorClosedAnim | Speed: 1.00x [DEFAULT]
│ │ └ [doorOpen = true] (exitTime:0.00 | transition:0.01s) → doorOpeningAnim
│ ├ doorClosingAnim | Motion: doorClosingAnim | Speed: 1.00x
│ │ └ [auto] (exitTime:1.00 | transition:0.01s) → doorClosedAnim
│ ├ doorOpenedAnim | Motion: doorOpenedAnim | Speed: 1.00x
│ │ └ [doorClose = true] (exitTime:0.00 | transition:0.01s) → doorClosingAnim
│ └ doorOpeningAnim | Motion: doorOpeningAnim | Speed: 1.00x
│   └ [auto] (exitTime:1.00 | transition:0.01s) → doorOpenedAnim


```

