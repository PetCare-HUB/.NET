using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.Repositories;

public interface IAlertaSaudeRepository : IRepository<AlertaSaude>
{
    IEnumerable<AlertaSaude> GetByPet(long petId);

    IEnumerable<AlertaSaude> GetByClinica(long clinicaId);

    IEnumerable<AlertaSaude> GetUnresolved();

    IEnumerable<AlertaSaude> GetUnresolvedByPet(long petId);

    void ResolveAlert(long alertaId);
}