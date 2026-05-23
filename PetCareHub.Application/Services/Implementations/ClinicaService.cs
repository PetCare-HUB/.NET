using PetCareHub.Application.DTOs;
using PetCareHub.Application.Repositories;
using PetCareHub.Application.Services.Interfaces;

namespace PetCareHub.Application.Services.Implementations;

public sealed class ClinicaService(IClinicaRepository clinicaRepository) : IClinicaService
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
}