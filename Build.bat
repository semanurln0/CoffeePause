@echo off
echo Building CoffeePause Game Library...
echo.

dotnet publish Code/GameLauncher/GameLauncher.csproj -c Release -r win-x64 --self-contained false -o Build

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Build successful!
    echo Executable created at: Build\GameLauncher.exe
    echo.
    echo You can now run the game by:
    echo   1. Double-clicking Build\GameLauncher.exe
    echo.
    pause
) else (
    echo.
    echo Build failed! Make sure .NET 9.0 SDK is installed.
    echo.
    pause
)
