using Microsoft.EntityFrameworkCore;
using PetCareHub.Application.Repositories;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Repositories;

public sealed class ConsultaRepository(PetCareHubContext context) 
    : Repository<Consulta>(context), IConsultaRepository
{
    private readonly PetCareHubContext _context = context;

    public IEnumerable<Consulta> GetByClinica(long clinicaId) =>
        _context.Consultas
            .AsNoTracking()
            .Where(c => c.ClinicaId == clinicaId)
            .Include(c => c.Pet)
            .Include(c => c.Clinica)
            .OrderByDescending(c => c.DataConsulta)
            .ToList();

    public IEnumerable<Consulta> GetByPet(long petId) =>
        _context.Consultas
            .AsNoTracking()
            .Where(c => c.PetId == petId)
            .Include(c => c.Pet)
            .Include(c => c.Clinica)
            .OrderByDescending(c => c.DataConsulta)
            .ToList();

    public IEnumerable<Consulta> GetByPetAndClinica(long petId, long clinicaId) =>
        _context.Consultas
            .AsNoTracking()
            .Where(c => c.PetId == petId && c.ClinicaId == clinicaId)
            .Include(c => c.Pet)
            .Include(c => c.Clinica)
            .OrderByDescending(c => c.DataConsulta)
            .ToList();
}