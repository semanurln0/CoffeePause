namespace GameLauncher;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "CoffeePause - Game Launcher";
        this.Size = new Size(800, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.Sizable;
        this.KeyPreview = true;
        this.KeyDown += MainForm_KeyDown;
        this.Resize += MainForm_Resize;
        this.DoubleBuffered = true;
        
        // Set form icon from SVG
        try
        {
            var logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "sprites", "project-logo.svg");
            var icon = IconConverter.LoadIconFromSvg(logoPath, 64);
            if (icon != null)
            {
                this.Icon = icon;
            }
        }
        catch
        {
            // Icon loading failed, continue without custom icon
        }
        
        // Create menu strip
        var menuStrip = new MenuStrip();
        var fileMenu = new ToolStripMenuItem("File");
        var exitMenuItem = new ToolStripMenuItem("Exit");
        exitMenuItem.Click += (s, e) => Application.Exit();
        fileMenu.DropDownItems.Add(exitMenuItem);
        
        var helpMenu = new ToolStripMenuItem("Help");
        var aboutMenuItem = new ToolStripMenuItem("About");
        aboutMenuItem.Click += ShowAbout;
        helpMenu.DropDownItems.Add(aboutMenuItem);
        
        menuStrip.Items.Add(fileMenu);
        menuStrip.Items.Add(helpMenu);
        this.MainMenuStrip = menuStrip;
        this.Controls.Add(menuStrip);
        
        // Create main panel
        var mainPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(245, 245, 245),
            Padding = new Padding(20)
        };
        
        // Load and set background image
        try
        {
            var backgroundImage = AssetManager.LoadImage("main-menu-background.jpg");
            mainPanel.BackgroundImage = backgroundImage;
            mainPanel.BackgroundImageLayout = ImageLayout.Stretch;
        }
        catch
        {
            // Keep default background color if image loading fails
        }
        
        // Calculate initial center position
        int formCenterX = this.ClientSize.Width / 2;
        
        // Title label with logo
        var logoImage = AssetManager.LoadSvgAsImage("project-logo.svg", 64, 64);
        if (logoImage != null)
        {
            var logoPictureBox = new PictureBox
            {
                Image = logoImage,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(64, 64),
                Location = new Point(formCenterX - 150, 40),
                BackColor = Color.Transparent
            };
            mainPanel.Controls.Add(logoPictureBox);
        }
        
        var titleLabel = new Label
        {
            Text = "CoffeePause",
            Font = new Font("Segoe UI", 28, FontStyle.Bold),
            ForeColor = Color.FromArgb(74, 166, 186),
            AutoSize = true,
            Location = new Point(formCenterX - 80, 50)
        };
        mainPanel.Controls.Add(titleLabel);
        
        // Subtitle
        var subtitleLabel = new Label
        {
            Text = "Choose Game",
            Font = new Font("Segoe UI", 12),
            ForeColor = Color.Gray,
            AutoSize = true,
            Location = new Point(formCenterX - 50, 100)
        };
        mainPanel.Controls.Add(subtitleLabel);
        
        // Game buttons panel
        var gamesPanel = new TableLayoutPanel
        {
            Name = "gamesPanel",
            Location = new Point((this.ClientSize.Width - 500) / 2, 150),
            Size = new Size(500, 350),
            ColumnCount = 2,
            RowCount = 2,
            Padding = new Padding(10),
            CellBorderStyle = TableLayoutPanelCellBorderStyle.None
        };
        
        gamesPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        gamesPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        gamesPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        gamesPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        
        // Create game buttons
        var pacmanBtn = CreateGameButton("Pac-Man", Color.FromArgb(255, 213, 0));
        pacmanBtn.Click += (s, e) => LaunchGame("PacMan");
        
        var sudokuBtn = CreateGameButton("Sudoku", Color.FromArgb(76, 175, 80));
        sudokuBtn.Click += (s, e) => LaunchGame("Sudoku");
        
        var minesweeperBtn = CreateGameButton("Minesweeper", Color.FromArgb(33, 150, 243));
        minesweeperBtn.Click += (s, e) => LaunchGame("Minesweeper");
        
        var solitaireBtn = CreateGameButton("Spider Solitaire", Color.FromArgb(156, 39, 176));
        solitaireBtn.Click += (s, e) => LaunchGame("SpiderSolitaire");
        
        gamesPanel.Controls.Add(pacmanBtn, 0, 0);
        gamesPanel.Controls.Add(sudokuBtn, 1, 0);
        gamesPanel.Controls.Add(minesweeperBtn, 0, 1);
        gamesPanel.Controls.Add(solitaireBtn, 1, 1);
        
        mainPanel.Controls.Add(gamesPanel);
        this.Controls.Add(mainPanel);
    }
    
    private Button CreateGameButton(string gameName, Color color)
    {
        var button = new Button
        {
            Text = gameName,
            Size = new Size(220, 150),
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            BackColor = color,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(10)
        };
        
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color, 0.1f);
        button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(color, 0.1f);
        
        return button;
    }
    
    private void LaunchGame(string gameName)
    {
        Form? gameForm = gameName switch
        {
            "PacMan" => new PacManForm(),
            "Sudoku" => new SudokuForm(),
            "Minesweeper" => new MinesweeperForm(),
            "SpiderSolitaire" => new SpiderSolitaireForm(),
            _ => null
        };
        
        if (gameForm != null)
        {
            gameForm.ShowDialog(this);
        }
    }
    
    private void ShowAbout(object? sender, EventArgs e)
    {
        MessageBox.Show(
            "CoffeePause - Game Library\n\n" +
            "A collection of classic games:\n" +
            "- Pac-Man\n" +
            "- Sudoku\n" +
            "- Minesweeper\n" +
            "- Spider Solitaire\n\n" +
            "Version 1.0\n\n" +
            "For help, contact: semanurln0.code@gmail.com",
            "About CoffeePause",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );
    }
    
    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            Application.Exit();
        }
    }
    
    private void MainForm_Resize(object? sender, EventArgs e)
    {
        // Find the main panel and games panel
        Panel? mainPanel = null;
        TableLayoutPanel? gamesPanel = null;
        PictureBox? logoPictureBox = null;
        Label? titleLabel = null;
        Label? subtitleLabel = null;
        
        foreach (Control control in this.Controls)
        {
            if (control is Panel panel && panel.BackgroundImage != null)
            {
                mainPanel = panel;
                break;
            }
        }
        
        if (mainPanel != null)
        {
            foreach (Control control in mainPanel.Controls)
            {
                if (control.Name == "gamesPanel" || control is TableLayoutPanel)
                {
                    gamesPanel = control as TableLayoutPanel;
                }
                else if (control is PictureBox)
                {
                    logoPictureBox = control as PictureBox;
                }
                else if (control is Label label)
                {
                    if (label.Text == "CoffeePause")
                    {
                        titleLabel = label;
                    }
                    else if (label.Text == "Choose Game")
                    {
                        subtitleLabel = label;
                    }
                }
            }
            
            // Center the games panel
            if (gamesPanel != null)
            {
                int centerX = (this.ClientSize.Width - gamesPanel.Width) / 2;
                int centerY = (this.ClientSize.Height - gamesPanel.Height) / 2 + 20;
                gamesPanel.Location = new Point(Math.Max(10, centerX), Math.Max(150, centerY));
            }
            
            // Reposition title elements based on new center
            int formCenterX = this.ClientSize.Width / 2;
            
            if (logoPictureBox != null)
            {
                logoPictureBox.Location = new Point(formCenterX - 150, 40);
            }
            
            if (titleLabel != null)
            {
                titleLabel.Location = new Point(formCenterX - 80, 50);
            }
            
            if (subtitleLabel != null)
            {
                subtitleLabel.Location = new Point(formCenterX - subtitleLabel.Width / 2, 100);
            }
        }
    }
}
