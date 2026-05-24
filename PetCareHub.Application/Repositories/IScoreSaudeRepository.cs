using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.Repositories;

public interface IScoreSaudeRepository : IRepository<ScoreSaude>
{
    IEnumerable<ScoreSaude> GetByPet(long petId);

    IEnumerable<ScoreSaude> GetByClinica(long clinicaId);

    IEnumerable<ScoreSaude> GetFiltered(
        long? petId,
        long? clinicaId,
        string? categoria,
        int? scoreMin,
        int? scoreMax);

    ScoreSaude? GetLatestByPet(long petId);
}