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
        Assert.DoesNotContain("api-java", conteudo);
    }

    [Fact]
    public async Task GetHealthReady_ChecaDependenciasExternas_IncluiOracleMongodbEApiJava()
    {
        // A rubrica pede três coisas distintas no health check: a saúde da API, a
        // conectividade com o banco (Oracle/MongoDB) e a disponibilidade de serviços
        // externos. Oracle e Mongo cobrem só a segunda — os dois são banco. Foi a falta
        // da terceira que a correção da Sprint 3 apontou ("não há check de serviço
        // externo"), e quem a cobre é o "api-java".
        //
        // Este teste trava as três: remover ou renomear qualquer um dos checks passa a
        // quebrar a suíte em vez de passar despercebido.
        //
        // A asserção é sobre o check existir e ser consultado, não sobre a dependência
        // estar de pé: Mongo e API Java são Degraded-on-failure, então exigir "Healthy"
        // aqui tornaria o teste um verificador de ambiente, não da configuração da API.

        // Act
        var response = await _client.GetAsync("/health/ready");
        var conteudo = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("\"name\":\"oracle-db\"", conteudo);
        Assert.Contains("\"name\":\"mongodb\"", conteudo);
        Assert.Contains("\"name\":\"api-java\"", conteudo);
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
    public async Task GetHealth_Combinado_IncluiSelfOracleMongodbEApiJava()
    {
        // Act
        var response = await _client.GetAsync("/health");
        var conteudo = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"name\":\"self\"", conteudo);
        Assert.Contains("\"name\":\"oracle-db\"", conteudo);
        Assert.Contains("\"name\":\"mongodb\"", conteudo);
        Assert.Contains("\"name\":\"api-java\"", conteudo);
    }
}
