using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.Repositories;

public interface IAlertaSaudeRepository : IRepository<AlertaSaude>
{
    IEnumerable<AlertaSaude> GetByPet(long petId);

    IEnumerable<AlertaSaude> GetByClinica(long clinicaId);

    IEnumerable<AlertaSaude> GetUnresolved();

    IEnumerable<AlertaSaude> GetUnresolvedByPet(long petId);

    IEnumerable<AlertaSaude> GetFiltered(long? petId, string? nivelAlerta, bool? resolvido);

    void ResolveAlert(long alertaId);
}