# CoffeePause Repository Architecture

## Overview

This repository contains **CoffeePause**, a Windows Forms game collection built with C# and .NET 9.0. It's a collection of four classic games with a unified launcher interface, featuring settings management, highscore tracking, and asset support - perfect for your coffee break!

## Repository Structure

```
CoffeePause/
├── .git/                  # Git version control
├── .gitignore            # Git ignore patterns
├── .vs/                  # Visual Studio cache
├── assets/               # Game assets (images, sprites)
│   ├── Background.jpg    # Main menu background
│   ├── logo.png/jpg      # CoffeePause logo (optional)
│   ├── sprites/          # Game sprites (Pac-Man assets)
│   └── solitaire cards.* # Card images
├── GameLauncher/         # Main application directory
│   ├── *.cs              # C# source files
│   ├── GameLauncher.csproj  # Project file
│   └── assets/           # Runtime assets (symlink/copy)
├── GameLauncher.sln      # Visual Studio solution file
└── README.md             # User documentation
```

## Technology Stack

- **Framework**: .NET 9.0
- **UI Framework**: Windows Forms
- **Target Platform**: Windows (net9.0-windows)
- **Language**: C# with implicit usings and nullable reference types enabled
- **Storage**: JSON-based file storage

## Core Components

### 1. Application Entry Point

**File**: `Program.cs` (5 lines)
- Uses C# 9+ top-level statements (no explicit Main method)
- Initializes Windows Forms application
- Launches `MainMenuForm` with CoffeePause branding

### 2. Main Menu (`MainMenuForm.cs`)

**Responsibilities**:
- Central navigation hub for all games
- Displays game selection buttons with CoffeePause branding
- Provides access to settings and highscores
- Manages background image and logo display

**Key Features**:
- **Logo support**: Displays logo from assets if available (logo.png/jpg)
- Custom background image support via `AssetManager`
- Graceful fallback to gradient background if assets not found
- Large, styled game buttons (Pac-Man, Minesweeper, Spider Solitaire, Sudoku)
- Settings and Highscores buttons
- Exit button
- Responsive layout

**Design Pattern**: Form-based UI with event-driven architecture

### 3. Game Forms

Each game is implemented as a separate Form class:

#### a. Pac-Man (`PacManForm.cs`)
- **Game Type**: Maze navigation
- **Controls**: WASD keys
- **Features**:
  - 10x20 character-based maze
  - Score tracking (10 points per dot)
  - **Ghost AI**: 3 ghosts with intelligent pathfinding
  - Timer-based game loop (100ms interval)
  - Custom rendering with double buffering and anti-aliasing
  - **Retry functionality**: Play again option on game over/win
- **State Management**: Character array for maze, position tracking, ghost movement AI

#### b. Minesweeper (`MinesweeperForm.cs`)
- **Game Type**: Logic puzzle
- **Controls**: Mouse (left-click reveal, right-click flag)
- **Features**:
  - Configurable grid size via settings
  - Configurable mine count
  - Adjacent mine calculation
  - Recursive cell revealing for empty cells
  - Win/loss detection
  - **Retry functionality**: Try again option on game over/win
- **Difficulty**: Dynamically adjusts based on settings

#### c. Spider Solitaire (`SpiderSolitaireForm.cs`)
- **Game Type**: Card game
- **Controls**: Mouse for card selection and movement
- **Features**:
  - 7-column layout
  - Deck management
  - **Proper card stacking**: Click-to-select, click-to-move mechanics
  - **Descending sequence validation**: Cards stack in descending order
  - **Complete sequence detection**: Auto-remove K-to-A sequences
  - Score system (starts at 500)
  - Move counter
  - Card rendering with fallback graphics
  - **New Game button**: Restart without closing
  - **Retry functionality**: Play again option on win
- **Data Structures**: List-based columns and deck with selection state tracking

