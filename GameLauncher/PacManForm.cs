using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GameLauncher
{
    public partial class PacManForm : Form
    {
        private const int CellSize = 30;
        private const int MapWidth = 20;
        private const int MapHeight = 20;
        
        private Timer gameTimer;
        private int[,] map;
        private Point pacManPos;
        private Point pacManDir;
        private List<Ghost> ghosts;
        private int score;
        private int supremeTimer;
        private bool isPaused;
        private string difficulty = "Medium";
        
        private Image pacManImage;
        private Image ghostNormalImage;
        private Image ghostVulnerableImage;
        private Image foodDotImage;
        private Image supremeFoodImage;

        public PacManForm()
        {
            InitializeComponent();
            LoadAssets();
            InitializeGame();
        }

        private void LoadAssets()
        {
            try
            {
                pacManImage = AssetManager.LoadSvgAsImage("pacman-character.svg", CellSize, CellSize);
                ghostNormalImage = AssetManager.LoadSvgAsImage("pacman-ghost_red_normal.svg", CellSize, CellSize);
                ghostVulnerableImage = AssetManager.LoadSvgAsImage("pacman-ghost_blue_vulnerable.svg", CellSize, CellSize);
                foodDotImage = AssetManager.LoadSvgAsImage("pacman-food_dot.svg", CellSize, CellSize);
                supremeFoodImage = AssetManager.LoadSvgAsImage("pacman-strawberry.svg", CellSize, CellSize);
            }
            catch { }
        }

        private void InitializeComponent()
        {
            this.Text = "Pac-Man";
            this.ClientSize = new Size(MapWidth * CellSize + 200, MapHeight * CellSize);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.KeyPreview = true;
            this.DoubleBuffered = true;

            this.KeyDown += PacManForm_KeyDown;
            this.Paint += PacManForm_Paint;

            Button settingsBtn = new Button
            {
                Text = "Settings",
                Location = new Point(MapWidth * CellSize + 10, 20),
                Size = new Size(180, 40)
            };
            settingsBtn.Click += (s, e) => ShowSettings();
            this.Controls.Add(settingsBtn);

            Button scoreboardBtn = new Button
            {
                Text = "Scoreboard",
                Location = new Point(MapWidth * CellSize + 10, 70),
                Size = new Size(180, 40)
            };
            scoreboardBtn.Click += (s, e) => ShowScoreboard();
            this.Controls.Add(scoreboardBtn);

            Button newGameBtn = new Button
            {
                Text = "New Game",
                Location = new Point(MapWidth * CellSize + 10, 120),
                Size = new Size(180, 40)
            };
            newGameBtn.Click += (s, e) => InitializeGame();
            this.Controls.Add(newGameBtn);

            gameTimer = new Timer { Interval = 200 };
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void InitializeGame()
        {
            map = new int[MapHeight, MapWidth];
            Random rand = new Random();
            
            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    if (x == 0 || x == MapWidth - 1 || y == 0 || y == MapHeight - 1)
                        map[y, x] = 9; // wall
                    else if (rand.Next(10) < 2)
                        map[y, x] = 9; // random walls
                    else if (rand.Next(20) == 0)
                        map[y, x] = 2; // supreme food (5pt)
                    else
                        map[y, x] = 1; // regular food (1pt)
                }
            }

            pacManPos = new Point(1, 1);
            map[pacManPos.Y, pacManPos.X] = 0;
            pacManDir = new Point(1, 0);
            
            ghosts = new List<Ghost>
            {
                new Ghost { Position = new Point(MapWidth - 2, 1), IsVulnerable = false },
                new Ghost { Position = new Point(MapWidth - 2, MapHeight - 2), IsVulnerable = false },
                new Ghost { Position = new Point(1, MapHeight - 2), IsVulnerable = false }
            };

            score = 0;
            supremeTimer = 0;
            isPaused = false;
            this.Invalidate();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (isPaused) return;

            // Move Pac-Man
            Point newPos = new Point(pacManPos.X + pacManDir.X, pacManPos.Y + pacManDir.Y);
            if (newPos.X >= 0 && newPos.X < MapWidth && newPos.Y >= 0 && newPos.Y < MapHeight)
            {
                if (map[newPos.Y, newPos.X] != 9)
                {
                    if (map[newPos.Y, newPos.X] == 1)
                    {
                        score += 1;
                    }
                    else if (map[newPos.Y, newPos.X] == 2)
                    {
                        score += 5;
                        supremeTimer = 15;
                        foreach (var ghost in ghosts)
                            ghost.IsVulnerable = true;
                    }
                    map[newPos.Y, newPos.X] = 0;
                    pacManPos = newPos;
                }
            }

            // Update supreme timer
            if (supremeTimer > 0)
            {
                supremeTimer--;
                if (supremeTimer == 0)
                {
                    foreach (var ghost in ghosts)
                        ghost.IsVulnerable = false;
                }
            }

            // Move ghosts
            int speed = difficulty == "Easy" ? 3 : (difficulty == "Hard" ? 1 : 2);
            if (gameTimer.Interval % speed == 0)
            {
                foreach (var ghost in ghosts)
                {
                    MoveGhost(ghost);
                }
            }

            // Check collision
            foreach (var ghost in ghosts)
            {
                if (ghost.Position == pacManPos)
                {
                    if (ghost.IsVulnerable)
                    {
                        score += 10;
                        ghost.Position = new Point(MapWidth - 2, 1);
                        ghost.IsVulnerable = false;
                    }
                    else
                    {
                        GameOver();
                        return;
                    }
                }
            }

            // Check win condition
            bool hasFood = false;
            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    if (map[y, x] == 1 || map[y, x] == 2)
                    {
                        hasFood = true;
                        break;
                    }
                }
                if (hasFood) break;
            }

            if (!hasFood)
            {
                GameWon();
            }

            this.Invalidate();
        }

        private void MoveGhost(Ghost ghost)
        {
            Point target = ghost.IsVulnerable ? 
                new Point(MapWidth - 1 - pacManPos.X, MapHeight - 1 - pacManPos.Y) : 
                pacManPos;

            int dx = target.X - ghost.Position.X;
            int dy = target.Y - ghost.Position.Y;

            Point[] directions = new Point[]
            {
                new Point(Math.Sign(dx), 0),
                new Point(0, Math.Sign(dy)),
                new Point(-Math.Sign(dx), 0),
                new Point(0, -Math.Sign(dy))
            };

            foreach (var dir in directions)
            {
                Point newPos = new Point(ghost.Position.X + dir.X, ghost.Position.Y + dir.Y);
                if (newPos.X >= 0 && newPos.X < MapWidth && newPos.Y >= 0 && newPos.Y < MapHeight)
                {
                    if (map[newPos.Y, newPos.X] != 9)
                    {
                        ghost.Position = newPos;
                        break;
                    }
                }
            }
        }

        private void GameOver()
        {
            gameTimer.Stop();
            HighScoreManager.SaveScore("PacMan", score);
            var result = MessageBox.Show($"Game Over! Score: {score}\n\nWanna Try Again?", "Game Over", 
                MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
                InitializeGame();
            else
                this.Close();
        }

        private void GameWon()
        {
            gameTimer.Stop();
            HighScoreManager.SaveScore("PacMan", score);
            var result = MessageBox.Show($"You Won! Score: {score}\n\nWanna Try Again?", "Victory", 
                MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
                InitializeGame();
            else
                this.Close();
        }

        private void PacManForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.P)
            {
                isPaused = !isPaused;
                this.Invalidate();
            }
            else if (e.KeyCode == Keys.Up)
                pacManDir = new Point(0, -1);
            else if (e.KeyCode == Keys.Down)
                pacManDir = new Point(0, 1);
            else if (e.KeyCode == Keys.Left)
                pacManDir = new Point(-1, 0);
            else if (e.KeyCode == Keys.Right)
                pacManDir = new Point(1, 0);
        }

        private void PacManForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Draw map
            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    Rectangle rect = new Rectangle(x * CellSize, y * CellSize, CellSize, CellSize);
                    
                    if (map[y, x] == 9)
                    {
                        g.FillRectangle(Brushes.DarkBlue, rect);
                    }
                    else if (map[y, x] == 1)
                    {
                        if (foodDotImage != null)
                            g.DrawImage(foodDotImage, rect);
                        else
                            g.FillEllipse(Brushes.Yellow, x * CellSize + 12, y * CellSize + 12, 6, 6);
                    }
                    else if (map[y, x] == 2)
                    {
                        if (supremeFoodImage != null)
                            g.DrawImage(supremeFoodImage, rect);
                        else
                            g.FillEllipse(Brushes.Red, x * CellSize + 8, y * CellSize + 8, 14, 14);
                    }
                }
            }

            // Draw Pac-Man
            if (pacManImage != null)
                g.DrawImage(pacManImage, pacManPos.X * CellSize, pacManPos.Y * CellSize, CellSize, CellSize);
            else
                g.FillEllipse(Brushes.Yellow, pacManPos.X * CellSize, pacManPos.Y * CellSize, CellSize, CellSize);

            // Draw ghosts
            foreach (var ghost in ghosts)
            {
                Image img = ghost.IsVulnerable ? ghostVulnerableImage : ghostNormalImage;
                if (img != null)
                    g.DrawImage(img, ghost.Position.X * CellSize, ghost.Position.Y * CellSize, CellSize, CellSize);
                else
                {
                    Brush brush = ghost.IsVulnerable ? Brushes.Blue : Brushes.Red;
                    g.FillEllipse(brush, ghost.Position.X * CellSize, ghost.Position.Y * CellSize, CellSize, CellSize);
                }
            }

            // Draw score
            g.DrawString($"Score: {score}", new Font("Arial", 14), Brushes.Black, 
                MapWidth * CellSize + 10, MapHeight * CellSize - 60);

            // Draw pause overlay
            if (isPaused)
            {
                using (SolidBrush pauseBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0)))
                {
                    g.FillRectangle(pauseBrush, 0, 0, MapWidth * CellSize, MapHeight * CellSize);
                }
                g.DrawString("PAUSED", new Font("Arial", 48, FontStyle.Bold), Brushes.White, 
                    MapWidth * CellSize / 2 - 120, MapHeight * CellSize / 2 - 30);
            }
        }

        private void ShowSettings()
        {
            Form settingsForm = new Form
            {
                Text = "Pac-Man Settings",
                Size = new Size(300, 200),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog
            };

            Label label = new Label
            {
                Text = "Difficulty:",
                Location = new Point(20, 20),
                AutoSize = true
            };
            settingsForm.Controls.Add(label);

            ComboBox difficultyBox = new ComboBox
            {
                Location = new Point(20, 50),
                Size = new Size(240, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            difficultyBox.Items.AddRange(new[] { "Easy", "Medium", "Hard" });
            difficultyBox.SelectedItem = difficulty;
            settingsForm.Controls.Add(difficultyBox);

            Button okBtn = new Button
            {
                Text = "OK",
                Location = new Point(100, 110),
                Size = new Size(80, 30)
            };
            okBtn.Click += (s, e) =>
            {
                difficulty = difficultyBox.SelectedItem?.ToString() ?? "Medium";
                settingsForm.Close();
            };
            settingsForm.Controls.Add(okBtn);

            settingsForm.ShowDialog(this);
        }

        private void ShowScoreboard()
        {
            var scores = HighScoreManager.LoadScores("PacMan");
            string message = "Top Scores:\n\n";
            for (int i = 0; i < Math.Min(10, scores.Count); i++)
            {
                message += $"{i + 1}. {scores[i].Score} - {scores[i].Date:yyyy-MM-dd}\n";
            }
            if (scores.Count == 0)
                message = "No scores yet!";
            
            MessageBox.Show(message, "Scoreboard");
        }

        private class Ghost
        {
            public Point Position { get; set; }
            public bool IsVulnerable { get; set; }
        }
    }
}
