using System.Text.Json;

namespace CoffeePause;

public class GameSettings
{
    public bool SoundEnabled { get; set; } = true;
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;
    public MinesweeperSettings Minesweeper { get; set; } = new();
    
    private static GameSettings? _instance;
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "CoffeePause",
        "settings.json");

    public static GameSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Load();
            }
            return _instance;
        }
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsPath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save settings: {ex.Message}");
        }
    }

    private static GameSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                return JsonSerializer.Deserialize<GameSettings>(json) ?? new GameSettings();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load settings: {ex.Message}");
        }
        return new GameSettings();
    }
}

public enum DifficultyLevel
{
    Easy,
    Medium,
    Hard
}

public class MinesweeperSettings
{
    public int GridWidth { get; set; } = 10;
    public int GridHeight { get; set; } = 10;
    public int MineCount { get; set; } = 15;
    
    public void SetDifficulty(DifficultyLevel difficulty)
    {
        switch (difficulty)
        {
            case DifficultyLevel.Easy:
                GridWidth = 8;
                GridHeight = 8;
                MineCount = 10;
                break;
            case DifficultyLevel.Medium:
                GridWidth = 10;
                GridHeight = 10;
                MineCount = 15;
                break;
            case DifficultyLevel.Hard:
                GridWidth = 16;
                GridHeight = 16;
                MineCount = 40;
                break;
        }
    }
}
