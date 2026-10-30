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
        // Arrange — semeia a própria clínica de teste (não depende de nenhum id fixo já
        // existente na base compartilhada, que pode não existir em outro ambiente/Oracle).
        var cnpj = GerarCnpjUnico();
        var clinicaId = factory.SemearClinica(
            "Clínica Dashboard Teste", cnpj, $"dashboard.{cnpj}@petcare.com");

        // O dashboard é da clínica: o token precisa ser dela.
        var clinicaDona = factory.CreateAuthenticatedClient(clinicaId: clinicaId);

        try
        {
            // Act
            var response = await clinicaDona.GetAsync($"/api/Dashboard/clinicas/{clinicaId}");

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
            factory.RemoverClinicaSemeada(clinicaId);
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
