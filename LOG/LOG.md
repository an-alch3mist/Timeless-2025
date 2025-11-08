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

