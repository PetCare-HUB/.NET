using PetCareHub.Application.DTOs;

namespace PetCareHub.Application.Services.Interfaces;

public interface IPetService
{
    IReadOnlyList<PetResponse> GetAll();

    PetResponse? GetById(long id);

    IReadOnlyList<PetResponse> GetByClinica(long clinicaId);
}