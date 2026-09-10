using PetCareHub.Application.DTOs;

namespace PetCareHub.Application.Services.Interfaces;

public interface ITutorService
{
    IReadOnlyList<TutorResponse> GetAll();

    TutorResponse? GetById(long id);

    IReadOnlyList<TutorResponse> GetByClinica(long clinicaId);
}
