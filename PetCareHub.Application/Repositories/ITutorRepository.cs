using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.Repositories;

public interface ITutorRepository : IRepository<Tutor>
{
    IEnumerable<Tutor> GetByClinica(long clinicaId);
}
