using PetCareHub.Application.DTOs;

namespace PetCareHub.Application.Services.Interfaces;

public interface IScoreSaudeService
{
    IReadOnlyList<ScoreSaudeResponse> GetAll();

    IReadOnlyList<ScoreSaudeResponse> GetFiltered(
        long? petId,
        long? clinicaId,
        string? categoria,
        int? scoreMin,
        int? scoreMax);

    ScoreSaudeResponse? GetById(long id);

    IReadOnlyList<ScoreSaudeResponse> GetByPet(long petId);

    IReadOnlyList<ScoreSaudeResponse> GetByClinica(long clinicaId);

    ScoreSaudeResponse? GetLatestByPet(long petId);
}