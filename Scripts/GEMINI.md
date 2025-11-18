# Project Overview

This is a Unity project for a game titled "Timeless". The project is focused on creating a robust and interactive door system, which appears to be a core gameplay mechanic. The codebase includes two different implementations of the door system, one using an interface-based approach and the other using an abstract base class. The latter seems to be the more recent and production-ready version.

The project also includes a `GameStore.cs` script that manages the game's state, including player statistics and input actions. This script is loaded at the beginning of a scene and handles saving and loading of game data.

## Key Files

*   `Assets/Scripts/GameStore.cs`: This script manages the game's state, including player statistics and input actions. It also contains a detailed design for an animator controller for a door.
*   `Assets/Scripts/DoorSystem/SimpleIDoor with IDoor/IDoor.cs`: This file defines an interface for a door system, including states for the door and its locks, as well as a variety of actions that can be performed on the door.
*   `Assets/Scripts/DoorSystem/SimpleDoor With DoorBase/DoorBase.cs`: This abstract class provides a comprehensive implementation of the door logic. It handles door states, animations, audio, and various interaction scenarios.
*   `Assets/Scripts/DoorSystem/SimpleDoor With DoorBase/SimpleDoorHinged.cs`: This is a concrete implementation of the `DoorBase` class and is considered production-ready.

## Building and Running

This is a Unity project, so it needs to be opened in the Unity Editor. To run the game, you would typically press the "Play" button in the editor.

**TODO:** Add specific instructions on which scene to open and any other setup required to run the game.

## Development Conventions

The project seems to be evolving from an interface-based design to a more robust abstract base class approach for the door system. The newer implementation in `SimpleDoor With DoorBase` is well-documented and includes clear setup instructions.

The code uses namespaces to organize the scripts, with `SPACE_GAME_0` and `SPACE_GAME_1` being the two main namespaces. The code also uses a custom utility `SPACE_UTIL` for logging.
