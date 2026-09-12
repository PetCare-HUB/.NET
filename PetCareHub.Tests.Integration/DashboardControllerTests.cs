using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace PetCareHub.Tests.Integration;

[Collection("PetCareHub API collection")]
public class DashboardControllerTests(PetCareHubApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateAuthenticatedClient();

    [Fact]
    public async Task GetResumoClinica_ClinicaExistente_RetornaStatusOkEJson()
    {
        // Arrange — cria a própria clínica de teste (não depende de nenhum id fixo já
        // existente na base compartilhada, que pode não existir em outro ambiente/Oracle).
        var cnpj = GerarCnpjUnico();
        var criacao = await _client.PostAsJsonAsync("/api/Clinicas", new
        {
            nome = "Clínica Dashboard Teste",
            cnpj,
            email = $"dashboard.{cnpj}@petcare.com",
            telefone = (string?)null,
            endereco = (string?)null,
            ativo = true
        });
        criacao.EnsureSuccessStatusCode();
        using var corpoCriacao = JsonDocument.Parse(await criacao.Content.ReadAsStringAsync());
        var clinicaId = corpoCriacao.RootElement.GetProperty("id").GetInt64();

        try
        {
            // Act
            var response = await _client.GetAsync($"/api/Dashboard/clinicas/{clinicaId}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

            var corpo = await response.Content.ReadAsStringAsync();
            Assert.Contains("indicadores", corpo);
            Assert.Contains("totalPets", corpo);
        }
        finally
        {
            // Cleanup garantido mesmo se um Assert falhar acima.
            await _client.DeleteAsync($"/api/Clinicas/{clinicaId}");
        }
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

    private static string GerarCnpjUnico()
    {
        // 14 dígitos numéricos, sem máscara (regra do Create) — não precisa ser um CNPJ
        // válido de verdade para o teste, só único o bastante pra não colidir entre execuções.
        var sufixo = DateTime.UtcNow.Ticks.ToString()[^12..];
        return $"77{sufixo}";
    }
}
