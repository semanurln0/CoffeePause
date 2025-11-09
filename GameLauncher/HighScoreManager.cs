using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace GameLauncher
{
    public class HighScoreEntry
    {
        public string PlayerName { get; set; } = "Player";
        public int Score { get; set; }
        public DateTime Date { get; set; }
    }

    public static class HighScoreManager
    {
        private static string GetScoreFilePath(string gameName)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string coffeePauseDir = Path.Combine(appData, "CoffeePause");
            Directory.CreateDirectory(coffeePauseDir);
            return Path.Combine(coffeePauseDir, $"{gameName}_scores.json");
        }

        public static List<HighScoreEntry> LoadScores(string gameName)
        {
            try
            {
                string path = GetScoreFilePath(gameName);
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    return JsonConvert.DeserializeObject<List<HighScoreEntry>>(json) ?? new List<HighScoreEntry>();
                }
            }
            catch { }
            return new List<HighScoreEntry>();
        }

        public static void SaveScore(string gameName, int score, string playerName = "Player")
        {
            try
            {
                var scores = LoadScores(gameName);
                scores.Add(new HighScoreEntry
                {
                    PlayerName = playerName,
                    Score = score,
                    Date = DateTime.Now
                });
                scores.Sort((a, b) => b.Score.CompareTo(a.Score));
                if (scores.Count > 10)
                    scores = scores.GetRange(0, 10);

                string path = GetScoreFilePath(gameName);
                string tempPath = path + ".tmp";
                File.WriteAllText(tempPath, JsonConvert.SerializeObject(scores, Formatting.Indented));
                if (File.Exists(path))
                    File.Delete(path);
                File.Move(tempPath, path);
            }
            catch { }
        }
    }
}
