using System.Drawing;
using System.Windows.Forms;

namespace CoffeePause;

public class MinesweeperForm : Form
{
    private new int Width => GameSettings.Instance.Minesweeper.GridWidth;
    private new int Height => GameSettings.Instance.Minesweeper.GridHeight;
    private int MineCount => GameSettings.Instance.Minesweeper.MineCount;
    
    private bool[,] mines = null!;
    private bool[,] revealed = null!;
    private bool[,] flagged = null!;
    private int[,] adjacentMines = null!;
    private bool gameOver = false;
    private bool won = false;
    private DateTime startTime;
    private const int CellSize = 30;
    private Panel gamePanel = null!;

    public MinesweeperForm()
    {
        InitializeComponent();
        InitializeGame();
        startTime = DateTime.Now;
    }

    private void InitializeComponent()
    {
        this.Text = "Minesweeper";
        this.Size = new Size(Math.Min(Width * CellSize + 50, 900), Math.Min(Height * CellSize + 120, 700));
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;

        var infoLabel = new Label
        {
            Text = $"Left-click to reveal, Right-click to flag. Mines: {MineCount}",
            Location = new Point(10, 10),
            Size = new Size(400, 25),
            Font = new Font("Arial", 10)
        };
        this.Controls.Add(infoLabel);

        gamePanel = new Panel
        {
            Location = new Point(10, 40),
            Size = new Size(Width * CellSize, Height * CellSize),
            BorderStyle = BorderStyle.FixedSingle
        };
        gamePanel.Paint += GamePanel_Paint;
        gamePanel.MouseClick += GamePanel_MouseClick;
        this.Controls.Add(gamePanel);
    }

    private void InitializeGame()
    {
        mines = new bool[Height, Width];
        revealed = new bool[Height, Width];
        flagged = new bool[Height, Width];
        adjacentMines = new int[Height, Width];

        // Place mines randomly
        Random rand = new Random();
        int placedMines = 0;
        while (placedMines < MineCount)
        {
            int x = rand.Next(Width);
            int y = rand.Next(Height);
            if (!mines[y, x])
            {
                mines[y, x] = true;
                placedMines++;
            }
        }

        // Calculate adjacent mines
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (!mines[y, x])
                {
                    adjacentMines[y, x] = CountAdjacentMines(x, y);
                }
            }
        }
    }

    private int CountAdjacentMines(int x, int y)
    {
        int count = 0;
        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = x + dx;
                int ny = y + dy;
                if (nx >= 0 && nx < Width && ny >= 0 && ny < Height && mines[ny, nx])
                    count++;
            }
        }
        return count;
    }

    private void GamePanel_MouseClick(object? sender, MouseEventArgs e)
    {
        if (gameOver || won) return;

        int x = e.X / CellSize;
        int y = e.Y / CellSize;

        if (x >= Width || y >= Height) return;

        if (e.Button == MouseButtons.Left)
        {
            if (!flagged[y, x])
            {
                RevealCell(x, y);
                gamePanel.Invalidate();
                CheckWinCondition();
            }
        }
        else if (e.Button == MouseButtons.Right)
        {
            if (!revealed[y, x])
            {
                flagged[y, x] = !flagged[y, x];
                gamePanel.Invalidate();
            }
        }

        if (gameOver)
        {
            var elapsed = DateTime.Now - startTime;
            var result = MessageBox.Show($"BOOM! You hit a mine!\nTime: {elapsed:mm\\:ss}\n\nTry again?", "Game Over", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            
            if (result == DialogResult.Yes)
            {
                gameOver = false;
                won = false;
                startTime = DateTime.Now;
                InitializeGame();
                gamePanel.Invalidate();
            }
            else
            {
                this.Close();
            }
        }
        else if (won)
        {
            var elapsed = DateTime.Now - startTime;
            HighScoreManager.Instance.AddScore("Minesweeper", 1000, elapsed);
            var result = MessageBox.Show($"Congratulations! You won!\nTime: {elapsed:mm\\:ss}\n\nPlay again?", 
                "Victory!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            
            if (result == DialogResult.Yes)
            {
                gameOver = false;
                won = false;
                startTime = DateTime.Now;
                InitializeGame();
                gamePanel.Invalidate();
            }
            else
            {
                this.Close();
            }
        }
    }

    private void RevealCell(int x, int y)
    {
        if (revealed[y, x] || flagged[y, x])
            return;

        revealed[y, x] = true;

        if (mines[y, x])
        {
            gameOver = true;
            RevealAllMines();
            return;
        }

        if (adjacentMines[y, x] == 0)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int nx = x + dx;
                    int ny = y + dy;
                    if (nx >= 0 && nx < Width && ny >= 0 && ny < Height)
                        RevealCell(nx, ny);
                }
            }
        }
    }

    private void RevealAllMines()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (mines[y, x])
                    revealed[y, x] = true;
            }
        }
    }

    private void CheckWinCondition()
    {
        int revealedCount = 0;
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (revealed[y, x] && !mines[y, x])
                    revealedCount++;
            }
        }

        if (revealedCount == (Width * Height - MineCount))
            won = true;
    }

    private void GamePanel_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int drawX = x * CellSize;
                int drawY = y * CellSize;

                if (revealed[y, x])
                {
                    g.FillRectangle(Brushes.LightGray, drawX, drawY, CellSize, CellSize);
                    
                    if (mines[y, x])
                    {
                        g.FillEllipse(Brushes.Red, drawX + 5, drawY + 5, CellSize - 10, CellSize - 10);
                    }
                    else if (adjacentMines[y, x] > 0)
                    {
                        var color = GetNumberColor(adjacentMines[y, x]);
                        g.DrawString(adjacentMines[y, x].ToString(), 
                            new Font("Arial", 14, FontStyle.Bold), 
                            new SolidBrush(color), 
                            drawX + 8, drawY + 5);
                    }
                }
                else
                {
                    g.FillRectangle(Brushes.Gray, drawX, drawY, CellSize, CellSize);
                    
                    if (flagged[y, x])
                    {
                        g.FillPolygon(Brushes.Red, new Point[] {
                            new Point(drawX + 8, drawY + 8),
                            new Point(drawX + 22, drawY + 12),
                            new Point(drawX + 8, drawY + 16)
                        });
                        g.DrawLine(new Pen(Color.Black, 2), drawX + 8, drawY + 8, drawX + 8, drawY + 24);
                    }
                }

                g.DrawRectangle(Pens.Black, drawX, drawY, CellSize, CellSize);
            }
        }
    }

    private Color GetNumberColor(int number)
    {
        return number switch
        {
            1 => Color.Blue,
            2 => Color.Green,
            3 => Color.Red,
            4 => Color.DarkBlue,
            5 => Color.DarkRed,
            6 => Color.Cyan,
            7 => Color.Black,
            8 => Color.DarkGray,
            _ => Color.Black
        };
    }
}