#### d. Sudoku (`SudokuForm.cs`)
- **Game Type**: Number puzzle
- **Controls**: Mouse + Numpad/Number keys
- **Features**:
  - 9x9 grid with hardcoded puzzle
  - Cell selection system
  - Fixed/editable cell differentiation
  - Win condition validation
  - Visual highlighting for selected cell
  - **Retry functionality**: Play again option on win
- **Validation**: Checks row, column, and 3x3 box constraints

### 4. Support Systems

#### Settings Management (`GameSettings.cs`)

**Pattern**: Singleton pattern
**Storage**: JSON file in `%APPDATA%/CoffeePause/settings.json`

**Settings Structure**:
```csharp
public class GameSettings
{
    public bool SoundEnabled { get; set; } = true;
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;
    public MinesweeperSettings Minesweeper { get; set; } = new();
}

public class MinesweeperSettings
{
    public int GridWidth { get; set; } = 10;
    public int GridHeight { get; set; } = 10;
    public int MineCount { get; set; } = 15;
}

public enum DifficultyLevel { Easy, Medium, Hard }
```

**Key Features**:
- Persistent storage across sessions
- Automatic save on changes
- Default values if file doesn't exist or fails to load
- Error handling with console logging

#### Highscore Manager (`HighScoreManager.cs`)

**Pattern**: Singleton pattern
**Storage**: JSON file in `%APPDATA%/CoffeePause/highscores.json`

**Data Structure**:
```csharp
public class ScoreEntry
{
    public string PlayerName { get; set; } = "Player";
    public int Score { get; set; }
    public TimeSpan Time { get; set; }
    public DateTime Date { get; set; }
}
```

**Key Features**:
- Per-game score tracking (Dictionary<string, List<ScoreEntry>>)
- Top 10 scores per game
- Automatic sorting (by score descending, then time ascending)
- Persistent storage
- Date tracking for each entry

#### Asset Manager (`AssetManager.cs`)

**Pattern**: Singleton pattern with caching
**Purpose**: Centralized image loading and caching

**Key Features**:
- Image caching to avoid repeated disk I/O
- Support for multiple formats (PNG, JPG)
- Graceful handling of missing assets
- Background image loading with fallback
- Card image loading for Spider Solitaire
- Proper resource disposal

**Cache Strategy**: Dictionary-based in-memory cache

### 5. UI Forms

#### Settings Form (`SettingsForm.cs`)
- Sound enable/disable toggle
- Difficulty level selection (Easy/Medium/Hard)
- Custom Minesweeper configuration
  - Grid width (numeric input)
  - Grid height (numeric input)
  - Mine count (numeric input)
- Save/Cancel buttons
- Validates settings before applying

#### Highscores Form (`HighscoresForm.cs`)
- Displays top 10 scores per game
- Shows player name, score, time, and date
- Tabbed or list-based view for different games
- Read-only display

## Design Patterns Used

### 1. Singleton Pattern
- `GameSettings.Instance`
- `HighScoreManager.Instance`
- `AssetManager.Instance`

**Rationale**: Ensures single source of truth for global state

### 2. Form-Based Architecture
Each game and screen is a separate `Form` class
- Isolation of game logic
- Easy navigation between screens
- Independent lifecycle management

### 3. Event-Driven Programming
- Mouse event handlers for game interaction
- Keyboard event handlers for controls
- Timer events for game loops
- Button click events for navigation

### 4. Double Buffering
Used in games with frequent redraws (Pac-Man, Spider Solitaire)
- Prevents flickering
- Smoother animations

### 5. Data Persistence
JSON serialization for:
- Settings storage
- Highscore storage
- Easy to read/debug
- Human-editable configuration

## Data Flow

### Application Startup
```
Program.cs
  └─> MainMenuForm
        ├─> Loads background via AssetManager
        └─> Displays game selection buttons
```

### Game Launch Flow
```
MainMenuForm (button click)
  └─> Creates new GameForm instance
        ├─> Loads settings from GameSettings.Instance
        ├─> Initializes game state
        ├─> Starts game loop (if applicable)
        └─> Shows form
```

