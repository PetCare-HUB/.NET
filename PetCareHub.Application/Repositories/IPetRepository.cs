using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.Repositories;

public interface IPetRepository : IRepository<Pet>
{
    IEnumerable<Pet> GetByClinica(long clinicaId);

    bool ExistsByNome(string nome);
}