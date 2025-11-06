@echo off
echo Building CoffeePause Game Library...
echo.

dotnet publish GameLauncher/GameLauncher.csproj -c Release -r win-x64 --self-contained false -o ./publish

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Build successful!
    echo Executable created at: publish\GameLauncher.exe
    echo.
    echo You can now run the game by:
    echo   1. Double-clicking RunCoffeePause.bat
    echo   2. Running publish\GameLauncher.exe directly
    echo   3. Creating a shortcut with CreateShortcut.ps1
    echo.
    pause
) else (
    echo.
    echo Build failed! Make sure .NET 9.0 SDK is installed.
    echo.
    pause
)
