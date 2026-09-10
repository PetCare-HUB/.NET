using PetCareHub.Application.DTOs;
using PetCareHub.Application.Repositories;
using PetCareHub.Application.Services.Interfaces;

namespace PetCareHub.Application.Services.Implementations;

public sealed class TutorService(
    ITutorRepository tutorRepository,
    IClinicaRepository clinicaRepository) : ITutorService
{
    public IReadOnlyList<TutorResponse> GetAll()
    {
        return tutorRepository
            .GetAll()
            .Select(TutorResponse.FromDomain)
            .ToList();
    }

    public TutorResponse? GetById(long id)
    {
        var tutor = tutorRepository.GetById(id);
        return tutor is null ? null : TutorResponse.FromDomain(tutor);
    }

    public IReadOnlyList<TutorResponse> GetByClinica(long clinicaId)
    {
        if (!clinicaRepository.Exists(clinicaId))
            throw new KeyNotFoundException($"Clínica com id {clinicaId} não encontrada.");

        return tutorRepository
            .GetByClinica(clinicaId)
            .Select(TutorResponse.FromDomain)
            .ToList();
    }
}
