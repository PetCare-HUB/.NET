using Microsoft.EntityFrameworkCore;
using PetCareHub.Application.Repositories;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Repositories;

public sealed class ConsultaRepository(PetCareHubContext context)
    : Repository<Consulta>(context), IConsultaRepository
{

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

    public IEnumerable<Consulta> GetFiltered(
        long? clinicaId,
        long? petId,
        string? tipoConsulta,
        bool? retornoRecomendado)
    {
        var query = _context.Consultas
            .AsNoTracking()
            .Include(c => c.Pet)
            .Include(c => c.Clinica)
            .AsQueryable();

        if (clinicaId.HasValue)
            query = query.Where(c => c.ClinicaId == clinicaId.Value);

        if (petId.HasValue)
            query = query.Where(c => c.PetId == petId.Value);

        if (!string.IsNullOrWhiteSpace(tipoConsulta))
            query = query.Where(c => c.TipoConsulta.ToUpper() == tipoConsulta.ToUpper());

        if (retornoRecomendado.HasValue)
            query = query.Where(c => c.RetornoRecomendado == retornoRecomendado.Value);

        return query.OrderByDescending(c => c.DataConsulta).ToList();
    }
}