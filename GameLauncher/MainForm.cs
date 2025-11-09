using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameLauncher
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "CoffeePause - Game Library";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Title Label
            Label titleLabel = new Label
            {
                Text = "CoffeePause",
                Font = new Font("Arial", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(116, 166, 186),
                AutoSize = true,
                Location = new Point(180, 30)
            };
            this.Controls.Add(titleLabel);

            Label subtitleLabel = new Label
            {
                Text = "Select a game to play",
                Font = new Font("Arial", 12, FontStyle.Regular),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(215, 75)
            };
            this.Controls.Add(subtitleLabel);

            // Game Buttons
            int buttonY = 120;
            int buttonSpacing = 70;

            Button pacManBtn = CreateGameButton("Pac-Man", buttonY);
            pacManBtn.Click += (s, e) => OpenGame(new PacManForm());
            this.Controls.Add(pacManBtn);

            Button sudokuBtn = CreateGameButton("Sudoku", buttonY + buttonSpacing);
            sudokuBtn.Click += (s, e) => OpenGame(new SudokuForm());
            this.Controls.Add(sudokuBtn);

            Button minesweeperBtn = CreateGameButton("Minesweeper", buttonY + buttonSpacing * 2);
            minesweeperBtn.Click += (s, e) => OpenGame(new MinesweeperForm());
            this.Controls.Add(minesweeperBtn);

            Button spiderBtn = CreateGameButton("Spider Solitaire", buttonY + buttonSpacing * 3);
            spiderBtn.Click += (s, e) => OpenGame(new SpiderSolitaireForm());
            this.Controls.Add(spiderBtn);

            // Menu
            MenuStrip menu = new MenuStrip();
            ToolStripMenuItem settingsMenu = new ToolStripMenuItem("Settings");
            settingsMenu.Click += (s, e) => MessageBox.Show("Global settings (placeholder)", "Settings");
            menu.Items.Add(settingsMenu);

            ToolStripMenuItem aboutMenu = new ToolStripMenuItem("About");
            aboutMenu.Click += (s, e) => MessageBox.Show("CoffeePause Game Library\nVersion 1.0\nA collection of classic games", "About");
            menu.Items.Add(aboutMenu);

            ToolStripMenuItem exitMenu = new ToolStripMenuItem("Exit");
            exitMenu.Click += (s, e) => Application.Exit();
            menu.Items.Add(exitMenu);

            this.MainMenuStrip = menu;
            this.Controls.Add(menu);

            this.BackColor = Color.FromArgb(250, 253, 250);
        }

        private Button CreateGameButton(string text, int y)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(300, 50),
                Location = new Point(150, y),
                Font = new Font("Arial", 14, FontStyle.Bold),
                BackColor = Color.FromArgb(116, 166, 186),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void OpenGame(Form gameForm)
        {
            gameForm.ShowDialog(this);
        }
    }
}
