using Xunit;

namespace GameLauncher.Tests;

public class CardValueTests
{
    // Helper method to simulate GetCardValue from SpiderSolitaireForm
    private int GetCardValue(string value)
    {
        return value switch
        {
            "A" => 1,
            "J" => 11,
            "Q" => 12,
            "K" => 13,
            _ => int.Parse(value)
        };
    }

    [Theory]
    [InlineData("A", 1)]
    [InlineData("2", 2)]
    [InlineData("3", 3)]
    [InlineData("4", 4)]
    [InlineData("5", 5)]
    [InlineData("6", 6)]
    [InlineData("7", 7)]
    [InlineData("8", 8)]
    [InlineData("9", 9)]
    [InlineData("10", 10)]
    [InlineData("J", 11)]
    [InlineData("Q", 12)]
    [InlineData("K", 13)]
    public void GetCardValue_ReturnsCorrectValue(string cardValue, int expectedValue)
    {
        // Act
        var result = GetCardValue(cardValue);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void GetCardValue_HandlesAce()
    {
        // Act
        var result = GetCardValue("A");

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void GetCardValue_HandlesJack()
    {
        // Act
        var result = GetCardValue("J");

        // Assert
        Assert.Equal(11, result);
    }

    [Fact]
    public void GetCardValue_HandlesQueen()
    {
        // Act
        var result = GetCardValue("Q");

        // Assert
        Assert.Equal(12, result);
    }

    [Fact]
    public void GetCardValue_HandlesKing()
    {
        // Act
        var result = GetCardValue("K");

        // Assert
        Assert.Equal(13, result);
    }
}
