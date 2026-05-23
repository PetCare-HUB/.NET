using PetCareHub.Application.DTOs;
using PetCareHub.Application.Repositories;
using PetCareHub.Application.Services.Interfaces;

namespace PetCareHub.Application.Services.Implementations;

public sealed class ResponsavelService(IResponsavelRepository responsavelRepository) : IResponsavelService
{
    public IReadOnlyList<ResponsavelResponse> GetAll()
    {
        return responsavelRepository
            .GetAll()
            .Select(ResponsavelResponse.FromDomain)
            .ToList();
    }

    public ResponsavelResponse? GetById(long id)
    {
        var responsavel = responsavelRepository.GetById(id);
        return responsavel is null ? null : ResponsavelResponse.FromDomain(responsavel);
    }
    
    public IReadOnlyList<ResponsavelResponse> GetByClinica(long clinicaId)
    {
        return responsavelRepository
            .GetByClinica(clinicaId)
            .Select(ResponsavelResponse.FromDomain)
            .ToList();
    }
}