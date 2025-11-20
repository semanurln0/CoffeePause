# CoffeePause - Requirements Analysis

This document tracks the implementation status of formal requirements for the CoffeePause project.

## Requirements Status

### ✅ Implemented Requirements

#### 1. Partial Class (0.5 points) - **IMPLEMENTED**
**Location:** Multiple Form classes
```csharp
// File: Code/GameLauncher/MainForm.cs (line 3)
public partial class MainForm : Form

// File: Code/GameLauncher/MinesweeperForm.cs (line 3)
public partial class MinesweeperForm : Form

// File: Code/GameLauncher/SudokuForm.cs (line 3)
public partial class SudokuForm : Form

// File: Code/GameLauncher/PacManForm.cs (line 3)
public partial class PacManForm : Form

// File: Code/GameLauncher/SpiderSolitaireForm.cs (line 3)
public partial class SpiderSolitaireForm : Form
```

#### 2. Data Structures from System.Collections or System.Collections.Generic (1 point) - **IMPLEMENTED**
**Location:** Throughout the codebase
```csharp
// File: Code/GameLauncher/HighScoreManager.cs (line 18)
public List<ScoreEntry> LoadScores(string gameName)

// File: Code/GameLauncher/PacManForm.cs (line 13)
private List<Ghost> ghosts = new List<Ghost>();

// File: Code/GameLauncher/SpiderSolitaireForm.cs (line 9)
private List<List<Card>> tableau = new List<List<Card>>();

// File: Code/GameLauncher/SpiderSolitaireForm.cs (line 12)
private Stack<GameState> undoStack = new Stack<GameState>();

// File: Code/GameLauncher/SudokuForm.cs (line 11)
private HashSet<string>[,] drafts = new HashSet<string>[GridSize, GridSize];
```

#### 3. The Project Consists of More Than One Module (assembly) (1 point) - **IMPLEMENTED**
**Location:** Solution structure
```
Two assemblies in the solution:
1. GameLauncher (main application) - Code/GameLauncher/GameLauncher.csproj
2. GameLauncher.Tests (test project) - Code/tests/GameLauncher.Tests/GameLauncher.Tests.csproj
```

#### 4. Delegates or Lambda Functions (1.5 points) - **IMPLEMENTED**
**Location:** Multiple locations
```csharp
// File: Code/GameLauncher/MainForm.cs (line 40)
exitMenuItem.Click += (s, e) => Application.Exit();

// File: Code/GameLauncher/MainForm.cs (line 128)
pacmanBtn.Click += (s, e) => LaunchGame("PacMan");

// File: Code/GameLauncher/HighScoreManager.cs (line 39)
scores = scores.OrderByDescending(s => s.Score).Take(10).ToList();

// File: Code/GameLauncher/SpiderSolitaireForm.cs (line 159)
deck = deck.OrderBy(c => rand.Next()).ToList();

// File: Code/GameLauncher/SpiderSolitaireForm.cs (line 649)
Tableau = tableau.Select(col => col.Select(c => c.Clone()).ToList()).ToList()
```

#### 5. Operators ?., ?[], ??, or ??= (0.5 points) - **IMPLEMENTED**
**Location:** Throughout the codebase
```csharp
// File: Code/GameLauncher/HighScoreManager.cs (line 27)
return JsonConvert.DeserializeObject<List<ScoreEntry>>(json) ?? new List<ScoreEntry>();

// File: Code/GameLauncher/IconConverter.cs (line 8)
public static Icon? LoadIconFromSvg(string svgPath, int size = 256)

// File: Code/GameLauncher/MainForm.cs (line 182)
gameForm?.ShowDialog(this);

// File: Code/GameLauncher/MinesweeperForm.cs (line 148)
gamePanel?.Invalidate();

// File: Code/GameLauncher/PacManForm.cs (line 181)
gamePanel?.Invalidate();
```

#### 6. Pattern Matching (1 point) - **IMPLEMENTED**
**Location:** Multiple switch expressions
```csharp
// File: Code/GameLauncher/MainForm.cs (line 171-178)
Form? gameForm = gameName switch
{
    "PacMan" => new PacManForm(),
    "Sudoku" => new SudokuForm(),
    "Minesweeper" => new MinesweeperForm(),
    "SpiderSolitaire" => new SpiderSolitaireForm(),
    _ => null
};

// File: Code/GameLauncher/PacManForm.cs (line 313-320)
return dir switch
{
    Direction.Up => new Point(pos.X, Math.Max(0, pos.Y - 1)),
    Direction.Down => new Point(pos.X, Math.Min(GridHeight - 1, pos.Y + 1)),
    Direction.Left => new Point(Math.Max(0, pos.X - 1), pos.Y),
    Direction.Right => new Point(Math.Min(GridWidth - 1, pos.X + 1), pos.Y),
    _ => pos
};

// File: Code/GameLauncher/MinesweeperForm.cs (line 290-297)
var color = cell.AdjacentMines switch
{
    1 => Brushes.Blue,
    2 => Brushes.Green,
    3 => Brushes.Red,
    4 => Brushes.DarkBlue,
    5 => Brushes.DarkRed,
    _ => Brushes.Black
};
```

