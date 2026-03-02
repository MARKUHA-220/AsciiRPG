#!/usr/bin/env bash
set -euo pipefail

dotnet publish AsciiRPG.csproj \
  -c Release \
  -r win-x64 \
  --self-contained true \
  /p:PublishSingleFile=true

echo "EXE собран: bin/Release/net8.0/win-x64/publish/AsciiRPG.exe"
