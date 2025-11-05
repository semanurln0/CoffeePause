using System.Drawing;
using System.Windows.Forms;

namespace CoffeePause;

public class HighscoresForm : Form
{
    private ComboBox _gameSelector = null!;
    private ListBox _scoresList = null!;

    public HighscoresForm()
    {
        InitializeComponent();
        LoadHighscores();
    }

    private void InitializeComponent()
    {
        this.Text = "Highscores";
        this.Size = new Size(500, 450);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        var titleLabel = new Label
        {
            Text = "HIGHSCORES",
            Location = new Point(150, 20),
            Size = new Size(200, 30),
            Font = new Font("Arial", 18, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter
        };
        this.Controls.Add(titleLabel);

        var gameLabel = new Label
        {
            Text = "Select Game:",
            Location = new Point(20, 70),
            Size = new Size(100, 25),
            Font = new Font("Arial", 11)
        };
        this.Controls.Add(gameLabel);

        _gameSelector = new ComboBox
        {
            Location = new Point(130, 70),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Arial", 10)
        };
        _gameSelector.Items.AddRange(new object[] { 
            "Pac-Man", 
            "Minesweeper", 
            "Spider Solitaire", 
            "Sudoku" 
        });
        _gameSelector.SelectedIndex = 0;
        _gameSelector.SelectedIndexChanged += (s, e) => LoadHighscores();
        this.Controls.Add(_gameSelector);

        _scoresList = new ListBox
        {
            Location = new Point(20, 110),
            Size = new Size(440, 250),
            Font = new Font("Courier New", 10)
        };
        this.Controls.Add(_scoresList);

        var closeButton = new Button
        {
            Text = "Close",
            Location = new Point(190, 375),
            Size = new Size(100, 35),
            Font = new Font("Arial", 11),
            BackColor = Color.LightGray
        };
        closeButton.Click += (s, e) => this.Close();
        this.Controls.Add(closeButton);
    }

    private void LoadHighscores()
    {
        _scoresList.Items.Clear();
        
        var gameName = _gameSelector.SelectedItem?.ToString() ?? "Pac-Man";
        var scores = HighScoreManager.Instance.GetScores(gameName);

        if (scores.Count == 0)
        {
            _scoresList.Items.Add("No highscores yet!");
            return;
        }

        _scoresList.Items.Add("Rank  Player          Score    Time      Date");
        _scoresList.Items.Add("".PadRight(60, '-'));

        int rank = 1;
        foreach (var score in scores)
        {
            var timeStr = score.Time.TotalSeconds > 0 
                ? score.Time.ToString(@"mm\:ss") 
                : "N/A";
            
            var line = string.Format("{0,-5} {1,-15} {2,-8} {3,-9} {4}",
                rank + ".",
                score.PlayerName,
                score.Score,
                timeStr,
                score.Date.ToString("MM/dd/yy"));
            
            _scoresList.Items.Add(line);
            rank++;
        }
    }
}
