using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.Repositories;

public interface IScoreSaudeRepository : IRepository<ScoreSaude>
{
    IEnumerable<ScoreSaude> GetByPet(long petId);

    IEnumerable<ScoreSaude> GetByClinica(long clinicaId);

    ScoreSaude? GetLatestByPet(long petId);
}