#### 7. The 'is' Operator (0.5 points) - **IMPLEMENTED**
**Location:** MainForm.cs
```csharp
// File: Code/GameLauncher/MainForm.cs (line 217-227)
if (control is Panel panel && panel.BackgroundImage != null)
{
    // Skip panel adjustments - already docked
    continue;
}
else if (control.Name == "gamesPanel" || control is TableLayoutPanel)
{
    control.Location = new Point(Math.Max(10, centerX), Math.Max(150, centerY));
}
```

#### 8. Range Type (0.5 points) - **IMPLEMENTED**
**Location:** SpiderSolitaireForm.cs
```csharp
// File: Code/GameLauncher/IconConverter.cs (line 67)
using var icon = Icon.FromHandle(bitmaps[^1].GetHicon());

// File: Code/GameLauncher/SpiderSolitaireForm.cs (line 447-450)
var cardsToMove = tableau[fromCol].GetRange(cardIndex,
    tableau[fromCol].Count - cardIndex);
tableau[fromCol].RemoveRange(cardIndex,
    tableau[fromCol].Count - cardIndex);
```

### ❌ Not Implemented Requirements

#### 9. Created and Applied Custom Interface (0.5 points) - **NOT IMPLEMENTED**
No custom interfaces have been created in the codebase. Only built-in .NET interfaces are used.

#### 10. IComparable<T> Implementation (0.5 points) - **NOT IMPLEMENTED**
No classes implement IComparable<T> interface for custom sorting logic.

#### 11. IEquatable<T> Implementation (0.5 points) - **NOT IMPLEMENTED**
No classes implement IEquatable<T> interface for custom equality comparison.

#### 12. IFormattable Implementation (1 point) - **NOT IMPLEMENTED**
No classes implement IFormattable interface for custom string formatting.

#### 13. Switch with 'when' Keyword (0.5 points) - **NOT IMPLEMENTED**
Pattern matching with switch is used, but not with the 'when' guard clause.

#### 14. Sealed Class (0.5 points) - **NOT IMPLEMENTED**
No sealed classes are present in the codebase. All classes can be inherited.

#### 15. Abstract Class (0.5 points) - **NOT IMPLEMENTED**
No abstract classes are defined in the codebase.

#### 16. Static Constructor (1 point) - **NOT IMPLEMENTED**
No static constructors are used in any classes.

#### 17. Deconstructor (0.5 points) - **NOT IMPLEMENTED**
No classes implement deconstruction methods (Deconstruct).

#### 18. Operator Overloading (0.5 points) - **NOT IMPLEMENTED**
No custom operator overloading (==, !=, +, -, etc.) is implemented.

#### 19. Default and Named Arguments (0.5 points) - **NOT IMPLEMENTED**
Methods use default arguments (e.g., `LoadIconFromSvg(string svgPath, int size = 256)`) but named arguments are not explicitly demonstrated in method calls.

#### 20. Params Keyword (0.5 points) - **NOT IMPLEMENTED**
No methods use the params keyword for variable-length argument lists.

#### 21. Out Arguments Initialization (1 point) - **NOT IMPLEMENTED**
No methods use out parameters for initialization or returning multiple values.

#### 22. Bitwise Operations (1 point) - **NOT IMPLEMENTED**
No bitwise operations (&, |, ^, ~, <<, >>) are used in the codebase.

## Summary

**Total Points Implemented: 8.0 out of 17.0 points**

### Implemented (8 points):
- Partial class (0.5)
- Data structures from System.Collections.Generic (1.0)
- More than one module/assembly (1.0)
- Delegates/lambda functions (1.5)
- Null-conditional operators (0.5)
- Pattern matching (1.0)
- 'is' operator (0.5)
- Range type (0.5)
- Partial implementation of default arguments (usage not demonstrated)

### Not Implemented (9 points):
- Custom interface (0.5)
- IComparable<T> (0.5)
- IEquatable<T> (0.5)
- IFormattable (1.0)
- Switch with 'when' keyword (0.5)
- Sealed class (0.5)
- Abstract class (0.5)
- Static constructor (1.0)
- Deconstructor (0.5)
- Operator overloading (0.5)
- Named arguments (proper demonstration) (0.5)
- Params keyword (0.5)
- Out arguments (1.0)
- Bitwise operations (1.0)

## Notes

The project is a well-structured Windows Forms game launcher application with four games: Pac-Man, Sudoku, Minesweeper, and Spider Solitaire. The codebase demonstrates good use of modern C# features but is missing several specific requirements listed above. These could be easily added to satisfy the formal requirements while maintaining the existing functionality.
