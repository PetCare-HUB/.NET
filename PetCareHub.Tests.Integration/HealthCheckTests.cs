using System.Net;

namespace PetCareHub.Tests.Integration;

[Collection("PetCareHub API collection")]
public class HealthCheckTests(PetCareHubApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetHealthLive_ProcessoAtivo_RetornaHealthy()
    {
        // /health/live só verifica o próprio processo (nenhuma dependência externa), então
        // — diferente do endpoint combinado — não há cenário legítimo em que deva falhar
        // enquanto a API estiver de pé para responder à requisição.

        // Act
        var response = await _client.GetAsync("/health/live");
        var conteudo = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"status\":\"Healthy\"", conteudo);
        Assert.DoesNotContain("oracle-db", conteudo);
    }

    [Fact]
    public async Task GetHealthReady_OracleAcessivel_RetornaHealthy()
    {
        // Os testes de integração já exigem a mesma rede/VPN da FIAP para rodar (ver README),
        // então nesse ambiente o Oracle estar saudável é a expectativa real, não só "responde
        // alguma coisa" — se isso falhar, é sinal genuíno de problema de conectividade.

        // Act
        var response = await _client.GetAsync("/health/ready");
        var conteudo = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("oracle-db", conteudo);
        Assert.Contains("\"status\":\"Healthy\"", conteudo);
    }

    [Fact]
    public async Task GetHealth_Combinado_IncluiSelfEOracle()
    {
        // Act
        var response = await _client.GetAsync("/health");
        var conteudo = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"name\":\"self\"", conteudo);
        Assert.Contains("\"name\":\"oracle-db\"", conteudo);
    }
}
