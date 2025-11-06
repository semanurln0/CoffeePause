using Newtonsoft.Json;

namespace GameLauncher;

public class HighScoreManager
{
    private readonly string _appDataPath;
    
    public HighScoreManager()
    {
        _appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CoffeePause"
        );
        Directory.CreateDirectory(_appDataPath);
    }
    
    public List<ScoreEntry> LoadScores(string gameName)
    {
        var filePath = GetScoreFilePath(gameName);
        if (!File.Exists(filePath))
            return new List<ScoreEntry>();
            
        try
        {
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<ScoreEntry>>(json) ?? new List<ScoreEntry>();
        }
        catch
        {
            return new List<ScoreEntry>();
        }
    }
    
    public void SaveScore(string gameName, ScoreEntry entry)
    {
        var scores = LoadScores(gameName);
        scores.Add(entry);
        scores = scores.OrderByDescending(s => s.Score).Take(10).ToList();
        
        var filePath = GetScoreFilePath(gameName);
        var tempPath = filePath + ".tmp";
        
        try
        {
            var json = JsonConvert.SerializeObject(scores, Formatting.Indented);
            File.WriteAllText(tempPath, json);
            
            if (File.Exists(filePath))
                File.Delete(filePath);
            
            File.Move(tempPath, filePath);
        }
        catch
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
            throw;
        }
    }
    
    private string GetScoreFilePath(string gameName)
    {
        return Path.Combine(_appDataPath, $"{gameName}_scores.json");
    }
}

public class ScoreEntry
{
    public string PlayerName { get; set; } = "Player";
    public int Score { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
}
