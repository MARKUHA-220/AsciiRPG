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
ASCII DnD-подобная RPG на C# с архитектурой под DM/игроков, LAN-сигналом готовности и системой сохранений.

## Что реализовано

- Генерация мира (комнаты + коридоры/туннели) и модификаторы комнат.
- Бои по инициативе (пошагово), враги, статусы.
- Сундуки, триггеры, система лута.
- Инвентарь и экипировка (оружие/броня).
- Классы и расы персонажа.
- Редактор персонажа через файл сохранения.
- Туман войны отдельно для каждого игрока.
- Меню DM: выбор готовности, запуск хоста, режим редактора.
- LAN мультиплеер (хост/клиент; события готовности).
- Сохранение/загрузка игры и персонажа.

## Понятная структура

- `src/AsciiRPG/Core` — модели (карта, персонажи, инвентарь, статусы).
- `src/AsciiRPG/Gameplay` — генерация, бой, лут, игровой движок.
- `src/AsciiRPG/Network` — LAN сессия (TCP host/client).
- `src/AsciiRPG/Persistence` — сохранения JSON.
- `src/AsciiRPG/UI` — консольное меню и создание персонажа.
- `saves/` — примеры сохранений.
- `tools/build_windows.bat` — подготовка EXE.
- `build/` — артефакты публикации.

## Как получить EXE

> Требуется .NET SDK 8+

### Windows

```bat
tools\build_windows.bat
```

Result EXE:
После этого EXE будет здесь:

```text
build/win-x64/AsciiRPG.exe
```

### Manual publish
### Вручную

```bash
cd src/AsciiRPG
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ../../build/win-x64
```

## Run from source
## Запуск из исходников

```bash
cd src/AsciiRPG
dotnet run
```
