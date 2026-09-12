using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace PetCareHub.Tests.Integration;

[Collection("PetCareHub API collection")]
public class ClinicasControllerTests(PetCareHubApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateAuthenticatedClient();

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

    [Fact]
    public async Task Clinica_CriarConsultarAtualizarExcluir_FluxoCompletoPersisteNoOracle()
    {
        // Fluxo de ponta a ponta contra o Oracle real da FIAP: cada etapa confere tanto o
        // corpo da resposta quanto o efeito persistido, buscando de novo com uma requisição
        // HTTP separada — não basta o Create "parecer" certo, o dado tem que estar lá depois.
        var cnpj = GerarCnpjUnico();
        long clinicaId;

        // Create
        var criacao = await _client.PostAsJsonAsync("/api/Clinicas", new
        {
            nome = "Clínica CRUD Teste",
            cnpj,
            email = $"crud.{cnpj}@petcare.com",
            telefone = (string?)null,
            endereco = "Rua de Teste, 1",
            ativo = true
        });
        criacao.EnsureSuccessStatusCode();
        using (var corpoCriacao = JsonDocument.Parse(await criacao.Content.ReadAsStringAsync()))
        {
            Assert.Equal("Clínica CRUD Teste", corpoCriacao.RootElement.GetProperty("nome").GetString());
            clinicaId = corpoCriacao.RootElement.GetProperty("id").GetInt64();
        }

        try
        {
            // Read — confirma que o que foi criado está de fato persistido, não só na
            // resposta do Create
            var leituraInicial = await _client.GetAsync($"/api/Clinicas/{clinicaId}");
            leituraInicial.EnsureSuccessStatusCode();
            using (var corpoLeitura = JsonDocument.Parse(await leituraInicial.Content.ReadAsStringAsync()))
            {
                Assert.Equal(cnpj, corpoLeitura.RootElement.GetProperty("cnpj").GetString());
            }

            // Update
            var atualizacao = await _client.PutAsJsonAsync($"/api/Clinicas/{clinicaId}", new
            {
                nome = "Clínica CRUD Teste Atualizada",
                cnpj,
                email = $"crud.{cnpj}@petcare.com",
                telefone = (string?)null,
                endereco = "Rua de Teste, 2",
                ativo = true
            });
            atualizacao.EnsureSuccessStatusCode();

            var leituraAposUpdate = await _client.GetAsync($"/api/Clinicas/{clinicaId}");
            leituraAposUpdate.EnsureSuccessStatusCode();
            using (var corpoAposUpdate = JsonDocument.Parse(await leituraAposUpdate.Content.ReadAsStringAsync()))
            {
                Assert.Equal("Clínica CRUD Teste Atualizada", corpoAposUpdate.RootElement.GetProperty("nome").GetString());
                Assert.Equal("Rua de Teste, 2", corpoAposUpdate.RootElement.GetProperty("endereco").GetString());
            }

            // Delete
            var exclusao = await _client.DeleteAsync($"/api/Clinicas/{clinicaId}");
            Assert.Equal(HttpStatusCode.NoContent, exclusao.StatusCode);

            var leituraAposDelete = await _client.GetAsync($"/api/Clinicas/{clinicaId}");
            Assert.Equal(HttpStatusCode.NotFound, leituraAposDelete.StatusCode);
        }
        catch
        {
            // Se qualquer assert acima falhar antes do Delete "oficial" do fluxo, ainda
            // assim não deixa a clínica de teste presa no banco compartilhado.
            await _client.DeleteAsync($"/api/Clinicas/{clinicaId}");
            throw;
        }
    }

    private static string GerarCnpjUnico()
    {
        var sufixo = DateTime.UtcNow.Ticks.ToString()[^12..];
        return $"88{sufixo}";
    }
}