### Settings Flow
```
SettingsForm
  ├─> Reads from GameSettings.Instance
  ├─> User modifies settings
  └─> GameSettings.Instance.Save()
        └─> Writes to %APPDATA%/CoffeePause/settings.json
```

### Highscore Flow
```
Game completes
  └─> HighScoreManager.Instance.AddScore()
        ├─> Adds new entry
        ├─> Sorts and trims to top 10
        └─> Saves to %APPDATA%/CoffeePause/highscores.json
```

## File Organization

### Source Files (11 C# files, 1625 lines total)

1. **Program.cs** (5 lines) - Entry point
2. **MainMenuForm.cs** (161 lines) - Main launcher UI
3. **PacManForm.cs** (172 lines) - Pac-Man game implementation
4. **MinesweeperForm.cs** (273 lines) - Minesweeper game implementation
5. **SpiderSolitaireForm.cs** (215 lines) - Spider Solitaire implementation
6. **SudokuForm.cs** (211 lines) - Sudoku game implementation
7. **SettingsForm.cs** (209 lines) - Settings configuration UI
8. **HighscoresForm.cs** (116 lines) - Highscore display UI
9. **GameSettings.cs** (95 lines) - Settings management and persistence
10. **HighScoreManager.cs** (99 lines) - Highscore tracking and persistence
11. **AssetManager.cs** (69 lines) - Image asset loading and caching

**Configuration Files**:
- **GameLauncher.csproj** - MSBuild project configuration (XML)

### Assets

**Background Images**:
- `assets/Background.jpg` - Main menu background

**Sprites** (Pac-Man themed):
- `assets/sprites/pacman_Version3.svg`
- `assets/sprites/ghost_red_normal_Version4.svg`
- `assets/sprites/ghost_blue_vulnerable_Version4.svg`
- `assets/sprites/sprites_strawberry_Version3.svg`
- `assets/sprites/food_dot_Version3.svg`

**Card Images**:
- `assets/solitaire cards.jpg`
- `assets/solitaire cards.eps`

## State Management

### Global State
- **GameSettings**: Singleton, persisted to disk
- **HighScoreManager**: Singleton, persisted to disk
- **AssetManager**: Singleton, in-memory cache

### Game-Specific State
Each game form maintains its own state:
- Board/maze representation
- Player position/selection
- Score and time tracking
- Game status (running/won/lost)

### No Shared Game State
Games are completely independent - no cross-game state sharing

## Error Handling

### Strategy
- Try-catch blocks around I/O operations
- Console.WriteLine for error logging
- Graceful degradation (fallback graphics if assets missing)
- Default values for failed settings/highscore loads

### Asset Loading
```csharp
AssetManager.LoadImage(filename)
  ├─> Try to load image
  ├─> Cache if successful
  └─> Return null if failed (caller handles null)
```

## Build Configuration

**Project Type**: WinExe (Windows executable)
**Target Framework**: net9.0-windows
**Features Enabled**:
- ImplicitUsings
- Nullable reference types
- UseWindowsForms
- EnableWindowsTargeting

## User Data Storage

### Windows Locations
- Settings: `%APPDATA%\CoffeePause\settings.json`
- Highscores: `%APPDATA%\CoffeePause\highscores.json`

**Structure Created Automatically**:
```
C:\Users\[Username]\AppData\Roaming\CoffeePause\
  ├── settings.json
  └── highscores.json
```

## Rendering Approach

### Custom Drawing
- Games use `Paint` events for custom rendering
- GDI+ (System.Drawing) for graphics
- Manual cell/card/maze rendering

### Double Buffering
- Enabled on forms with frequent updates
- Reduces flicker
- Smoother visual experience

## Game Logic Highlights

### Pac-Man
- Character-based maze representation
- Timer-driven updates (50ms)
- Collision detection against walls
- Dot collection scoring

### Minesweeper
- Random mine placement
- Adjacent mine calculation for all cells
- Recursive reveal for empty cells
- Flag placement without revealing

### Spider Solitaire
- Card object model with Suit/Value
- Column-based layout
- Deck drawing system
- Score decreases with moves

