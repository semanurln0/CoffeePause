using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameLauncher
{
    public partial class MinesweeperForm : Form
    {
        private int gridWidth = 10;
        private int gridHeight = 10;
        private int mineCount = 15;
        private const int CellSize = 30;
        
        private Button[,] cells;
        private bool[,] mines;
        private bool[,] revealed;
        private bool[,] flagged;
        private int[,] adjacentMines;
        private bool isPaused;
        private int score;
        private string difficulty = "Medium";

        public MinesweeperForm()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeComponent()
        {
            this.Text = "Minesweeper";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.KeyPreview = true;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            this.KeyDown += MinesweeperForm_KeyDown;
        }

        private void InitializeGame()
        {
            this.Controls.Clear();
            
            SetDifficultySettings();

            this.ClientSize = new Size(gridWidth * CellSize + 220, gridHeight * CellSize + 20);

            Panel gridPanel = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(gridWidth * CellSize, gridHeight * CellSize),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(gridPanel);

            mines = new bool[gridHeight, gridWidth];
            revealed = new bool[gridHeight, gridWidth];
            flagged = new bool[gridHeight, gridWidth];
            adjacentMines = new int[gridHeight, gridWidth];
            cells = new Button[gridHeight, gridWidth];

            // Place mines
            Random rand = new Random();
            int placedMines = 0;
            while (placedMines < mineCount)
            {
                int x = rand.Next(gridWidth);
                int y = rand.Next(gridHeight);
                if (!mines[y, x])
                {
                    mines[y, x] = true;
                    placedMines++;
                }
            }

            // Calculate adjacent mines
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    if (!mines[y, x])
                    {
                        int count = 0;
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            for (int dx = -1; dx <= 1; dx++)
                            {
                                int ny = y + dy;
                                int nx = x + dx;
                                if (ny >= 0 && ny < gridHeight && nx >= 0 && nx < gridWidth && mines[ny, nx])
                                    count++;
                            }
                        }
                        adjacentMines[y, x] = count;
                    }
                }
            }

            // Create cells
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    Button cell = new Button
                    {
                        Location = new Point(x * CellSize, y * CellSize),
                        Size = new Size(CellSize, CellSize),
                        Tag = new Point(x, y),
                        Font = new Font("Arial", 10, FontStyle.Bold)
                    };
                    
                    int cx = x, cy = y;
                    cell.MouseDown += (s, e) => Cell_MouseDown(s, e, cx, cy);
                    
                    cells[y, x] = cell;
                    gridPanel.Controls.Add(cell);
                }
            }

            Button settingsBtn = new Button
            {
                Text = "Settings",
                Location = new Point(gridWidth * CellSize + 20, 20),
                Size = new Size(180, 40)
            };
            settingsBtn.Click += (s, e) => ShowSettings();
            this.Controls.Add(settingsBtn);

            Button scoreboardBtn = new Button
            {
                Text = "Scoreboard",
                Location = new Point(gridWidth * CellSize + 20, 70),
                Size = new Size(180, 40)
            };
            scoreboardBtn.Click += (s, e) => ShowScoreboard();
            this.Controls.Add(scoreboardBtn);

            Button newGameBtn = new Button
            {
                Text = "New Game",
                Location = new Point(gridWidth * CellSize + 20, 120),
                Size = new Size(180, 40)
            };
            newGameBtn.Click += (s, e) => InitializeGame();
            this.Controls.Add(newGameBtn);

            score = 0;
            isPaused = false;
        }

        private void SetDifficultySettings()
        {
            switch (difficulty)
            {
                case "Easy":
                    gridWidth = 10;
                    gridHeight = 10;
                    mineCount = 10;
                    break;
                case "Hard":
                    gridWidth = 20;
                    gridHeight = 20;
                    mineCount = 60;
                    break;
                default: // Medium
                    gridWidth = 15;
                    gridHeight = 15;
                    mineCount = 30;
                    break;
            }
        }

        private void Cell_MouseDown(object sender, MouseEventArgs e, int x, int y)
        {
            if (isPaused) return;

            if (e.Button == MouseButtons.Right)
            {
                // Flag
                if (!revealed[y, x])
                {
                    flagged[y, x] = !flagged[y, x];
                    cells[y, x].Text = flagged[y, x] ? "F" : "";
                    cells[y, x].BackColor = flagged[y, x] ? Color.Yellow : SystemColors.Control;
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                // Reveal
                if (!flagged[y, x] && !revealed[y, x])
                {
                    RevealCell(x, y);
                }
            }
        }

        private void RevealCell(int x, int y)
        {
            if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight || revealed[y, x])
                return;

            revealed[y, x] = true;
            cells[y, x].Enabled = false;

            if (mines[y, x])
            {
                cells[y, x].Text = "X";
                cells[y, x].BackColor = Color.Red;
                GameOver(false);
                return;
            }

            score += 10;
            cells[y, x].BackColor = Color.LightGreen;
            
            if (adjacentMines[y, x] > 0)
            {
                cells[y, x].Text = adjacentMines[y, x].ToString();
            }
            else
            {
                // Recursively reveal adjacent cells
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        RevealCell(x + dx, y + dy);
                    }
                }
            }

            // Check win condition
            bool won = true;
            for (int cy = 0; cy < gridHeight; cy++)
            {
                for (int cx = 0; cx < gridWidth; cx++)
                {
                    if (!mines[cy, cx] && !revealed[cy, cx])
                    {
                        won = false;
                        break;
                    }
                }
                if (!won) break;
            }

            if (won)
            {
                GameOver(true);
            }
        }

        private void GameOver(bool won)
        {
            HighScoreManager.SaveScore("Minesweeper", score);
            
            string message = won ? 
                $"You Won! Score: {score}\n\nWanna Try Again?" : 
                $"Game Over! Score: {score}\n\nWanna Try Again?";
            
            var result = MessageBox.Show(message, won ? "Victory" : "Game Over", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
                InitializeGame();
            else
                this.Close();
        }

        private void MinesweeperForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.P)
            {
                isPaused = !isPaused;
                foreach (var cell in cells)
                {
                    cell.Enabled = !isPaused;
                }
                if (isPaused)
                    MessageBox.Show("Game Paused", "Pause");
            }
        }

        private void ShowSettings()
        {
            Form settingsForm = new Form
            {
                Text = "Minesweeper Settings",
                Size = new Size(300, 250),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog
            };

            Label diffLabel = new Label
            {
                Text = "Difficulty:",
                Location = new Point(20, 20),
                AutoSize = true
            };
            settingsForm.Controls.Add(diffLabel);

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
                Location = new Point(100, 150),
                Size = new Size(80, 30)
            };
            okBtn.Click += (s, e) =>
            {
                difficulty = difficultyBox.SelectedItem?.ToString() ?? "Medium";
                settingsForm.Close();
                InitializeGame();
            };
            settingsForm.Controls.Add(okBtn);

            settingsForm.ShowDialog(this);
        }

        private void ShowScoreboard()
        {
            var scores = HighScoreManager.LoadScores("Minesweeper");
            string message = "Top Scores:\n\n";
            for (int i = 0; i < Math.Min(10, scores.Count); i++)
            {
                message += $"{i + 1}. {scores[i].Score} - {scores[i].Date:yyyy-MM-dd}\n";
            }
            if (scores.Count == 0)
                message = "No scores yet!";
            
            MessageBox.Show(message, "Scoreboard");
        }
    }
}
