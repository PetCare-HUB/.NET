using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.Repositories;

public interface IPetRepository : IRepository<Pet>
{
    IEnumerable<Pet> GetByClinica(long clinicaId);

    IEnumerable<Pet> GetFiltered(long? clinicaId, string? especie, bool? ativo);

    Pet? GetByIdWithRelations(long id);

    bool ExistsByNome(string nome);
}