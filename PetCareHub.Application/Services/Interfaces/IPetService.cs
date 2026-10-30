using PetCareHub.Application.DTOs;
using PetCareHub.Application.DTOs.Paging;

namespace PetCareHub.Application.Services.Interfaces;

public interface IPetService
{
    PagedResponse<PetResponse> GetAll(PageQuery consulta);

    IReadOnlyList<PetResponse> GetFiltered(long? clinicaId, string? especie, bool? ativo);

    PetResponse? GetById(long id);

    IReadOnlyList<PetResponse> GetByClinica(long clinicaId);

    PetResponse Create(PetRequest request);

    PetResponse? Update(long id, PetRequest request);

    bool Delete(long id);
}