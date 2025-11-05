# CoffeePause
Game Collection - Windows Forms Application

## CoffeePause

A C# Windows Forms application featuring 4 classic games - perfect for your coffee break!

### Games

1. **Pac-Man** - Navigate maze and collect dots (WASD controls)
2. **Minesweeper** - Find mines and place flags (Mouse controls)
3. **Spider Solitaire** - Arrange cards in order (Mouse controls)
4. **Sudoku** - Solve the 9x9 puzzle (Mouse + Numpad)

### Features

- **Global Settings**
  - Sound on/off toggle
  - Difficulty levels (Easy, Medium, Hard)
  - Custom Minesweeper grid size and mine count

- **Highscore Tracking**
  - Top 10 scores per game
  - Time tracking
  - Persistent storage

- **Asset Support**
  - Background image for main menu
  - Custom card designs for Spider Solitaire
  - Automatic fallback to simple graphics if assets not found

### How to Run

```bash
cd GameLauncher
dotnet run
```

**Note:** This is a Windows Forms application and requires Windows to run. On Linux/Mac, you can build but not execute the GUI.

### Requirements

- .NET 9.0 SDK or higher
- Windows OS (for running the application)

### Controls

**Main Menu:**
- Click game buttons to launch
- Settings button to configure game options
- Highscores button to view top scores

**Pac-Man:**
- W/A/S/D: Move
- ESC: Return to main menu

**Minesweeper:**
- Left-click: Reveal cell
- Right-click: Place/remove flag

**Spider Solitaire:**
- Click cards to select/move
- "Draw from Deck" button to deal new cards

**Sudoku:**
- Click cell to select
- Number keys (1-9) or Numpad: Enter number
- 0/Backspace/Delete: Clear cell

### Assets

Place image assets in the `assets/` folder:
- `logo.png` or `logo.jpg` - CoffeePause logo for main menu (optional)
- `background.png` or `background.jpg` - Main menu background
- Card images in `assets/cards/` folder (optional - will use simple graphics as fallback)

If assets are not provided, the application will use simple fallback graphics.

### Data Storage

Settings and highscores are stored in:
- Windows: `%APPDATA%\CoffeePause\`
  - `settings.json` - Game settings
  - `highscores.json` - Highscore data

## For Developers

For detailed technical documentation including architecture, design patterns, code organization, and extensibility guidelines, see [ARCHITECTURE.md](ARCHITECTURE.md).
