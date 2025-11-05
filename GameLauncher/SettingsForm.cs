using System.Drawing;
using System.Windows.Forms;

namespace CoffeePause;

public class SettingsForm : Form
{
    private CheckBox _soundCheckBox = null!;
    private ComboBox _difficultyComboBox = null!;
    private NumericUpDown _minesweeperWidthInput = null!;
    private NumericUpDown _minesweeperHeightInput = null!;
    private NumericUpDown _minesweeperMinesInput = null!;

    public SettingsForm()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void InitializeComponent()
    {
        this.Text = "Settings";
        this.Size = new Size(500, 400);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        int y = 20;

        // Sound Settings
        var soundLabel = new Label
        {
            Text = "Sound:",
            Location = new Point(20, y),
            Size = new Size(100, 25),
            Font = new Font("Arial", 12)
        };
        this.Controls.Add(soundLabel);

        _soundCheckBox = new CheckBox
        {
            Text = "Enable Sound",
            Location = new Point(150, y),
            Size = new Size(150, 25),
            Font = new Font("Arial", 10)
        };
        this.Controls.Add(_soundCheckBox);

        y += 40;

        // Difficulty Settings
        var difficultyLabel = new Label
        {
            Text = "Difficulty:",
            Location = new Point(20, y),
            Size = new Size(100, 25),
            Font = new Font("Arial", 12)
        };
        this.Controls.Add(difficultyLabel);

        _difficultyComboBox = new ComboBox
        {
            Location = new Point(150, y),
            Size = new Size(150, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Arial", 10)
        };
        _difficultyComboBox.Items.AddRange(new object[] { "Easy", "Medium", "Hard" });
        _difficultyComboBox.SelectedIndexChanged += DifficultyChanged;
        this.Controls.Add(_difficultyComboBox);

        y += 50;

        // Minesweeper Settings Group
        var minesweeperGroupBox = new GroupBox
        {
            Text = "Minesweeper Settings",
            Location = new Point(20, y),
            Size = new Size(440, 150),
            Font = new Font("Arial", 11, FontStyle.Bold)
        };
        this.Controls.Add(minesweeperGroupBox);

        int groupY = 30;

        var widthLabel = new Label
        {
            Text = "Grid Width:",
            Location = new Point(20, groupY),
            Size = new Size(100, 25),
            Font = new Font("Arial", 10, FontStyle.Regular)
        };
        minesweeperGroupBox.Controls.Add(widthLabel);

        _minesweeperWidthInput = new NumericUpDown
        {
            Location = new Point(150, groupY),
            Size = new Size(80, 25),
            Minimum = 5,
            Maximum = 30,
            Font = new Font("Arial", 10)
        };
        minesweeperGroupBox.Controls.Add(_minesweeperWidthInput);

        groupY += 35;

        var heightLabel = new Label
        {
            Text = "Grid Height:",
            Location = new Point(20, groupY),
            Size = new Size(100, 25),
            Font = new Font("Arial", 10, FontStyle.Regular)
        };
        minesweeperGroupBox.Controls.Add(heightLabel);

        _minesweeperHeightInput = new NumericUpDown
        {
            Location = new Point(150, groupY),
            Size = new Size(80, 25),
            Minimum = 5,
            Maximum = 30,
            Font = new Font("Arial", 10)
        };
        minesweeperGroupBox.Controls.Add(_minesweeperHeightInput);

        groupY += 35;

        var minesLabel = new Label
        {
            Text = "Mine Count:",
            Location = new Point(20, groupY),
            Size = new Size(100, 25),
            Font = new Font("Arial", 10, FontStyle.Regular)
        };
        minesweeperGroupBox.Controls.Add(minesLabel);

        _minesweeperMinesInput = new NumericUpDown
        {
            Location = new Point(150, groupY),
            Size = new Size(80, 25),
            Minimum = 1,
            Maximum = 200,
            Font = new Font("Arial", 10)
        };
        minesweeperGroupBox.Controls.Add(_minesweeperMinesInput);

        // Buttons
        var saveButton = new Button
        {
            Text = "Save",
            Location = new Point(280, 320),
            Size = new Size(80, 30),
            Font = new Font("Arial", 10),
            BackColor = Color.LightGreen
        };
        saveButton.Click += SaveButton_Click;
        this.Controls.Add(saveButton);

        var cancelButton = new Button
        {
            Text = "Cancel",
            Location = new Point(380, 320),
            Size = new Size(80, 30),
            Font = new Font("Arial", 10),
            BackColor = Color.LightCoral
        };
        cancelButton.Click += (s, e) => this.Close();
        this.Controls.Add(cancelButton);
    }

    private void LoadSettings()
    {
        var settings = GameSettings.Instance;
        _soundCheckBox.Checked = settings.SoundEnabled;
        _difficultyComboBox.SelectedIndex = (int)settings.Difficulty;
        _minesweeperWidthInput.Value = settings.Minesweeper.GridWidth;
        _minesweeperHeightInput.Value = settings.Minesweeper.GridHeight;
        _minesweeperMinesInput.Value = settings.Minesweeper.MineCount;
    }

    private void DifficultyChanged(object? sender, EventArgs e)
    {
        var difficulty = (DifficultyLevel)_difficultyComboBox.SelectedIndex;
        var tempSettings = new MinesweeperSettings();
        tempSettings.SetDifficulty(difficulty);
        
        _minesweeperWidthInput.Value = tempSettings.GridWidth;
        _minesweeperHeightInput.Value = tempSettings.GridHeight;
        _minesweeperMinesInput.Value = tempSettings.MineCount;
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        var settings = GameSettings.Instance;
        settings.SoundEnabled = _soundCheckBox.Checked;
        settings.Difficulty = (DifficultyLevel)_difficultyComboBox.SelectedIndex;
        settings.Minesweeper.GridWidth = (int)_minesweeperWidthInput.Value;
        settings.Minesweeper.GridHeight = (int)_minesweeperHeightInput.Value;
        settings.Minesweeper.MineCount = (int)_minesweeperMinesInput.Value;
        
        settings.Save();
        
        MessageBox.Show("Settings saved successfully!", "Success", 
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        
        this.Close();
    }
}
