# CoffeePause - GameLauncher

A collection of classic games built with Windows Forms (.NET 8.0).

## Games Included

- **Pac-Man** - Collect dots while avoiding ghosts
- **Minesweeper** - Classic mine-sweeping puzzle game
- **Spider Solitaire** - Card game with 1, 2, or 4 suit variations
- **Sudoku** - Number puzzle with draft mode support

## Building the Project

### Prerequisites
- .NET 8.0 SDK or later
- Windows operating system (for running the application)

### Build Commands

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build GameLauncher/GameLauncher.csproj

# Run the application
dotnet run --project GameLauncher/GameLauncher.csproj
```

## Running Tests

```bash
# Run all unit tests
dotnet test GameLauncher.Tests/GameLauncher.Tests.csproj
```

The test project includes:
- **HighScoreManager tests** - Validates score saving, loading, and roundtrip persistence
- **Card value tests** - Ensures correct card value mappings for Spider Solitaire

## Asset Structure

Game assets are stored in the `assets/sprites/` directory:
- **logo.svg** - Application logo
- **pacman.svg** - Pac-Man sprite
- **strawberry.svg** - Power-up sprite for Pac-Man
- **ghost_*.svg** - Ghost sprites
- **food_dot.svg** - Dot sprite for Pac-Man
- **solitaire cards.jpg** - Card sprites for Spider Solitaire
- **Background.jpg** - Background image for the main menu

**Note**: Do not delete or modify files in the `assets/` directory as they are required for the games to function properly.

## High Scores

High scores are automatically saved to:
```
%APPDATA%/CoffeePause/
```

Each game maintains its own high score file with the top 10 scores.

## Game Controls

### Pac-Man
- **W/A/S/D** - Move Pac-Man
- **P** - Pause/Unpause
- **ESC** - Exit game

### Minesweeper
- **Left-click** - Reveal cell
- **Right-click** - Flag/Unflag cell

### Spider Solitaire
- **Drag and drop** - Move cards
- **Undo button** - Undo last move (up to 5 moves)
- **Hint button** - Show one possible move
- **Suit selector** - Choose 1, 2, or 4 suits

### Sudoku
- **Left-click + numpad** - Place final value
- **Ctrl + numpad** - Place draft (temporary) value
- **Backspace/Delete** - Clear cell

## Development

### Framework
- Target Framework: `net8.0-windows`
- UI Framework: Windows Forms
- Package Dependencies:
  - `Svg` (v3.3.0) - For SVG image rendering

### Project Structure
```
GameLauncher/
├── Program.cs              - Application entry point
├── MainMenuForm.cs         - Main menu UI
├── AssetManager.cs         - Asset loading and caching
├── HighScoreManager.cs     - Score persistence
├── GameSettings.cs         - Game configuration
├── PacManForm.cs           - Pac-Man game logic
├── MinesweeperForm.cs      - Minesweeper game logic
├── SpiderSolitaireForm.cs  - Spider Solitaire game logic
├── SudokuForm.cs           - Sudoku game logic
├── HighscoresForm.cs       - High scores display
└── SettingsForm.cs         - Settings UI
```

## Notes

- The project was upgraded from net7.0-windows to net8.0-windows to resolve SDK compatibility issues
- `EnableWindowsTargeting` is enabled to support cross-platform builds
- All compile errors (CS1009, CS1026) have been resolved
- String interpolation bugs in HighScoreManager have been fixed
