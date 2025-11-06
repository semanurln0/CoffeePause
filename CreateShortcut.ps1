# PowerShell script to create a desktop shortcut for CoffeePause
# Run this script on Windows after building the project

$WshShell = New-Object -comObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut("$PSScriptRoot\CoffeePause.lnk")
$Shortcut.TargetPath = "$PSScriptRoot\publish\GameLauncher.exe"
$Shortcut.WorkingDirectory = "$PSScriptRoot\publish"
$Shortcut.Description = "CoffeePause Game Library"
$Shortcut.Save()

Write-Host "Shortcut created successfully: CoffeePause.lnk"
Write-Host "You can now move this shortcut to your desktop or start menu."
