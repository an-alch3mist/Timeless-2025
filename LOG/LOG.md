# LIST<> GameObject from hierarchy query:
pri~m_stringLength   | pri~m_firstChar  
---------------------+------------------
7                    | t                
20                   | d                
20                   | d                


 * LIST<> GameObject from hierarchy query Count: 3 
---------------------------------------------------
 trigger                                           
 door outside trigger                              
 door inside trigger                               


 * LIST<> GameObject from hierarchy query Count: 3 
---------------------------------------------------
 trigger                                           
 door outside trigger                              
 door inside trigger                               


 * LIST<> GameObject from hierarchy query Count: 3 
---------------------------------------------------
 trigger (UnityEngine.GameObject)                  
 door outside trigger (UnityEngine.GameObject)     
 door inside trigger  (UnityEngine.GameObject)     


```sceneGameObject-hierarchy
=== Component Abbreviations ===
dmc = MeshFilter | MeshRenderer
================================

./PlayerFirstPerson/(scale:1.0 | no components)
├ PlayerFollowCamera (scale:1.0 | CinemachineVirtualCamera)
│ └ cm (scale:1.0 | CinemachinePipeline, CinemachineBasicMultiChannelPerlin, Cinemachine3rdPersonFollow)
└ PlayerCapsule (scale:1.0 | CharacterController, FirstPersonController, BasicRigidBodyPush, StarterAssetsInputs, PlayerInput)
  ├ PlayerCameraRoot (scale:1.0 | no components)
  └ Capsule (scale:(0.3,1.8,0.3) | dmc, DEBUG_CollisionTrigger)

```

```sceneGameObject-hierarchy
=== Component Abbreviations ===
dmc = MeshFilter | MeshRenderer
bc = BoxCollider
================================

./Level/(scale:1.0 | no components)
├ door (0) (scale:1.0 | no components)
│ ├ Cube (0) trigger (0) (scale:(3.0,1.0,3.0) | dmc, bc)
│ └ Cube (0) trigger (1) (scale:(3.0,1.0,3.0) | dmc, bc)
├ door (1) (scale:1.0 | no components)
│ ├ Cube (0) trigger (0) (scale:(3.0,1.0,3.0) | dmc, bc)
│ └ Cube (0) trigger (1) (scale:(3.0,1.0,3.0) | dmc, bc)
└ door (2) (scale:1.0 | no components)
  ├ Cube (0) trigger (0) (scale:(3.0,1.0,3.0) | dmc, bc)
  └ Cube (0) trigger (1) (scale:(3.0,1.0,3.0) | dmc, bc)

```

