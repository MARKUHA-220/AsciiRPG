# AsciiRPG
Сетевая или однопользовательская игра на C# с дизайном ASCII.

## Запуск
```bash
dotnet run --project AsciiRPG.csproj
```

## Сборка `.exe` (Windows)
Быстрый способ:
```bash
./build-exe.sh
```

Или вручную:
```bash
dotnet publish AsciiRPG.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

Готовый файл:
`bin/Release/net8.0/win-x64/publish/AsciiRPG.exe`
