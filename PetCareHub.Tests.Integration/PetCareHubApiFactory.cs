using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using PetCareHub.API;

namespace PetCareHub.Tests.Integration;

public class PetCareHubApiFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// Cliente HTTP com um Bearer token válido (assinado com a chave de teste) já anexado —
    /// atalho para os testes que precisam simular uma clínica autenticada. A maioria dos
    /// endpoints exige role CLINICA (é o dashboard B2B da clínica).
    /// </summary>
    public HttpClient CreateAuthenticatedClient(string role = "CLINICA", long? clinicaId = 1, long? tutorId = null)
    {
        var client = CreateClient();
        var token = TestTokenFactory.CreateToken(role, clinicaId, tutorId);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