### Sudoku
- Hardcoded puzzle (future: could randomize)
- Distinction between fixed and editable cells
- Row/column/box validation
- Visual cell selection feedback

## Extensibility Points

### Adding New Games
1. Create new Form class inheriting from `Form`
2. Implement game logic
3. Add button in `MainMenuForm`
4. Register in `HighScoreManager` for scoring

### Adding Settings
1. Add properties to `GameSettings` class
2. Update `SettingsForm` UI
3. Use in game forms via `GameSettings.Instance`

### Asset Support
1. Place assets in `assets/` directory
2. Load via `AssetManager.LoadImage()`
3. Handle null return for missing assets

## Testing Approach

**Current State**: No automated tests
**Manual Testing**: Build and run each game
**Build Verification**: `dotnet build` in GameLauncher directory

## Future Considerations

### Potential Improvements
1. **Localization**: Multi-language support (currently English)
2. **Sound Effects**: Framework in place (SoundEnabled setting)
3. **Randomized Sudoku**: Currently uses hardcoded puzzle
4. **Multiplayer**: Player name support exists in highscores
5. **Themes**: Color schemes, custom card designs
6. **More Games**: Architecture supports easy addition
7. **Unit Tests**: No test infrastructure currently
8. **AI Opponents**: For applicable games
9. **Achievements**: Track milestones beyond highscores
10. **Replay System**: Save/replay game sessions

### Known Limitations
1. **Windows Only**: Windows Forms is Windows-specific
2. **No Linux/Mac Support**: Can build but not run GUI
3. **Hardcoded Puzzles**: Sudoku uses same puzzle each time
4. **Simple AI**: No computer opponents
5. **Limited Asset Formats**: PNG/JPG only

## Recent Enhancements

### Version 2.0 - CoffeePause Rebranding (2025)

**New Features**:
1. **Rebranded to CoffeePause** - Complete application rename with coffee break theme
2. **Logo Support** - Displays custom logo from assets (logo.png/jpg) on main menu
3. **Ghost AI in Pac-Man** - Three intelligent ghosts with pathfinding behavior
4. **Fixed Spider Solitaire** - Proper card stacking with click-to-select/click-to-move mechanics
5. **Retry Functionality** - All games now offer "Play Again?" option on game over/win
6. **Enhanced Visual Polish** - Better anti-aliasing, improved UI layouts

**Bug Fixes**:
- Pac-Man: Added missing ghost AI and collision detection
- Spider Solitaire: Fixed card disappearing bug - cards now properly stack in descending order
- Spider Solitaire: Added sequence detection and removal for K-to-A same-suit sequences
- All games: Replaced simple "OK" dialogs with "Yes/No" retry options

**Technical Improvements**:
- Updated all namespace references from GameLauncher to CoffeePause
- Improved main menu with logo support and better layout
- Enhanced ghost rendering with eyes and proper colors
- Added New Game button to Spider Solitaire for quick restart
- Better background fallback (gradient instead of solid color)

## Development Workflow

### Building
```bash
cd GameLauncher
dotnet build
```

### Running
```bash
cd GameLauncher
dotnet run
```

### Requirements
- .NET 9.0 SDK or higher
- Windows OS (for running)
- Visual Studio / VS Code / Rider (recommended)

## Code Style

- Implicit usings enabled
- Nullable reference types enabled
- Field naming: `_privateField` convention
- Property naming: `PascalCase`
- Event handlers: `Control_EventName` pattern
- Double buffering for graphics-intensive forms

## Summary

This is a well-structured, educational game launcher project that demonstrates:
- Windows Forms application development
- Game loop implementation
- State management with singletons
- Persistent data storage with JSON
- Asset management and caching
- Event-driven UI programming
- Basic game algorithms (pathfinding, puzzle solving, etc.)

The codebase is clean, maintainable, and designed for easy extension with additional games or features. The separation of concerns (game logic, settings, highscores, assets) makes it an excellent reference for learning Windows Forms development and game programming patterns.
