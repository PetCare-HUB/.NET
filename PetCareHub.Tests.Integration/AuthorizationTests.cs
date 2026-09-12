using System.Net;
using System.Net.Http.Headers;

namespace PetCareHub.Tests.Integration;

/// <summary>
/// Cobre os três cenários de autenticação/autorização exigidos para a API: acesso
/// autorizado, token ausente/inválido e perfil sem permissão para o recurso.
/// </summary>
[Collection("PetCareHub API collection")]
public class AuthorizationTests(PetCareHubApiFactory factory)
{
    [Fact]
    public async Task GetClinicas_SemToken_RetornaUnauthorized()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/Clinicas");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetClinicas_TokenMalformado_RetornaUnauthorized()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "isto-nao-e-um-jwt-valido");

        var response = await client.GetAsync("/api/Clinicas");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetClinicas_TokenExpirado_RetornaUnauthorized()
    {
        var client = factory.CreateClient();
        var tokenExpirado = TestTokenFactory.CreateExpiredToken();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenExpirado);

        var response = await client.GetAsync("/api/Clinicas");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetClinicas_TokenValidoComRoleTutor_RetornaForbidden()
    {
        // Perfil autenticado, mas sem permissão para este recurso — o dashboard clínico é
        // exclusivo de CLINICA (um TUTOR autêntico não deveria conseguir listar clínicas).
        var client = factory.CreateAuthenticatedClient(role: "TUTOR", clinicaId: null, tutorId: 1);

        var response = await client.GetAsync("/api/Clinicas");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetClinicas_TokenValidoComRoleClinica_RetornaOk()
    {
        var client = factory.CreateAuthenticatedClient(role: "CLINICA");

        var response = await client.GetAsync("/api/Clinicas");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetHealthLive_SemToken_NaoExigeAutenticacao()
    {
        // /health precisa continuar acessível sem token — é o que uma ferramenta de
        // monitoramento externa chama, sem credencial de clínica nenhuma.
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
