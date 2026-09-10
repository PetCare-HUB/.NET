using System.Net;

namespace PetCareHub.Tests.Integration;

[Collection("PetCareHub API collection")]
public class HealthCheckTests(PetCareHubApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetHealth_RetornaJsonComStatusDoOracle()
    {
        // Arrange — client já configurado no construtor

        // Act
        var response = await _client.GetAsync("/health");
        var conteudo = await response.Content.ReadAsStringAsync();

        // Assert — status pode ser 200 (Healthy) ou 503 (Unhealthy), mas sempre
        // responde e sempre inclui o check "oracle-db" no corpo
        Assert.True(
            response.StatusCode is HttpStatusCode.OK or HttpStatusCode.ServiceUnavailable);
        Assert.Contains("oracle-db", conteudo);
    }
}
