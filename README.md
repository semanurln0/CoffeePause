# CoffeePause - Game Library

A minimal, clean WinForms application featuring four classic games:
- **Pac-Man**: Chase ghosts and eat food
- **Sudoku**: Solve number puzzles
- **Minesweeper**: Find all the mines
- **Spider Solitaire**: Card game patience

## Features

### Global Features
- **ESC Key**: Return to main menu or quit the application
- **P Key**: Pause any game with a transparent overlay
- **Try Again**: Each game asks if you want to play again when finished

### Game-Specific Features

#### Pac-Man
- Two food types: regular (1 point) and supreme strawberry (5 points)
- Eating supreme food makes ghosts vulnerable (blue) for a limited time
- Eating vulnerable ghosts gives 10 points
- Ghosts chase Pac-Man normally, but flee when vulnerable
- Settings: Easy/Medium/Hard difficulty
- Scoreboard with top 10 scores

#### Sudoku
- Right-click + Number or Ctrl + Number: Draft mode (red numbers)
- Left-click + Number: Submit mode (blue numbers)
- Each new game generates a random puzzle
- Settings and scoreboard available
- Check solution button to verify

#### Minesweeper
- Right-click: Flag a cell
- Left-click: Reveal a cell
- Settings: Easy/Medium/Hard difficulty with different grid sizes
- Scoreboard with top 10 scores

#### Spider Solitaire
- Settings: 1, 2, or 4 suits (1 suit is default)
- Click cards to auto-move when possible
- Click stock area to deal new cards
- Hint button shows one possible move
- Undo feature (simplified)
- Remove completed 13-card same-suit sequences for points
- Scoreboard with top 10 scores

## Building

Requirements:
- .NET 9.0 or later
- Windows (or Windows targeting enabled on Linux)

Build command:
```bash
dotnet build --configuration Release
```

## Running

On Windows:
```bash
cd GameLauncher/bin/Release/net9.0-windows
dotnet CoffeePause.dll
```

Or use the provided launch script.

## Project Structure

```
CoffeePause/
├── assets/
│   └── sprites/          # SVG game assets
├── GameLauncher/
│   ├── Program.cs        # Entry point
│   ├── MainForm.cs       # Main menu
│   ├── AssetManager.cs   # SVG asset loading
│   ├── HighScoreManager.cs  # Score persistence
│   ├── PacManForm.cs     # Pac-Man game
│   ├── SudokuForm.cs     # Sudoku game
│   ├── MinesweeperForm.cs   # Minesweeper game
│   └── SpiderSolitaireForm.cs  # Spider Solitaire game
└── CoffeePause.sln       # Solution file
```

## Data Storage

High scores are stored in JSON format in:
- Windows: `%APPDATA%/CoffeePause/`
- Each game has its own score file with atomic saves (write to temp, then replace)

## Assets

All SVG assets are provided and should not be modified. They include:
- Pac-Man character
- Ghost sprites (normal and vulnerable)
- Food dots and supreme food (strawberry)
- Project logo

Enjoy your coffee break! ☕
