@echo off
echo Building CoffeePause Game Library...
echo.

dotnet publish Code/GameLauncher/GameLauncher.csproj -c Release -r win-x64 --self-contained false -o Build

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Copying executable to main folder...
    copy Build\GameLauncher.exe . >nul
    echo.
    echo Build successful!
    echo Executable created at: GameLauncher.exe
    echo Other build files in: Build\
    echo.
    echo You can now run the game by:
    echo   1. Double-clicking GameLauncher.exe
    echo.
    pause
) else (
    echo.
    echo Build failed! Make sure .NET 9.0 SDK is installed.
    echo.
    pause
)
