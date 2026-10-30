using Microsoft.Extensions.Logging;
using PetCareHub.Application.DTOs;
using PetCareHub.Application.Diagnostics;
using PetCareHub.Application.Repositories;
using PetCareHub.Application.Services.Interfaces;
using PetCareHub.Domain.Entities;
using PetCareHub.Application.DTOs.Paging;

namespace PetCareHub.Application.Services.Implementations;

public sealed class ClinicaService(
    IClinicaRepository clinicaRepository,
    ILogger<ClinicaService> logger) : IClinicaService
{
    public PagedResponse<ClinicaResponse> GetAll(PageQuery consulta)
    {
        return PagedResponse<ClinicaResponse>.From(
            clinicaRepository.GetPaged(consulta),
            consulta,
            ClinicaResponse.FromDomain);
    }
    
    public ClinicaResponse? GetById(long id)
    {
        var clinica = clinicaRepository.GetById(id);
        return clinica is null ? null : ClinicaResponse.FromDomain(clinica);
    }
    
    // Create removido de propósito: esta API não cadastra clínica.
    //
    // O .NET não mapeia SENHA_HASH — autenticação é responsabilidade exclusiva
    // da API Java — então uma clínica criada aqui nasceria sem senha. E clínica
    // sem senha pode ser reivindicada por qualquer um que conheça o e-mail, o
    // CNPJ (que é público) e o código de credenciamento, que é um só para toda
    // a rede. O caminho correto é POST /auth/cadastro-clinica no Java, onde a
    // própria clínica define a senha, seguido da aprovação de um admin.

    public ClinicaResponse? Update(long id, ClinicaRequest request)
    {
        using var activity = AppTelemetry.Source.StartActivity("ClinicaService.Update");
        activity?.SetTag("clinica.id", id);

        var clinica = clinicaRepository.GetById(id);
        if (clinica is null)
            return null;

        if (request.Cnpj != clinica.Cnpj && clinicaRepository.ExistsByCnpj(request.Cnpj))
        {
            logger.LogWarning("Tentativa de atualizar clínica {ClinicaId} para CNPJ {Cnpj} já cadastrado", id, request.Cnpj);
            throw new InvalidOperationException($"Já existe uma clínica com CNPJ {request.Cnpj}.");
        }

        clinica.Nome = request.Nome;
        clinica.Cnpj = request.Cnpj;
        clinica.Email = request.Email;
        clinica.Telefone = request.Telefone;
        clinica.Endereco = request.Endereco;
        clinica.Ativo = request.Ativo;

        clinicaRepository.Update(clinica);

        logger.LogInformation("Clínica {ClinicaId} atualizada com sucesso", clinica.Id);

        return ClinicaResponse.FromDomain(clinica);
    }
    
    public bool Delete(long id)
    {
        using var activity = AppTelemetry.Source.StartActivity("ClinicaService.Delete");
        activity?.SetTag("clinica.id", id);

        var clinica = clinicaRepository.GetById(id);
        if (clinica is null)
            return false;

        try
        {
            clinica.GarantirQuePodeSerExcluida(clinicaRepository.HasPets(id));
        }
        catch (InvalidOperationException)
        {
            logger.LogWarning("Tentativa de deletar clínica {ClinicaId} que possui pets vinculados", id);
            throw;
        }

        var deletado = clinicaRepository.Delete(id);

        if (deletado)
            logger.LogInformation("Clínica {ClinicaId} deletada com sucesso", id);

        return deletado;
    }
}