using PetCareHub.Application.DTOs;
using PetCareHub.Application.Repositories;
using PetCareHub.Application.Services.Interfaces;
using PetCareHub.Application.DTOs.Paging;

namespace PetCareHub.Application.Services.Implementations;

public sealed class ScoreSaudeService(IScoreSaudeRepository scoreSaudeRepository) : IScoreSaudeService
{
    public PagedResponse<ScoreSaudeResponse> GetAll(PageQuery consulta)
    {
        return PagedResponse<ScoreSaudeResponse>.From(
            scoreSaudeRepository.GetPaged(consulta),
            consulta,
            ScoreSaudeResponse.FromDomain);
    }

    public IReadOnlyList<ScoreSaudeResponse> GetFiltered(
        long? petId,
        long? clinicaId,
        string? categoria,
        int? scoreMin,
        int? scoreMax)
    {
        return scoreSaudeRepository
            .GetFiltered(petId, clinicaId, categoria, scoreMin, scoreMax)
            .Select(ScoreSaudeResponse.FromDomain)
            .ToList();
    }

    public ScoreSaudeResponse? GetById(long id)
    {
        var score = scoreSaudeRepository.GetById(id);
        return score is null ? null : ScoreSaudeResponse.FromDomain(score);
    }

    public IReadOnlyList<ScoreSaudeResponse> GetByPet(long petId)
    {
        return scoreSaudeRepository
            .GetByPet(petId)
            .Select(ScoreSaudeResponse.FromDomain)
            .ToList();
    }

    public IReadOnlyList<ScoreSaudeResponse> GetByClinica(long clinicaId)
    {
        return scoreSaudeRepository
            .GetByClinica(clinicaId)
            .Select(ScoreSaudeResponse.FromDomain)
            .ToList();
    }

    public ScoreSaudeResponse? GetLatestByPet(long petId)
    {
        var score = scoreSaudeRepository.GetLatestByPet(petId);
        return score is null ? null : ScoreSaudeResponse.FromDomain(score);
    }
}