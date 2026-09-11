using System.Net;

namespace PetCareHub.Tests.Integration;

[Collection("PetCareHub API collection")]
public class DashboardControllerTests(PetCareHubApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    // ClinicaId real existente na base compartilhada (mesma usada em AuthorizationTests).
    private const long ClinicaExistente = 10;

    [Fact]
    public async Task GetResumoClinica_ClinicaExistente_RetornaStatusOkEJson()
    {
        // Arrange — client já configurado no construtor

        // Act
        var response = await _client.GetAsync($"/api/Dashboard/clinicas/{ClinicaExistente}");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task GetResumoClinica_ClinicaInexistente_RetornaNotFound()
    {
        // Arrange — id bem alto, não deve existir na base de testes
        const long clinicaInexistente = 999_999;

        // Act
        var response = await _client.GetAsync($"/api/Dashboard/clinicas/{clinicaInexistente}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
