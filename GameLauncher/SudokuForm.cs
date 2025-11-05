using System.Drawing;
using System.Windows.Forms;

namespace CoffeePause;

public class SudokuForm : Form
{
    private int[,] board = new int[9, 9];
    private bool[,] fixedCells = new bool[9, 9];
    private int selectedX = 0;
    private int selectedY = 0;
    private const int CellSize = 50;
    private Panel gamePanel = null!;
    private DateTime startTime;

    public SudokuForm()
    {
        InitializeComponent();
        InitializeGame();
        startTime = DateTime.Now;
    }

    private void InitializeComponent()
    {
        this.Text = "Sudoku";
        this.Size = new Size(520, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.KeyPreview = true;
        this.KeyDown += SudokuForm_KeyDown;

        var infoLabel = new Label
        {
            Text = "Click cell and use mouse/numpad to enter numbers 1-9",
            Location = new Point(10, 10),
            Size = new Size(480, 25),
            Font = new Font("Arial", 10)
        };
        this.Controls.Add(infoLabel);

        gamePanel = new Panel
        {
            Location = new Point(10, 40),
            Size = new Size(9 * CellSize, 9 * CellSize),
            BorderStyle = BorderStyle.FixedSingle
        };
        gamePanel.Paint += GamePanel_Paint;
        gamePanel.MouseClick += GamePanel_MouseClick;
        this.Controls.Add(gamePanel);
    }

    private void InitializeGame()
    {
        int[,] puzzle = new int[,]
        {
            {5, 3, 0, 0, 7, 0, 0, 0, 0},
            {6, 0, 0, 1, 9, 5, 0, 0, 0},
            {0, 9, 8, 0, 0, 0, 0, 6, 0},
            {8, 0, 0, 0, 6, 0, 0, 0, 3},
            {4, 0, 0, 8, 0, 3, 0, 0, 1},
            {7, 0, 0, 0, 2, 0, 0, 0, 6},
            {0, 6, 0, 0, 0, 0, 2, 8, 0},
            {0, 0, 0, 4, 1, 9, 0, 0, 5},
            {0, 0, 0, 0, 8, 0, 0, 7, 9}
        };

        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                board[y, x] = puzzle[y, x];
                fixedCells[y, x] = puzzle[y, x] != 0;
            }
        }
    }

    private void GamePanel_MouseClick(object? sender, MouseEventArgs e)
    {
        selectedX = e.X / CellSize;
        selectedY = e.Y / CellSize;
        
        if (selectedX >= 9) selectedX = 8;
        if (selectedY >= 9) selectedY = 8;
        
        gamePanel.Invalidate();
    }

    private void SudokuForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (fixedCells[selectedY, selectedX])
            return;

        if (e.KeyCode >= Keys.D1 && e.KeyCode <= Keys.D9)
        {
            board[selectedY, selectedX] = e.KeyCode - Keys.D0;
            gamePanel.Invalidate();
            CheckWin();
        }
        else if (e.KeyCode >= Keys.NumPad1 && e.KeyCode <= Keys.NumPad9)
        {
            board[selectedY, selectedX] = e.KeyCode - Keys.NumPad0;
            gamePanel.Invalidate();
            CheckWin();
        }
        else if (e.KeyCode == Keys.D0 || e.KeyCode == Keys.NumPad0 || e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
        {
            board[selectedY, selectedX] = 0;
            gamePanel.Invalidate();
        }
    }

    private void CheckWin()
    {
        if (!IsBoardComplete()) return;
        if (!IsValidSolution()) return;

        var elapsed = DateTime.Now - startTime;
        HighScoreManager.Instance.AddScore("Sudoku", 1000, elapsed);
        
        var result = MessageBox.Show($"Congratulations! You solved the Sudoku!\nTime: {elapsed:mm\\:ss}\n\nPlay again?", 
            "Victory!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
        
        if (result == DialogResult.Yes)
        {
            startTime = DateTime.Now;
            InitializeGame();
            gamePanel.Invalidate();
        }
        else
        {
            this.Close();
        }
    }

    private bool IsBoardComplete()
    {
        for (int y = 0; y < 9; y++)
            for (int x = 0; x < 9; x++)
                if (board[y, x] == 0)
                    return false;
        return true;
    }

    private bool IsValidSolution()
    {
        // Check rows
        for (int y = 0; y < 9; y++)
        {
            HashSet<int> seen = new HashSet<int>();
            for (int x = 0; x < 9; x++)
                if (!seen.Add(board[y, x]))
                    return false;
        }

        // Check columns
        for (int x = 0; x < 9; x++)
        {
            HashSet<int> seen = new HashSet<int>();
            for (int y = 0; y < 9; y++)
                if (!seen.Add(board[y, x]))
                    return false;
        }

        // Check 3x3 boxes
        for (int boxY = 0; boxY < 3; boxY++)
        {
            for (int boxX = 0; boxX < 3; boxX++)
            {
                HashSet<int> seen = new HashSet<int>();
                for (int y = 0; y < 3; y++)
                    for (int x = 0; x < 3; x++)
                        if (!seen.Add(board[boxY * 3 + y, boxX * 3 + x]))
                            return false;
            }
        }

        return true;
    }

    private void GamePanel_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;

        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                int drawX = x * CellSize;
                int drawY = y * CellSize;

                // Highlight selected cell
                if (x == selectedX && y == selectedY)
                    g.FillRectangle(Brushes.LightYellow, drawX, drawY, CellSize, CellSize);
                else
                    g.FillRectangle(Brushes.White, drawX, drawY, CellSize, CellSize);

                // Draw number
                if (board[y, x] != 0)
                {
                    var brush = fixedCells[y, x] ? Brushes.Black : Brushes.Blue;
                    var font = new Font("Arial", 20, FontStyle.Bold);
                    g.DrawString(board[y, x].ToString(), font, brush, 
                        drawX + 15, drawY + 10);
                }

                // Draw cell border
                var pen = Pens.Gray;
                g.DrawRectangle(pen, drawX, drawY, CellSize, CellSize);
            }
        }

        // Draw thick lines for 3x3 boxes
        var thickPen = new Pen(Color.Black, 3);
        for (int i = 0; i <= 9; i += 3)
        {
            g.DrawLine(thickPen, i * CellSize, 0, i * CellSize, 9 * CellSize);
            g.DrawLine(thickPen, 0, i * CellSize, 9 * CellSize, i * CellSize);
        }
    }
}
