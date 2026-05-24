using Microsoft.EntityFrameworkCore;
using PetCareHub.Application.Repositories;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Repositories;

public sealed class ClinicaRepository(PetCareHubContext context)
    : Repository<Clinica>(context), IClinicaRepository
{
    // FIX bug Oracle EF Core 9.23 — não usar .Any()
    public bool ExistsByCnpj(string cnpj) =>
        _context.Clinicas.Count(c => c.Cnpj == cnpj) > 0;

    public Clinica? GetByCnpj(string cnpj) =>
        _context.Clinicas
            .AsNoTracking()
            .FirstOrDefault(c => c.Cnpj == cnpj);

    public bool HasPets(long clinicaId) =>
        _context.Pets.Count(p => p.ClinicaId == clinicaId) > 0;
}