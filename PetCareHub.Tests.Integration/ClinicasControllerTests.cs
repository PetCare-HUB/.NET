using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace PetCareHub.Tests.Integration;

[Collection("PetCareHub API collection")]
public class ClinicasControllerTests(PetCareHubApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateAuthenticatedClient();

    [Fact]
    public async Task GetClinicas_SemFiltro_RetornaStatusOkEJson()
    {
        // Arrange — client já configurado no construtor

        // Act
        var response = await _client.GetAsync("/api/Clinicas");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task PostClinica_NaoExisteNestaApi_RetornaMethodNotAllowed()
    {
        // O cadastro de clínica é da API Java (/auth/cadastro-clinica), onde a
        // própria clínica define a senha. Criar por aqui geraria clínica sem
        // senha, reivindicável por quem tiver o código de credenciamento.
        var resposta = await _client.PostAsJsonAsync("/api/Clinicas", new
        {
            nome = "Clínica Que Não Deveria Nascer",
            cnpj = GerarCnpjUnico(),
            ativo = true
        });

        Assert.Equal(HttpStatusCode.MethodNotAllowed, resposta.StatusCode);
    }

    [Fact]
    public async Task Clinica_ConsultarAtualizarExcluir_PersisteNoOracle()
    {
        // Fluxo contra o Oracle real da FIAP: cada etapa confere tanto o corpo da
        // resposta quanto o efeito persistido, buscando de novo com uma requisição
        // HTTP separada — não basta "parecer" certo, o dado tem que estar lá depois.
        var cnpj = GerarCnpjUnico();
        var clinicaId = factory.SemearClinica("Clínica CRUD Teste", cnpj, $"crud.{cnpj}@petcare.com");

        // A clínica olhando os próprios dados
        var clinicaDona = factory.CreateAuthenticatedClient(clinicaId: clinicaId);

        try
        {
            // Read — a resposta vem envelopada com os links HATEOAS
            var leituraInicial = await clinicaDona.GetAsync($"/api/Clinicas/{clinicaId}");
            leituraInicial.EnsureSuccessStatusCode();
            using (var corpoLeitura = JsonDocument.Parse(await leituraInicial.Content.ReadAsStringAsync()))
            {
                var dados = corpoLeitura.RootElement.GetProperty("data");
                Assert.Equal(cnpj, dados.GetProperty("cnpj").GetString());
                Assert.Equal("PRE_CADASTRADO", dados.GetProperty("statusAcesso").GetString());

                var links = corpoLeitura.RootElement.GetProperty("_links");
                Assert.Contains(links.EnumerateArray(), link => link.GetProperty("rel").GetString() == "self");
            }

            // Update — editar o próprio cadastro é legítimo para a clínica
            var atualizacao = await clinicaDona.PutAsJsonAsync($"/api/Clinicas/{clinicaId}", new
            {
                nome = "Clínica CRUD Teste Atualizada",
                cnpj,
                email = $"crud.{cnpj}@petcare.com",
                telefone = (string?)null,
                endereco = "Rua de Teste, 2",
                ativo = true
            });
            atualizacao.EnsureSuccessStatusCode();

            var leituraAposUpdate = await clinicaDona.GetAsync($"/api/Clinicas/{clinicaId}");
            leituraAposUpdate.EnsureSuccessStatusCode();
            using (var corpoAposUpdate = JsonDocument.Parse(await leituraAposUpdate.Content.ReadAsStringAsync()))
            {
                var dados = corpoAposUpdate.RootElement.GetProperty("data");
                Assert.Equal("Clínica CRUD Teste Atualizada", dados.GetProperty("nome").GetString());
                Assert.Equal("Rua de Teste, 2", dados.GetProperty("endereco").GetString());
            }

            // Delete — descredenciar é do admin
            var admin = factory.CreateAuthenticatedClient(role: "ADMIN", clinicaId: null);
            var exclusao = await admin.DeleteAsync($"/api/Clinicas/{clinicaId}");
            Assert.Equal(HttpStatusCode.NoContent, exclusao.StatusCode);

            var leituraAposDelete = await admin.GetAsync($"/api/Clinicas/{clinicaId}");
            Assert.Equal(HttpStatusCode.NotFound, leituraAposDelete.StatusCode);
        }
        finally
        {
            // Não deixa a clínica de teste presa no banco compartilhado se um
            // assert falhar antes do Delete do fluxo.
            factory.RemoverClinicaSemeada(clinicaId);
        }
    }

    [Fact]
    public async Task DeleteClinica_TokenDeClinica_RetornaForbidden()
    {
        // Descredenciar quem está na rede é decisão de quem administra a rede.
        var cnpj = GerarCnpjUnico();
        var clinicaId = factory.SemearClinica("Clínica Indelével", cnpj, $"indelevel.{cnpj}@petcare.com");

        try
        {
            var clinicaDona = factory.CreateAuthenticatedClient(clinicaId: clinicaId);
            var resposta = await clinicaDona.DeleteAsync($"/api/Clinicas/{clinicaId}");

            Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
        }
        finally
        {
            factory.RemoverClinicaSemeada(clinicaId);
        }
    }

    [Fact]
    public async Task GetClinicaPorId_DeOutraClinica_NaoVazaDados()
    {
        // Arrange
        var cnpj = GerarCnpjUnico();
        var idAlheio = factory.SemearClinica("Clínica Alheia", cnpj, $"alheia.{cnpj}@petcare.com");

        try
        {
            // Act — uma clínica qualquer tenta ler os dados da outra
            var intruso = factory.CreateAuthenticatedClient(clinicaId: idAlheio + 1);
            var resposta = await intruso.GetAsync($"/api/Clinicas/{idAlheio}");

            // Assert — o filtro global do EF não pode deixar vazar carteira de
            // pets, métricas nem agenda de uma clínica para outra
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }
        finally
        {
            factory.RemoverClinicaSemeada(idAlheio);
        }
    }

    private static string GerarCnpjUnico()
    {
        var sufixo = DateTime.UtcNow.Ticks.ToString()[^12..];
        return $"88{sufixo}";
    }
}
