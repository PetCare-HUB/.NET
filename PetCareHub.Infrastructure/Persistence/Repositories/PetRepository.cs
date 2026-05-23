using Microsoft.EntityFrameworkCore;
using PetCareHub.Application.Repositories;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Repositories;

public sealed class PetRepository(PetCareHubContext context) 
    : Repository<Pet>(context), IPetRepository
{
    private readonly PetCareHubContext _context = context;

    public IEnumerable<Pet> GetByClinica(long clinicaId) =>
        _context.Pets
            .AsNoTracking()
            .Where(p => p.ClinicaId == clinicaId)
            .Include(p => p.Clinica)
            .Include(p => p.Responsavel)
            .OrderBy(p => p.Nome)
            .ToList();

    public bool ExistsByNome(string nome) =>
        _context.Pets.Any(p => p.Nome.ToLower() == nome.ToLower());
}