using CoffeePause.Tests;
using Xunit;

namespace GameLauncher.Tests;

public class HighScoreManagerTests : IDisposable
{
    private readonly string _testDir;
    private readonly HighScoreManager _manager;

    public HighScoreManagerTests()
    {
        // Create a temporary directory for test scores
        _testDir = Path.Combine(Path.GetTempPath(), "CoffeePauseTests_" + Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDir);
        _manager = new HighScoreManager(_testDir);
    }

    public void Dispose()
    {
        // Clean up test directory
        if (Directory.Exists(_testDir))
        {
            try
            {
                Directory.Delete(_testDir, true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    [Fact]
    public void AddScore_SavesScoreToList()
    {
        // Arrange
        var gameName = "TestGame";

        // Act
        _manager.AddScore(gameName, 100, TimeSpan.FromMinutes(5), "TestPlayer");

        // Assert
        var scores = _manager.GetScores(gameName);
        Assert.NotEmpty(scores);
        Assert.Equal(100, scores[0].Score);
        Assert.Equal("TestPlayer", scores[0].PlayerName);
    }

    [Fact]
    public void GetScores_ReturnsEmptyListForNonExistentGame()
    {
        // Arrange & Act
        var scores = _manager.GetScores("NonExistentGame");

        // Assert
        Assert.Empty(scores);
    }

    [Fact]
    public void AddScore_LimitsScoresToTop10()
    {
        // Arrange
        var gameName = "TestGame";

        // Act - Add 15 scores
        for (int i = 0; i < 15; i++)
        {
            _manager.AddScore(gameName, i * 10, TimeSpan.FromMinutes(i), $"Player{i}");
        }

        // Assert
        var scores = _manager.GetScores(gameName);
        Assert.Equal(10, scores.Count);
    }

    [Fact]
    public void AddScore_SortsScoresByScoreDescending()
    {
        // Arrange
        var gameName = "TestGame";

        // Act
        _manager.AddScore(gameName, 50, TimeSpan.FromMinutes(5), "Player1");
        _manager.AddScore(gameName, 100, TimeSpan.FromMinutes(3), "Player2");
        _manager.AddScore(gameName, 75, TimeSpan.FromMinutes(4), "Player3");

        // Assert
        var scores = _manager.GetScores(gameName);
        Assert.Equal(100, scores[0].Score);
        Assert.Equal(75, scores[1].Score);
        Assert.Equal(50, scores[2].Score);
    }

    [Fact]
    public void SaveAndLoad_Roundtrip()
    {
        // Arrange
        var gameName = "RoundtripTest";
        var testDir = Path.Combine(Path.GetTempPath(), "CoffeePauseRoundtrip_" + Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDir);

        try
        {
            // Act - Add score and save
            var manager1 = new HighScoreManager(testDir);
            manager1.AddScore(gameName, 200, TimeSpan.FromMinutes(10), "RoundtripPlayer");

            // Create new instance which should load from files
            var manager2 = new HighScoreManager(testDir);
            var scores = manager2.GetScores(gameName);

            // Assert
            Assert.NotEmpty(scores);
            Assert.Equal(200, scores[0].Score);
            Assert.Equal("RoundtripPlayer", scores[0].PlayerName);
        }
        finally
        {
            if (Directory.Exists(testDir))
            {
                Directory.Delete(testDir, true);
            }
        }
    }
}
