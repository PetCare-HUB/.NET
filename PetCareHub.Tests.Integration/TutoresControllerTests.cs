using System.Net;

namespace PetCareHub.Tests.Integration;

[Collection("PetCareHub API collection")]
public class TutoresControllerTests(PetCareHubApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateAuthenticatedClient();

    [Fact]
    public async Task GetTutores_SemFiltro_RetornaStatusOkEJson()
    {
        // Arrange — client já configurado no construtor

        // Act
        var response = await _client.GetAsync("/api/Tutores");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task GetByClinica_ClinicaInexistente_RetornaNotFound()
    {
        // Arrange — id bem alto, não deve existir na base de testes
        const long clinicaInexistente = 999_999;

        // Act
        var response = await _client.GetAsync($"/api/Tutores/clinica/{clinicaInexistente}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
