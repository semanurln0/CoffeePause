using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CoffeePause.Tests;

// Copy of HighScoreManager for testing
public class HighScoreManager
{
    public string ScoresDir { get; set; }
    private Dictionary<string, List<ScoreEntry>> _scores = new();

    public HighScoreManager(string scoresDir)
    {
        ScoresDir = scoresDir;
        LoadAll();
    }

    public void AddScore(string gameName, int score, TimeSpan time, string playerName = "Player")
    {
        if (!_scores.ContainsKey(gameName)) 
            _scores[gameName] = new List<ScoreEntry>();

        _scores[gameName].Add(new ScoreEntry 
        { 
            PlayerName = playerName, 
            Score = score, 
            Time = time, 
            Date = DateTime.Now 
        });

        _scores[gameName] = _scores[gameName]
            .OrderByDescending(s => s.Score)
            .ThenBy(s => s.Time)
            .Take(10)
            .ToList();

        SaveGameScores(gameName);
    }

    public List<ScoreEntry> GetScores(string gameName)
    {
        return _scores.ContainsKey(gameName) ? _scores[gameName] : new List<ScoreEntry>();
    }

    private string GetScoresPath(string gameName)
    {
        var safe = string.Concat(gameName.Where(c => !Path.GetInvalidFileNameChars().Contains(c)));
        return Path.Combine(ScoresDir, $"{safe}-highscores.json");
    }

    private void SaveGameScores(string gameName)
    {
        try
        {
            Directory.CreateDirectory(ScoresDir);
            var path = GetScoresPath(gameName);
            var tmp = path + ".tmp";
            var json = JsonSerializer.Serialize(_scores[gameName], new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(tmp, json);
            if (File.Exists(path)) 
                File.Replace(tmp, path, null);
            else 
                File.Move(tmp, path);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save highscores for {gameName}: {ex.Message}");
        }
    }

    private void LoadAll()
    {
        try
        {
            if (!Directory.Exists(ScoresDir)) return;
            var files = Directory.GetFiles(ScoresDir, "*-highscores.json");
            foreach (var f in files)
            {
                try
                {
                    var gameName = Path.GetFileNameWithoutExtension(f).Replace("-highscores", "");
                    var json = File.ReadAllText(f);
                    var list = JsonSerializer.Deserialize<List<ScoreEntry>>(json) ?? new List<ScoreEntry>();
                    _scores[gameName] = list;
                }
                catch { }
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
