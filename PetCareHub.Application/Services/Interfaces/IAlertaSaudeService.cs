using PetCareHub.Application.DTOs;
using PetCareHub.Application.DTOs.Paging;

namespace PetCareHub.Application.Services.Interfaces;

public interface IAlertaSaudeService
{
    PagedResponse<AlertaSaudeResponse> GetAll(PageQuery consulta);

    IReadOnlyList<AlertaSaudeResponse> GetFiltered(long? petId, string? nivelAlerta, bool? resolvido);

    AlertaSaudeResponse? GetById(long id);

    IReadOnlyList<AlertaSaudeResponse> GetByPet(long petId);

    IReadOnlyList<AlertaSaudeResponse> GetByClinica(long clinicaId);

    AlertaSaudeResponse Create(AlertaSaudeRequest request);

    AlertaSaudeResponse? Update(long id, AlertaSaudeRequest request);

    bool Delete(long id);

    bool Resolve(long id);
}