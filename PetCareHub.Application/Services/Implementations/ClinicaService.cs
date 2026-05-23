using PetCareHub.Application.DTOs;
using PetCareHub.Application.Repositories;
using PetCareHub.Application.Services.Interfaces;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.Services.Implementations;

public sealed class ClinicaService(
    IClinicaRepository clinicaRepository) : IClinicaService
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
            throw new InvalidOperationException($"Já existe uma clínica com CNPJ {request.Cnpj}.");

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

        return ClinicaResponse.FromDomain(clinica);
    }
    
    public ClinicaResponse? Update(long id, ClinicaRequest request)
    {
        var clinica = clinicaRepository.GetById(id);
        if (clinica is null)
            return null;

        if (request.Cnpj != clinica.Cnpj && clinicaRepository.ExistsByCnpj(request.Cnpj))
            throw new InvalidOperationException($"Já existe uma clínica com CNPJ {request.Cnpj}.");

        clinica.Nome = request.Nome;
        clinica.Cnpj = request.Cnpj;
        clinica.Email = request.Email;
        clinica.Telefone = request.Telefone;
        clinica.Endereco = request.Endereco;
        clinica.Ativo = request.Ativo;

        clinicaRepository.Update(clinica);

        return ClinicaResponse.FromDomain(clinica);
    }
    
    public bool Delete(long id)
    {
        var clinica = clinicaRepository.GetById(id);
        if (clinica is null)
            return false;

        if (clinicaRepository.HasPets(id))
            throw new InvalidOperationException("Não é possível deletar uma clínica que possui pets vinculados.");

        return clinicaRepository.Delete(id);
    }
}