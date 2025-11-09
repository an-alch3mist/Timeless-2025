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

