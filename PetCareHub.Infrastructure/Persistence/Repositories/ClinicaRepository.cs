using Microsoft.EntityFrameworkCore;
using PetCareHub.Application.Repositories;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Repositories;

public sealed class ClinicaRepository(PetCareHubContext context) 
    : Repository<Clinica>(context), IClinicaRepository
{
    private readonly PetCareHubContext _context = context;

    public Clinica? GetByCnpj(string cnpj) =>
        _context.Clinicas
            .AsNoTracking()
            .FirstOrDefault(c => c.Cnpj == cnpj);

    public bool ExistsByCnpj(string cnpj) =>
        _context.Clinicas.Any(c => c.Cnpj == cnpj);

    public bool HasPets(long clinicaId) =>
        _context.Pets.Any(p => p.ClinicaId == clinicaId);
}