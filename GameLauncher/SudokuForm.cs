using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameLauncher
{
    public partial class SudokuForm : Form
    {
        private const int GridSize = 9;
        private const int CellSize = 50;
        
        private int[,] puzzle;
        private int[,] solution;
        private bool[,] isFixed;
        private TextBox[,] cells;
        private bool isPaused;

        public SudokuForm()
        {
            InitializeComponent();
            GenerateNewPuzzle();
        }

        private void InitializeComponent()
        {
            this.Text = "Sudoku";
            this.ClientSize = new Size(GridSize * CellSize + 200, GridSize * CellSize + 40);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.KeyPreview = true;

            this.KeyDown += SudokuForm_KeyDown;

            Panel gridPanel = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(GridSize * CellSize, GridSize * CellSize),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(gridPanel);

            cells = new TextBox[GridSize, GridSize];
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    TextBox cell = new TextBox
                    {
                        Location = new Point(col * CellSize, row * CellSize),
                        Size = new Size(CellSize, CellSize),
                        Font = new Font("Arial", 16, FontStyle.Bold),
                        TextAlign = HorizontalAlignment.Center,
                        MaxLength = 1,
                        Tag = new Point(row, col)
                    };
                    
                    int r = row, c = col;
                    cell.KeyPress += (s, e) => Cell_KeyPress(s, e, r, c);
                    cell.MouseDown += Cell_MouseDown;
                    
                    cells[row, col] = cell;
                    gridPanel.Controls.Add(cell);
                }
            }

            Button newPuzzleBtn = new Button
            {
                Text = "New Puzzle",
                Location = new Point(GridSize * CellSize + 20, 20),
                Size = new Size(170, 40)
            };
            newPuzzleBtn.Click += (s, e) => GenerateNewPuzzle();
            this.Controls.Add(newPuzzleBtn);

            Button scoreboardBtn = new Button
            {
                Text = "Scoreboard",
                Location = new Point(GridSize * CellSize + 20, 70),
                Size = new Size(170, 40)
            };
            scoreboardBtn.Click += (s, e) => ShowScoreboard();
            this.Controls.Add(scoreboardBtn);

            Button checkBtn = new Button
            {
                Text = "Check Solution",
                Location = new Point(GridSize * CellSize + 20, 120),
                Size = new Size(170, 40)
            };
            checkBtn.Click += (s, e) => CheckSolution();
            this.Controls.Add(checkBtn);
        }

        private void Cell_KeyPress(object sender, KeyPressEventArgs e, int row, int col)
        {
            if (isFixed[row, col])
            {
                e.Handled = true;
                return;
            }

            if (e.KeyChar >= '1' && e.KeyChar <= '9')
            {
                TextBox cell = (TextBox)sender;
                bool isDraft = (ModifierKeys & Keys.Control) == Keys.Control;
                
                cell.Text = e.KeyChar.ToString();
                cell.ForeColor = isDraft ? Color.Red : Color.Blue;
                puzzle[row, col] = int.Parse(e.KeyChar.ToString());
                
                e.Handled = true;
            }
            else if (e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Delete)
            {
                TextBox cell = (TextBox)sender;
                cell.Text = "";
                puzzle[row, col] = 0;
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Cell_MouseDown(object sender, MouseEventArgs e)
        {
            TextBox cell = (TextBox)sender;
            Point pos = (Point)cell.Tag;
            
            if (isFixed[pos.X, pos.Y])
                return;

            if (e.Button == MouseButtons.Right)
            {
                // Right click = draft mode (red)
                cell.ForeColor = Color.Red;
            }
        }

        private void SudokuForm_KeyDown(object sender, KeyEventArgs e)
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

        private void GenerateNewPuzzle()
        {
            solution = new int[GridSize, GridSize];
            puzzle = new int[GridSize, GridSize];
            isFixed = new bool[GridSize, GridSize];

            // Generate a simple valid sudoku
            GenerateSolution();
            
            // Remove numbers to create puzzle
            Random rand = new Random();
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    puzzle[row, col] = solution[row, col];
                    if (rand.Next(100) < 60) // 60% cells removed
                    {
                        puzzle[row, col] = 0;
                        isFixed[row, col] = false;
                    }
                    else
                    {
                        isFixed[row, col] = true;
                    }
                }
            }

            UpdateUI();
        }

        private void GenerateSolution()
        {
            int[,] baseSolution = new int[,]
            {
                {5,3,4,6,7,8,9,1,2},
                {6,7,2,1,9,5,3,4,8},
                {1,9,8,3,4,2,5,6,7},
                {8,5,9,7,6,1,4,2,3},
                {4,2,6,8,5,3,7,9,1},
                {7,1,3,9,2,4,8,5,6},
                {9,6,1,5,3,7,2,8,4},
                {2,8,7,4,1,9,6,3,5},
                {3,4,5,2,8,6,1,7,9}
            };

            // Shuffle rows/cols within blocks for variety
            Random rand = new Random();
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    solution[i, j] = (baseSolution[i, j] + rand.Next(GridSize)) % 9 + 1;
                }
            }
            
            // Use base solution for simplicity
            Array.Copy(baseSolution, solution, baseSolution.Length);
        }

        private void UpdateUI()
        {
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    if (puzzle[row, col] != 0)
                    {
                        cells[row, col].Text = puzzle[row, col].ToString();
                        cells[row, col].ForeColor = isFixed[row, col] ? Color.Black : Color.Blue;
                        cells[row, col].ReadOnly = isFixed[row, col];
                        cells[row, col].BackColor = isFixed[row, col] ? Color.LightGray : Color.White;
                    }
                    else
                    {
                        cells[row, col].Text = "";
                        cells[row, col].ForeColor = Color.Blue;
                        cells[row, col].ReadOnly = false;
                        cells[row, col].BackColor = Color.White;
                    }
                }
            }
        }

        private void CheckSolution()
        {
            bool isCorrect = true;
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    if (puzzle[row, col] == 0 || puzzle[row, col] != solution[row, col])
                    {
                        isCorrect = false;
                        break;
                    }
                }
                if (!isCorrect) break;
            }

            if (isCorrect)
            {
                int score = 1000; // Base score
                HighScoreManager.SaveScore("Sudoku", score);
                var result = MessageBox.Show($"Correct! Score: {score}\n\nWanna Try Again?", "Success", 
                    MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                    GenerateNewPuzzle();
                else
                    this.Close();
            }
            else
            {
                MessageBox.Show("Not quite right. Keep trying!", "Try Again");
            }
        }

        private void ShowScoreboard()
        {
            var scores = HighScoreManager.LoadScores("Sudoku");
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
