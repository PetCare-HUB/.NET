using Microsoft.EntityFrameworkCore;
using PetCareHub.Application.Repositories;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Repositories;

public sealed class ResponsavelRepository(PetCareHubContext context) 
    : Repository<Responsavel>(context), IResponsavelRepository
{
    private readonly PetCareHubContext _context = context;

    public IEnumerable<Responsavel> GetByClinica(long clinicaId) =>
        _context.Responsaveis
            .AsNoTracking()
            .Where(r => r.Pets.Any(p => p.ClinicaId == clinicaId))
            .Include(r => r.Pets)
            .OrderBy(r => r.Nome)
            .ToList();
}