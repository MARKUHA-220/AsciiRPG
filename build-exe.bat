@echo off
setlocal

dotnet publish AsciiRPG.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
if errorlevel 1 exit /b 1

echo EXE собран: bin\Release\net8.0\win-x64\publish\AsciiRPG.exe