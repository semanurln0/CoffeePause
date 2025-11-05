using System.Text.Json;

namespace CoffeePause;

public class HighScoreManager
{
    private static readonly string ScoresPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "CoffeePause",
        "highscores.json");

    private Dictionary<string, List<ScoreEntry>> _scores = new();

    private static HighScoreManager? _instance;
    public static HighScoreManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new HighScoreManager();
                _instance.Load();
            }
            return _instance;
        }
    }

    public void AddScore(string gameName, int score, TimeSpan time, string playerName = "Player")
    {
        if (!_scores.ContainsKey(gameName))
        {
            _scores[gameName] = new List<ScoreEntry>();
        }

        _scores[gameName].Add(new ScoreEntry
        {
            PlayerName = playerName,
            Score = score,
            Time = time,
            Date = DateTime.Now
        });

        // Keep only top 10 scores
        _scores[gameName] = _scores[gameName]
            .OrderByDescending(s => s.Score)
            .ThenBy(s => s.Time)
            .Take(10)
            .ToList();

        Save();
    }

    public List<ScoreEntry> GetScores(string gameName)
    {
        return _scores.ContainsKey(gameName) 
            ? _scores[gameName] 
            : new List<ScoreEntry>();
    }

    private void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ScoresPath)!);
            var json = JsonSerializer.Serialize(_scores, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ScoresPath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save highscores: {ex.Message}");
        }
    }

    private void Load()
    {
        try
        {
            if (File.Exists(ScoresPath))
            {
                var json = File.ReadAllText(ScoresPath);
                _scores = JsonSerializer.Deserialize<Dictionary<string, List<ScoreEntry>>>(json) 
                    ?? new Dictionary<string, List<ScoreEntry>>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load highscores: {ex.Message}");
            _scores = new Dictionary<string, List<ScoreEntry>>();
        }
    }
}

public class ScoreEntry
{
    public string PlayerName { get; set; } = "Player";
    public int Score { get; set; }
    public TimeSpan Time { get; set; }
    public DateTime Date { get; set; }
}
