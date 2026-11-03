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
        Assert.DoesNotContain("mongodb", conteudo);
    }

    [Fact]
    public async Task GetHealthReady_ChecaServicosExternos_IncluiOracleEMongodb()
    {
        // O readiness precisa cobrir as dependências externas, não só o banco relacional:
        // foi a ausência de um segundo serviço externo que a correção da Sprint 3 apontou.
        // Este teste trava isso — remover ou renomear o check do MongoDB passa a quebrar a
        // suíte em vez de passar despercebido.
        //
        // A asserção é sobre o check existir e ser consultado, não sobre o MongoDB estar de
        // pé: ele é dependência opcional (failureStatus Degraded), então exigir "Healthy"
        // aqui tornaria o teste um verificador de ambiente, não da configuração da API.

        // Act
        var response = await _client.GetAsync("/health/ready");
        var conteudo = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("\"name\":\"oracle-db\"", conteudo);
        Assert.Contains("\"name\":\"mongodb\"", conteudo);
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
    public async Task GetHealth_Combinado_IncluiSelfOracleEMongodb()
    {
        // Act
        var response = await _client.GetAsync("/health");
        var conteudo = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"name\":\"self\"", conteudo);
        Assert.Contains("\"name\":\"oracle-db\"", conteudo);
        Assert.Contains("\"name\":\"mongodb\"", conteudo);
    }
}
