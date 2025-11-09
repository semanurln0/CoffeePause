# How to Run CoffeePause

## For Windows Users

### Method 1: Using the Batch Script
1. Double-click `RunCoffeePause.bat` in the root directory
2. The game launcher will start automatically

### Method 2: Using PowerShell
1. Right-click `RunCoffeePause.ps1`
2. Select "Run with PowerShell"

### Method 3: Manual Launch
1. Open a command prompt or PowerShell
2. Navigate to the project root directory
3. Run:
   ```
   cd GameLauncher\bin\Release\net9.0-windows
   dotnet CoffeePause.dll
   ```

## Building from Source

Requirements:
- .NET 9.0 SDK or later

Steps:
1. Open a command prompt in the project root
2. Run: `dotnet build --configuration Release`
3. The executable will be in `GameLauncher\bin\Release\net9.0-windows\`

## Controls

### Global Controls (All Games)
- **ESC**: Return to main menu or quit the application
- **P**: Pause the game (shows transparent "PAUSED" overlay)

### Pac-Man
- **Arrow Keys**: Move Pac-Man (Up, Down, Left, Right)
- Eat all food dots and strawberries to win
- Avoid ghosts unless they're blue (vulnerable)

### Sudoku
- **Number Keys (1-9)**: Enter number in selected cell (blue color)
- **Ctrl + Number Keys**: Draft mode (red color)
- **Backspace/Delete**: Clear cell
- **Click "Check Solution"**: Verify if puzzle is solved

### Minesweeper
- **Left Click**: Reveal a cell
- **Right Click**: Flag/unflag a cell as a mine
- Reveal all non-mine cells to win

### Spider Solitaire
- **Click on cards**: Auto-move to valid destination
- **Click on stock area** (top-left): Deal new cards to all columns
- **Hint button**: Shows a possible move
- **Undo button**: Undo last move (simplified)

## Troubleshooting

### Game doesn't start
- Make sure .NET 9.0 or later is installed
- Check that all files in `assets/sprites/` are present

### Assets not loading
- Verify that the `assets/sprites/` folder exists in the game directory
- Rebuild the project to ensure assets are copied

### Scores not saving
- Make sure you have write permissions to `%APPDATA%\CoffeePause\`
- On first run, the folder will be created automatically

Enjoy your coffee break! ☕
