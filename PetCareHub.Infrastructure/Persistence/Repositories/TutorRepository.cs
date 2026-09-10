using Microsoft.EntityFrameworkCore;
using PetCareHub.Application.Repositories;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Repositories;

public sealed class TutorRepository(PetCareHubContext context)
    : Repository<Tutor>(context), ITutorRepository
{

    public IEnumerable<Tutor> GetByClinica(long clinicaId) =>
        _context.Tutores
            .AsNoTracking()
            .Where(t => t.Pets.Any(p => p.ClinicaId == clinicaId))
            .Include(t => t.Pets)
            .OrderBy(t => t.Nome)
            .ToList();
}
