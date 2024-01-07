using Shouldly;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins.Sessionize;

namespace MattEland.AI.Semantic.Workshop.Tests;

public class SessionizePluginTests
{
    [Theory]
    [InlineData("Matt Eland")]
    [InlineData("Samuel Gomez")]
    [InlineData("Sarah Dutkiewicz")]
    public async Task GetSpeakerDetailsShouldContainExpectedString(string name)
    {
        // Arrange
        SessionizePlugin plugin = new();

        // Act
        string details = await plugin.GetSpeakerDetails(name);

        // Assert
        details.ShouldNotBeNull();
        details.ShouldContain($"{name} is speaking on ");
    }

    [Theory]
    [InlineData("Bruce Wayne")]
    [InlineData("Dick Grayson")]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetSpeakerDetailsShouldHandleSpeakerNotFound(string? name)
    {
        // Arrange
        SessionizePlugin plugin = new();

        // Act
        string details = await plugin.GetSpeakerDetails(name!);

        // Assert
        details.ShouldNotBeNull();
        details.ShouldContain($"Could not find a speaker named '{name}'");
    }

}