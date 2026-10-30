using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetCareHub.API;
using PetCareHub.Domain.Entities;
using PetCareHub.Infrastructure.Persistence;

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

    /// <summary>
    /// Insere uma clínica direto pelo DbContext e devolve o id gerado.
    /// </summary>
    /// <remarks>
    /// Esta API não expõe criação de clínica — quem entra na rede se cadastra
    /// em /auth/cadastro-clinica na API Java. Então arranjar o estado de um
    /// teste é trabalho do fixture, não de um endpoint que não deveria existir.
    /// </remarks>
    public long SemearClinica(string nome, string cnpj, string? email = null)
    {
        using var escopo = Services.CreateScope();
        var contexto = escopo.ServiceProvider.GetRequiredService<PetCareHubContext>();

        var clinica = new Clinica
        {
            Nome = nome,
            Cnpj = cnpj,
            Email = email,
            Endereco = "Rua de Teste, 1",
            Ativo = true,
            StatusAcesso = "PRE_CADASTRADO"
        };

        contexto.Set<Clinica>().Add(clinica);
        contexto.SaveChanges();
        return clinica.Id;
    }

    /// <summary>
    /// Remove uma clínica semeada. Ignora o filtro por clínica de propósito: o
    /// fixture roda fora de uma requisição, sem claim nenhuma, então nada
    /// estaria visível para ele.
    /// </summary>
    public void RemoverClinicaSemeada(long id)
    {
        using var escopo = Services.CreateScope();
        var contexto = escopo.ServiceProvider.GetRequiredService<PetCareHubContext>();

        var clinica = contexto.Set<Clinica>()
            .IgnoreQueryFilters()
            .FirstOrDefault(c => c.Id == id);

        if (clinica is null)
            return;

        contexto.Set<Clinica>().Remove(clinica);
        contexto.SaveChanges();
    }
}
