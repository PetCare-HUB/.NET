using Microsoft.Extensions.Logging;
using PetCareHub.Application.DTOs;
using PetCareHub.Application.Repositories;
using PetCareHub.Application.Services.Interfaces;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.Services.Implementations;

public sealed class ClinicaService(
    IClinicaRepository clinicaRepository,
    ILogger<ClinicaService> logger) : IClinicaService
{
    public IReadOnlyList<ClinicaResponse> GetAll()
    {
        return clinicaRepository
            .GetAll()
            .Select(ClinicaResponse.FromDomain)
            .ToList();
    }
    
    public ClinicaResponse? GetById(long id)
    {
        var clinica = clinicaRepository.GetById(id);
        return clinica is null ? null : ClinicaResponse.FromDomain(clinica);
    }
    
    public ClinicaResponse Create(ClinicaRequest request)
    {
        if (clinicaRepository.ExistsByCnpj(request.Cnpj))
        {
            logger.LogWarning("Tentativa de criar clínica com CNPJ {Cnpj} já cadastrado", request.Cnpj);
            throw new InvalidOperationException($"Já existe uma clínica com CNPJ {request.Cnpj}.");
        }

        var clinica = new Clinica
        {
            Nome = request.Nome,
            Cnpj = request.Cnpj,
            Email = request.Email,
            Telefone = request.Telefone,
            Endereco = request.Endereco,
            Ativo = request.Ativo
        };

        clinicaRepository.Add(clinica);

        logger.LogInformation("Clínica {ClinicaId} ({Nome}) criada com sucesso", clinica.Id, clinica.Nome);

        return ClinicaResponse.FromDomain(clinica);
    }
    
    public ClinicaResponse? Update(long id, ClinicaRequest request)
    {
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
        var clinica = clinicaRepository.GetById(id);
        if (clinica is null)
            return false;

        if (clinicaRepository.HasPets(id))
        {
            logger.LogWarning("Tentativa de deletar clínica {ClinicaId} que possui pets vinculados", id);
            throw new InvalidOperationException("Não é possível deletar uma clínica que possui pets vinculados.");
        }

        var deletado = clinicaRepository.Delete(id);

        if (deletado)
            logger.LogInformation("Clínica {ClinicaId} deletada com sucesso", id);

        return deletado;
    }
}