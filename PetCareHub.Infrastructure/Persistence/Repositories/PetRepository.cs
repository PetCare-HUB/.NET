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

    public IEnumerable<Pet> GetFiltered(long? clinicaId, string? especie, bool? ativo)
    {
        var query = _context.Pets
            .AsNoTracking()
            .Include(p => p.Clinica)
            .Include(p => p.Responsavel)
            .AsQueryable();

        if (clinicaId.HasValue)
            query = query.Where(p => p.ClinicaId == clinicaId.Value);

        if (!string.IsNullOrWhiteSpace(especie))
            query = query.Where(p => p.Especie.ToUpper() == especie.ToUpper());

        if (ativo.HasValue)
            query = query.Where(p => p.Ativo == ativo.Value);

        return query.OrderBy(p => p.Nome).ToList();
    }

    public Pet? GetByIdWithRelations(long id) =>
        _context.Pets
            .AsNoTracking()
            .Include(p => p.Clinica)
            .Include(p => p.Responsavel)
            .FirstOrDefault(p => p.Id == id);

    public bool ExistsByNome(string nome) =>
        _context.Pets.Any(p => p.Nome.ToLower() == nome.ToLower());
}