namespace PetCareHub.Tests.Integration;

[Collection("PetCareHub API collection")]
public class ScoresSaudeControllerTests(PetCareHubApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateAuthenticatedClient();

    [Fact]
    public async Task GetScoresSaude_RetornaStatusOkEJson()
    {
        // Arrange — client já configurado no construtor

        // Act
        var response = await _client.GetAsync("/api/ScoresSaude");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }
}
