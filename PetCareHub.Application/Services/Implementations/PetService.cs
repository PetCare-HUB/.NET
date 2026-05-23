using PetCareHub.Application.DTOs;
using PetCareHub.Application.Repositories;
using PetCareHub.Application.Services.Interfaces;

namespace PetCareHub.Application.Services.Implementations;

public sealed class PetService(IPetRepository petRepository) : IPetService
{
    public IReadOnlyList<PetResponse> GetAll()
    {
        return petRepository
            .GetAll()
            .Select(PetResponse.FromDomain)
            .ToList();
    }
    
    public PetResponse? GetById(long id)
    {
        var pet = petRepository.GetById(id);
        return pet is null ? null : PetResponse.FromDomain(pet);
    }
    
    public IReadOnlyList<PetResponse> GetByClinica(long clinicaId)
    {
        return petRepository
            .GetByClinica(clinicaId)
            .Select(PetResponse.FromDomain)
            .ToList();
    }
}