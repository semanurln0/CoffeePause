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

    // Powerup (strawberry)
    private int powerX = -1;
    private int powerY = -1;
    private int powerTicksLeft = 0;
    private const int PowerDurationTicks = 30;

    private Image? _powerImage;

    private bool isPaused = false;

    private class Ghost
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Color Color { get; set; }
        public bool IsVulnerable { get; set; } = false;
        public int SpawnX { get; set; }
        public int SpawnY { get; set; }
    }

    public PacManForm()
    {
        InitializeComponent();
        InitializeGame();
        _powerImage = AssetManager.Instance.LoadImage("strawberry.svg") ?? AssetManager.Instance.LoadImage("strawberry.png");
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
        isPaused = false;
        score = 0;
        startTime = DateTime.Now;
        ghostMoveCounter = 0;
        ghosts.Clear();

        InitializeMaze();
        InitializeGhosts();
        PlacePowerUp();
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
        ghosts.Add(new Ghost { X = 3, Y = 3, Color = Color.Red, SpawnX = 3, SpawnY = 3 });
        ghosts.Add(new Ghost { X = 16, Y = 3, Color = Color.Cyan, SpawnX = 16, SpawnY = 3 });
        ghosts.Add(new Ghost { X = 3, Y = 6, Color = Color.Blue, SpawnX = 3, SpawnY = 6 });
    }

    private void PlacePowerUp()
    {
        var rand = new Random();
        var freeCells = new List<(int x, int y)>();
        for (int y = 1; y < 9; y++)
            for (int x = 1; x < 19; x++)
                if (maze[y, x] == '.') freeCells.Add((x, y));

        if (freeCells.Count > 0)
        {
            var pick = freeCells[rand.Next(freeCells.Count)];
            powerX = pick.x; powerY = pick.y; maze[powerY, powerX] = 'o';
        }
    }

    private void PacManForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.P)
        {
            TogglePause();
            return;
        }
        if (e.KeyCode == Keys.Escape)
        {
            this.Close();
            return;
        }

        if (!gameRunning || isPaused) return;

        switch (e.KeyCode)
        {
            case Keys.W: MovePacMan(0, -1); break;
            case Keys.S: MovePacMan(0, 1); break;
            case Keys.A: MovePacMan(-1, 0); break;
            case Keys.D: MovePacMan(1, 0); break;
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused) gameTimer.Stop(); else gameTimer.Start();
        this.Invalidate();
    }

    private void MovePacMan(int dx, int dy)
    {
        int newX = pacManX + dx; int newY = pacManY + dy;
        if (newX >= 0 && newX < 20 && newY >= 0 && newY < 10 && maze[newY, newX] != '#')
        {
            if (maze[newY, newX] == '.') { score += 1; maze[newY, newX] = ' '; }
            else if (maze[newY, newX] == 'o') { score += 5; maze[newY, newX] = ' '; ActivatePowerUp(); }
            pacManX = newX; pacManY = newY; CheckGhostCollision(); this.Invalidate();
        }
    }

    private void ActivatePowerUp()
    {
        powerTicksLeft = PowerDurationTicks;
        foreach (var g in ghosts) g.IsVulnerable = true;
    }

    private void MoveGhosts()
    {
        Random rand = new Random();
        foreach (var ghost in ghosts)
        {
            int dx = 0, dy = 0;
            if (ghost.IsVulnerable)
            {
                if (Math.Abs(pacManX - ghost.X) > Math.Abs(pacManY - ghost.Y)) dx = pacManX > ghost.X ? -1 : 1; else dy = pacManY > ghost.Y ? -1 : 1;
                if (rand.Next(10) < 3) { int dir = rand.Next(4); switch (dir) { case 0: dx = 1; break; case 1: dx = -1; break; case 2: dy = 1; break; case 3: dy = -1; break; } }
            }
            else
            {
                if (rand.Next(10) < 6) { if (Math.Abs(pacManX - ghost.X) > Math.Abs(pacManY - ghost.Y)) dx = pacManX > ghost.X ? 1 : -1; else dy = pacManY > ghost.Y ? 1 : -1; }
                else { int dir = rand.Next(4); switch (dir) { case 0: dx = 1; break; case 1: dx = -1; break; case 2: dy = 1; break; case 3: dy = -1; break; } }
            }
            int newX = ghost.X + dx; int newY = ghost.Y + dy;
            if (newX > 0 && newX < 19 && newY > 0 && newY < 9 && maze[newY, newX] != '#') { ghost.X = newX; ghost.Y = newY; }
        }
    }

    private void CheckGhostCollision()
    {
        foreach (var ghost in ghosts.ToList())
        {
            if (ghost.X == pacManX && ghost.Y == pacManY)
            {
                if (ghost.IsVulnerable) { score += 10; ghost.X = ghost.SpawnX; ghost.Y = ghost.SpawnY; ghost.IsVulnerable = false; }
                else { GameOver(); return; }
            }
        }
    }

    private void GameOver()
    {
        gameRunning = false; gameTimer.Stop(); ShowGameOverPanel($"Game Over!\nScore: {{score}}\nTime: {{DateTime.Now - startTime:mm\:ss}}");
    }

    private void ShowGameOverPanel(string text)
    {
        var panel = new Panel { Size = new Size(this.ClientSize.Width - 40, 160), Location = new Point(20, (this.ClientSize.Height - 160) / 2), BackColor = Color.FromArgb(220, 30, 30, 30) };
        var label = new Label { Text = text, ForeColor = Color.White, Font = new Font("Arial", 14, FontStyle.Bold), AutoSize = false, Size = new Size(panel.Width - 20, 60), Location = new Point(10, 10) }; panel.Controls.Add(label);
        var retryBtn = new Button { Text = "Retry", Size = new Size(120, 40), Location = new Point((panel.Width / 2) - 130, 80) };
        retryBtn.Click += (s, e) => { this.Controls.Remove(panel); InitializeGame(); gameTimer.Start(); };
        panel.Controls.Add(retryBtn);
        var exitBtn = new Button { Text = "Exit", Size = new Size(120, 40), Location = new Point((panel.Width / 2) + 10, 80) };
        exitBtn.Click += (s, e) => this.Close(); panel.Controls.Add(exitBtn);
        this.Controls.Add(panel); panel.BringToFront();
    }

    private void GameTimer_Tick(object? sender, EventArgs e)
    {
        if (!gameRunning || isPaused) return;
        ghostMoveCounter++;
        if (ghostMoveCounter >= 2) { MoveGhosts(); CheckGhostCollision(); ghostMoveCounter = 0; }
        if (powerTicksLeft > 0) { powerTicksLeft--; if (powerTicksLeft == 0) foreach (var g in ghosts) g.IsVulnerable = false; }
        if (CheckWin()) { gameRunning = false; gameTimer.Stop(); var elapsed = DateTime.Now - startTime; HighScoreManager.Instance.AddScore("Pac-Man", score, elapsed); ShowGameOverPanel($"Congratulations! You collected all the dots!\nScore: {{score}}\nTime: {{elapsed:mm\:ss}}"); }
        this.Invalidate();
    }

    private bool CheckWin()
    {
        for (int y = 0; y < 10; y++) for (int x = 0; x < 20; x++) if (maze[y, x] == '.') return false;
        return true;
    }

    private void PacManForm_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics; g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        // Draw score (black text)
        g.DrawString($"Score: {{score}}", new Font("Arial", 14, FontStyle.Bold), Brushes.Black, new PointF(10, 10));
        g.DrawString("Controls: W/A/S/D to move, P to pause, ESC to exit", new Font("Arial", 10), Brushes.Black, new PointF(10, 35));
        // Draw maze
        for (int y = 0; y < 10; y++) for (int x = 0; x < 20; x++)
        {
            int drawX = x * CellSize; int drawY = 60 + y * CellSize;
            switch (maze[y, x])
            {
                case '#': g.FillRectangle(Brushes.Blue, drawX, drawY, CellSize, CellSize); break;
                case '.': g.FillEllipse(new SolidBrush(ColorTranslator.FromHtml("#0083b4"), drawX + CellSize/2 - 3, drawY + CellSize/2 - 3, 6, 6); break;
                case 'o': if (_powerImage != null) g.DrawImage(_powerImage, new Rectangle(drawX + 4, drawY + 4, CellSize - 8, CellSize - 8)); else g.FillEllipse(Brushes.Pink, drawX + 6, drawY + 6, CellSize - 12, CellSize - 12); break;
            }
        }
        // draw ghosts
        foreach (var ghost in ghosts) { int drawX = ghost.X * CellSize; int drawY = 60 + ghost.Y * CellSize; var drawColor = ghost.IsVulnerable ? Color.LightBlue : ghost.Color; using (var brush = new SolidBrush(drawColor)) { g.FillEllipse(brush, drawX + 2, drawY, CellSize - 4, CellSize - 4); g.FillRectangle(brush, drawX + 2, drawY + CellSize/2, CellSize - 4, CellSize/2); g.FillEllipse(Brushes.White, drawX + 6, drawY + 8, 7, 7); g.FillEllipse(Brushes.White, drawX + 17, drawY + 8, 7, 7); g.FillEllipse(Brushes.Black, drawX + 8, drawY + 10, 3, 3); g.FillEllipse(Brushes.Black, drawX + 19, drawY + 10, 3, 3); } }
        // draw pacman
        int pacDrawX = pacManX * CellSize; int pacDrawY = 60 + pacManY * CellSize; g.FillEllipse(Brushes.Yellow, pacDrawX + 2, pacDrawY + 2, CellSize - 4, CellSize - 4); g.FillPie(Brushes.Black, pacDrawX + 2, pacDrawY + 2, CellSize - 4, CellSize - 4, 30, 60);
        if (isPaused) { var overlay = new SolidBrush(Color.FromArgb(160, 0, 0, 0)); g.FillRectangle(overlay, 0, 0, this.ClientSize.Width, this.ClientSize.Height); g.DrawString("PAUSED", new Font("Arial", 36, FontStyle.Bold), Brushes.White, new PointF(this.ClientSize.Width/2 - 80, this.ClientSize.Height/2 - 30)); }
    }
}