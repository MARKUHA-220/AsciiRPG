@echo off
setlocal
REM Требуется установленный .NET SDK 8+
cd /d %~dp0\..\src\AsciiRPG

dotnet restore
if errorlevel 1 exit /b 1

dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ..\..\build\win-x64
if errorlevel 1 exit /b 1

echo Done. EXE: build\win-x64\AsciiRPG.exe
