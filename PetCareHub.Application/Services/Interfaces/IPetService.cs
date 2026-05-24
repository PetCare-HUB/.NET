using PetCareHub.Application.DTOs;

namespace PetCareHub.Application.Services.Interfaces;

public interface IPetService
{
    IReadOnlyList<PetResponse> GetAll();

    IReadOnlyList<PetResponse> GetFiltered(long? clinicaId, string? especie, bool? ativo);

    PetResponse? GetById(long id);

    IReadOnlyList<PetResponse> GetByClinica(long clinicaId);

    PetResponse Create(PetRequest request);

    PetResponse? Update(long id, PetRequest request);

    bool Delete(long id);
}