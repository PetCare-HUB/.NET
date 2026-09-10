using System.Net;
using System.Net.Http.Json;

namespace PetCareHub.Tests.Integration;

[Collection("PetCareHub API collection")]
public class ConsultasControllerTests(PetCareHubApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetConsultas_RetornaStatusOkEJson()
    {
        // Arrange — client já configurado no construtor

        // Act
        var response = await _client.GetAsync("/api/Consultas");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task PostConsulta_TipoConsultaInvalido_RetornaBadRequest()
    {
        // Arrange — corpo com TipoConsulta fora do enum aceito, falha na validação
        // antes de qualquer acesso ao banco
        var body = JsonContent.Create(new
        {
            petId = 1,
            clinicaId = 1,
            dataConsulta = DateTime.Now,
            tipoConsulta = "TIPO_INVALIDO",
            valor = 100.00
        });

        // Act
        var response = await _client.PostAsync("/api/Consultas", body);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
