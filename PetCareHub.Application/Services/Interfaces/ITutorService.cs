using PetCareHub.Application.DTOs;
using PetCareHub.Application.DTOs.Paging;

namespace PetCareHub.Application.Services.Interfaces;

public interface ITutorService
{
    PagedResponse<TutorResponse> GetAll(PageQuery consulta);

    TutorResponse? GetById(long id);

    IReadOnlyList<TutorResponse> GetByClinica(long clinicaId);
}
