# CoffeePause - Game Library

A collection of classic games built with .NET WinForms.

## Games Included

### 1. Pac-Man
- Two food types: regular (1pt) and supreme (5pt)
- Eating supreme food makes ghosts vulnerable
- Eating vulnerable ghosts gives 10 points
- Difficulty settings: Easy, Medium, Hard
- Ghost AI: Chase when normal, flee when vulnerable

### 2. Sudoku
- Random puzzle generation
- Draft mode (Ctrl + Number) for notes in red
- Submit mode (Number) for answers in blue
- Solution validation
- High score tracking

### 3. Minesweeper
- Left click to reveal cells
- Right click to place flags
- Difficulty presets and custom grid sizes
- Mine counter display
- Score based on difficulty and grid size

### 4. Spider Solitaire
- Suit options: 1, 2, or 4 suits
- Drag and drop cards
- Hint system for valid moves
- Undo last 10 moves
- Complete 13-card sequences to win

## Features

- **High Score System**: Each game tracks top 10 scores with player names and dates
- **Persistent Storage**: Scores saved to `%APPDATA%/CoffeePause` in JSON format
- **Settings**: Per-game difficulty and customization options
- **Clean UI**: Modern, colorful main menu with easy game selection

## Building and Running

### Prerequisites
- .NET 9.0 SDK or later
- Windows OS (for WinForms support)

### Build Instructions

```bash
# Build the solution
dotnet build CoffeePause.sln

# Run the application
dotnet run --project GameLauncher/GameLauncher.csproj
```

### Creating a Standalone Executable

```bash
# Publish as self-contained executable
dotnet publish GameLauncher/GameLauncher.csproj -c Release -r win-x64 --self-contained

# The executable will be in:
# GameLauncher/bin/Release/net9.0-windows/win-x64/publish/GameLauncher.exe
```

## Project Structure

```
CoffeePause/
├── GameLauncher/
│   ├── Program.cs                 # Application entry point
│   ├── MainForm.cs                # Main menu
│   ├── AssetManager.cs            # SVG asset loading
│   ├── HighScoreManager.cs        # Score persistence
│   ├── PacManForm.cs              # Pac-Man game
│   ├── SudokuForm.cs              # Sudoku game
│   ├── MinesweeperForm.cs         # Minesweeper game
│   └── SpiderSolitaireForm.cs     # Spider Solitaire game
├── assets/
│   └── sprites/                   # SVG and image assets
├── CoffeePause.sln                # Solution file
└── README.md                      # This file
```

## Controls

### Pac-Man
- Arrow keys or WASD: Move Pac-Man
- Eat all food to win
- Avoid ghosts (unless they're vulnerable)

### Sudoku
- Click a cell to select it
- Number keys (1-9): Enter value (blue)
- Ctrl + Number keys: Add draft note (red)
- Backspace: Clear cell

### Minesweeper
- Left click: Reveal cell
- Right click: Place/remove flag
- Reveal all non-mine cells to win

### Spider Solitaire
- Drag and drop cards between columns
- Click stock to deal more cards
- Hint button: Suggests a move
- Undo button: Undo last move (up to 10)

## License

This is a personal project for educational and entertainment purposes.

## Credits

Built with:
- .NET 9.0 WinForms
- Newtonsoft.Json for score persistence
- Svg library for SVG rendering
