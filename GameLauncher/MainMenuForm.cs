using System.Drawing;
using System.Windows.Forms;

namespace CoffeePause;

public class MainMenuForm : Form
{
    private Image? _backgroundImage;
    private Image? _logoImage;
    
    public MainMenuForm()
    {
        InitializeComponent();
        LoadBackground();
        LoadLogo();
    }

    private void InitializeComponent()
    {
        this.Text = "CoffeePause";
        this.Size = new Size(800, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;

        // Logo (if available, will be added in LoadLogo)
        
        // Title Label (will be hidden if logo is present)
        var titleLabel = new Label
        {
            Text = "COFFEEPAUSE",
            Font = new Font("Arial", 32, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(200, 50),
            Name = "titleLabel"
        };
        this.Controls.Add(titleLabel);

        // Game Buttons
        var buttonY = 180;
        var buttonHeight = 50;
        var buttonSpacing = 20;

        AddGameButton("Pac-Man", buttonY, (s, e) => LaunchPacMan());
        buttonY += buttonHeight + buttonSpacing;
        
        AddGameButton("Minesweeper", buttonY, (s, e) => LaunchMinesweeper());
        buttonY += buttonHeight + buttonSpacing;
        
        AddGameButton("Spider Solitaire", buttonY, (s, e) => LaunchSpiderSolitaire());
        buttonY += buttonHeight + buttonSpacing;
        
        AddGameButton("Sudoku", buttonY, (s, e) => LaunchSudoku());
        buttonY += buttonHeight + buttonSpacing;

        // Settings and Highscores buttons
        var settingsButton = new Button
        {
            Text = "Settings",
            Location = new Point(50, 500),
            Size = new Size(120, 40),
            Font = new Font("Arial", 12),
            BackColor = Color.LightGray
        };
        settingsButton.Click += (s, e) => ShowSettings();
        this.Controls.Add(settingsButton);

        var highscoresButton = new Button
        {
            Text = "Highscores",
            Location = new Point(200, 500),
            Size = new Size(120, 40),
            Font = new Font("Arial", 12),
            BackColor = Color.LightGray
        };
        highscoresButton.Click += (s, e) => ShowHighscores();
        this.Controls.Add(highscoresButton);

        var exitButton = new Button
        {
            Text = "Exit",
            Location = new Point(630, 500),
            Size = new Size(120, 40),
            Font = new Font("Arial", 12),
            BackColor = Color.IndianRed
        };
        exitButton.Click += (s, e) => this.Close();
        this.Controls.Add(exitButton);
    }

    private void LoadLogo()
    {
        _logoImage = AssetManager.Instance.GetLogoImage();
        if (_logoImage != null)
        {
            var logoPictureBox = new PictureBox
            {
                Image = _logoImage,
                Location = new Point(250, 20),
                Size = new Size(300, 150),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            this.Controls.Add(logoPictureBox);
            logoPictureBox.BringToFront();
            
            // Hide text title if logo is present
            var titleLabel = this.Controls.Find("titleLabel", false).FirstOrDefault();
            if (titleLabel != null)
            {
                titleLabel.Visible = false;
            }
        }
    }

    private void AddGameButton(string gameName, int y, EventHandler clickHandler)
    {
        var button = new Button
        {
            Text = gameName,
            Location = new Point(250, y),
            Size = new Size(300, 50),
            Font = new Font("Arial", 16, FontStyle.Bold),
            BackColor = Color.DodgerBlue,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        button.FlatAppearance.BorderSize = 0;
        button.Click += clickHandler;
        this.Controls.Add(button);
    }

    private void LoadBackground()
    {
        _backgroundImage = AssetManager.Instance.GetBackgroundImage();
        if (_backgroundImage != null)
        {
            this.BackgroundImage = _backgroundImage;
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }
        else
        {
            // Fallback gradient background
            this.BackColor = Color.FromArgb(20, 30, 50);
        }
    }

    private void LaunchPacMan()
    {
        var pacManForm = new PacManForm();
        pacManForm.ShowDialog();
    }

    private void LaunchMinesweeper()
    {
        var minesweeperForm = new MinesweeperForm();
        minesweeperForm.ShowDialog();
    }

    private void LaunchSpiderSolitaire()
    {
        var solitaireForm = new SpiderSolitaireForm();
        solitaireForm.ShowDialog();
    }

    private void LaunchSudoku()
    {
        var sudokuForm = new SudokuForm();
        sudokuForm.ShowDialog();
    }

    private void ShowSettings()
    {
        var settingsForm = new SettingsForm();
        settingsForm.ShowDialog();
    }

    private void ShowHighscores()
    {
        var highscoresForm = new HighscoresForm();
        highscoresForm.ShowDialog();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        AssetManager.Instance.Dispose();
    }
}
