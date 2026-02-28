# AsciiRPG

ASCII DnD-like RPG in C# with DM/player flow, LAN ready signaling, and save system.

## Implemented features

- Procedural world generation (rooms + corridor tunnels) with room modifiers.
- Initiative-based turn combat, enemies, and statuses.
- Chests, triggers, and loot system.
- Inventory and equipment (weapon/armor).
- Character classes and races.
- Character editor through save files.
- Per-player fog of war.
- DM menu with readiness display, host start, and editor mode.
- LAN multiplayer (host/client; readiness events).
- Save/load for game and characters.
- Console text rendering update: app forces UTF-8 input/output and all UI prompts are ASCII-safe.

## Project structure

- `src/AsciiRPG/Core` — core models (map, characters, inventory, statuses).
- `src/AsciiRPG/Gameplay` — generation, combat, loot, game engine.
- `src/AsciiRPG/Network` — LAN session (TCP host/client).
- `src/AsciiRPG/Persistence` — JSON save/load.
- `src/AsciiRPG/UI` — console menu and character creation.
- `saves/` — sample save files.
- `tools/build_windows.bat` — EXE build script.
- `build/` — publish artifacts.

## Build EXE

> Requires .NET SDK 8+

### Windows

```bat
tools\build_windows.bat
```

Result EXE:

```text
build/win-x64/AsciiRPG.exe
```

### Manual publish

```bash
cd src/AsciiRPG
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ../../build/win-x64
```

## Run from source

```bash
cd src/AsciiRPG
dotnet run
```
