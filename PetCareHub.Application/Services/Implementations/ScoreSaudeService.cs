using PetCareHub.Application.DTOs;
using PetCareHub.Application.Repositories;
using PetCareHub.Application.Services.Interfaces;

namespace PetCareHub.Application.Services.Implementations;

public sealed class ScoreSaudeService(IScoreSaudeRepository scoreSaudeRepository) : IScoreSaudeService
{
    public IReadOnlyList<ScoreSaudeResponse> GetAll()
    {
        return scoreSaudeRepository
            .GetAll()
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