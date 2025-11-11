### High-Level Architecture

```
┌─────────────────────────────────────────────────────────┐
│  IDoor (interface)                                      │
│  - DoorState State { get; }                             │
│  - bool TryOpen() / TryClose() / TryUnlock()            │
│  - event Action<DoorState> OnStateChanged               │
└─────────────────────────────────────────────────────────┘
                        ▲
                        │ inherits
┌─────────────────────────────────────────────────────────┐
│  DoorBase : MonoBehaviour (abstract)                    │
│  - Animator animator                                    │
│  - DoorConfigSO config                                  │
│  - Coroutine autoCloseRoutine                           │
│  - IEnumerator OpenRoutine() / CloseRoutine()           │
│  - bool IsObstructed()                                  │
└─────────────────────────────────────────────────────────┘
           ▲                    ▲                    ▲
           │                    │                    │
  ┌────────┴────────┐  ┌────────┴────────┐  ┌───────┴────────┐
  │ DoorHinged      │  │ DoorSliding     │  │ DoorDouble     │
  │ (rotate pivot)  │  │ (translate pos) │  │ (sync 2 doors) │
  └─────────────────┘  └─────────────────┘  └────────────────┘
```