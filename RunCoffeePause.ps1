# CoffeePause Launcher
Set-Location "$PSScriptRoot\GameLauncher\bin\Release\net9.0-windows"
Start-Process dotnet -ArgumentList "CoffeePause.dll" -NoNewWindow
