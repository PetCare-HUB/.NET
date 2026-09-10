namespace PetCareHub.Tests.Integration;

[Collection("PetCareHub API collection")]
public class ClinicasControllerTests(PetCareHubApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetClinicas_RetornaStatusOkEJson()
    {
        // Arrange — client já configurado no construtor

        // Act
        var response = await _client.GetAsync("/api/Clinicas");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }
}
