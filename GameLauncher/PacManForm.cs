using System.Drawing;
using System.Windows.Forms;

namespace CoffeePause;

public class PacManForm : Form
{
    private int pacManX = 10;
    private int pacManY = 5;
    private int score = 0;
    private char[,] maze = new char[10, 20];
    private bool gameRunning = true;
    private System.Windows.Forms.Timer gameTimer = null!;
    private DateTime startTime;
    private const int CellSize = 30;
    private List<Ghost> ghosts = new List<Ghost>();
    private int ghostMoveCounter = 0;

    private class Ghost
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Color Color { get; set; }
    }

    public PacManForm()
    {
        InitializeComponent();
        InitializeGame();
    }

    private void InitializeComponent()
    {
        this.Text = "Pac-Man";
        this.Size = new Size(620, 380);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.KeyPreview = true;
        this.KeyDown += PacManForm_KeyDown;
        this.Paint += PacManForm_Paint;
        this.DoubleBuffered = true;

        gameTimer = new System.Windows.Forms.Timer();
        gameTimer.Interval = 100;
        gameTimer.Tick += GameTimer_Tick;
        gameTimer.Start();
    }

    private void InitializeGame()
    {
        gameRunning = true;
        score = 0;
        startTime = DateTime.Now;
        ghostMoveCounter = 0;
        ghosts.Clear();
        
        InitializeMaze();
        InitializeGhosts();
    }

    private void InitializeMaze()
    {
        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 20; x++)
            {
                if (y == 0 || y == 9 || x == 0 || x == 19)
                    maze[y, x] = '#';
                else if ((x == 5 || x == 15) && y > 2 && y < 7)
                    maze[y, x] = '#';
                else
                    maze[y, x] = '.';
            }
        }
        pacManX = 10;
        pacManY = 5;
        maze[pacManY, pacManX] = ' ';
    }

    private void InitializeGhosts()
    {
        ghosts.Add(new Ghost { X = 3, Y = 3, Color = Color.Red });
        ghosts.Add(new Ghost { X = 16, Y = 3, Color = Color.Cyan });
        ghosts.Add(new Ghost { X = 3, Y = 6, Color = Color.Pink });
    }

    private void PacManForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (!gameRunning) return;

        switch (e.KeyCode)
        {
            case Keys.W:
                MovePacMan(0, -1);
                break;
            case Keys.S:
                MovePacMan(0, 1);
                break;
            case Keys.A:
                MovePacMan(-1, 0);
                break;
            case Keys.D:
                MovePacMan(1, 0);
                break;
            case Keys.Escape:
                this.Close();
                break;
        }
    }

    private void MovePacMan(int dx, int dy)
    {
        int newX = pacManX + dx;
        int newY = pacManY + dy;

        if (newX >= 0 && newX < 20 && newY >= 0 && newY < 10 && maze[newY, newX] != '#')
        {
            if (maze[newY, newX] == '.')
            {
                score += 10;
                maze[newY, newX] = ' ';
            }
            pacManX = newX;
            pacManY = newY;
            CheckGhostCollision();
            this.Invalidate();
        }
    }

    private void MoveGhosts()
    {
        Random rand = new Random();
        foreach (var ghost in ghosts)
        {
            // Simple AI: move randomly, but prefer moving toward Pac-Man
            int dx = 0, dy = 0;
            
            if (rand.Next(10) < 6) // 60% chance to move toward Pac-Man
            {
                if (Math.Abs(pacManX - ghost.X) > Math.Abs(pacManY - ghost.Y))
                    dx = pacManX > ghost.X ? 1 : -1;
                else
                    dy = pacManY > ghost.Y ? 1 : -1;
            }
            else // 40% chance to move randomly
            {
                int dir = rand.Next(4);
                switch (dir)
                {
                    case 0: dx = 1; break;
                    case 1: dx = -1; break;
                    case 2: dy = 1; break;
                    case 3: dy = -1; break;
                }
            }

            int newX = ghost.X + dx;
            int newY = ghost.Y + dy;

            if (newX > 0 && newX < 19 && newY > 0 && newY < 9 && maze[newY, newX] != '#')
            {
                ghost.X = newX;
                ghost.Y = newY;
            }
        }
    }

    private void CheckGhostCollision()
    {
        foreach (var ghost in ghosts)
        {
            if (ghost.X == pacManX && ghost.Y == pacManY)
            {
                GameOver();
                return;
            }
        }
    }

    private void GameOver()
    {
        gameRunning = false;
        gameTimer.Stop();
        
        var result = MessageBox.Show($"Game Over!\nScore: {score}\nTime: {DateTime.Now - startTime:mm\\:ss}\n\nTry again?", 
            "Game Over", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
        
        if (result == DialogResult.Yes)
        {
            InitializeGame();
            gameTimer.Start();
            this.Invalidate();
        }
        else
        {
            this.Close();
        }
    }

    private void GameTimer_Tick(object? sender, EventArgs e)
    {
        if (!gameRunning) return;

        ghostMoveCounter++;
        if (ghostMoveCounter >= 2) // Move ghosts every 2 ticks (slower than Pac-Man)
        {
            MoveGhosts();
            CheckGhostCollision();
            ghostMoveCounter = 0;
        }

        if (CheckWin())
        {
            gameRunning = false;
            gameTimer.Stop();
            
            var elapsed = DateTime.Now - startTime;
            HighScoreManager.Instance.AddScore("Pac-Man", score, elapsed);
            
            var result = MessageBox.Show($"Congratulations! You collected all the dots!\nScore: {score}\nTime: {elapsed:mm\\:ss}\n\nPlay again?", 
                "You Win!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            
            if (result == DialogResult.Yes)
            {
                InitializeGame();
                gameTimer.Start();
                this.Invalidate();
            }
            else
            {
                this.Close();
            }
        }

        this.Invalidate();
    }

    private bool CheckWin()
    {
        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 20; x++)
            {
                if (maze[y, x] == '.')
                    return false;
            }
        }
        return true;
    }

    private void PacManForm_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        
        // Draw score
        g.DrawString($"Score: {score}", new Font("Arial", 14, FontStyle.Bold), 
            Brushes.Yellow, new PointF(10, 10));
        
        g.DrawString("Controls: W/A/S/D to move, ESC to quit", new Font("Arial", 10), 
            Brushes.White, new PointF(10, 35));

        // Draw maze
        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 20; x++)
            {
                int drawX = x * CellSize;
                int drawY = 60 + y * CellSize;

                switch (maze[y, x])
                {
                    case '#':
                        g.FillRectangle(Brushes.Blue, drawX, drawY, CellSize, CellSize);
                        break;
                    case '.':
                        g.FillEllipse(Brushes.White, drawX + CellSize/2 - 3, drawY + CellSize/2 - 3, 6, 6);
                        break;
                }
            }
        }

        // Draw ghosts
        foreach (var ghost in ghosts)
        {
            int drawX = ghost.X * CellSize;
            int drawY = 60 + ghost.Y * CellSize;
            
            using (var brush = new SolidBrush(ghost.Color))
            {
                // Ghost body
                g.FillEllipse(brush, drawX + 2, drawY, CellSize - 4, CellSize - 4);
                g.FillRectangle(brush, drawX + 2, drawY + CellSize/2, CellSize - 4, CellSize/2);
                
                // Ghost eyes
                g.FillEllipse(Brushes.White, drawX + 6, drawY + 8, 7, 7);
                g.FillEllipse(Brushes.White, drawX + 17, drawY + 8, 7, 7);
                g.FillEllipse(Brushes.Black, drawX + 8, drawY + 10, 3, 3);
                g.FillEllipse(Brushes.Black, drawX + 19, drawY + 10, 3, 3);
            }
        }

        // Draw Pac-Man
        int pacDrawX = pacManX * CellSize;
        int pacDrawY = 60 + pacManY * CellSize;
        g.FillEllipse(Brushes.Yellow, pacDrawX + 2, pacDrawY + 2, CellSize - 4, CellSize - 4);
        // Draw mouth
        g.FillPie(Brushes.Black, pacDrawX + 2, pacDrawY + 2, CellSize - 4, CellSize - 4, 30, 60);
    }
}
