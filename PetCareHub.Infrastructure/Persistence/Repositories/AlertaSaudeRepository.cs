using Microsoft.EntityFrameworkCore;
using PetCareHub.Application.Repositories;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Repositories;

public sealed class AlertaSaudeRepository(PetCareHubContext context)
    : Repository<AlertaSaude>(context), IAlertaSaudeRepository
{
    private readonly PetCareHubContext _context = context;

    public IEnumerable<AlertaSaude> GetByPet(long petId) =>
        _context.AlertasSaude
            .AsNoTracking()
            .Where(a => a.PetId == petId)
            .Include(a => a.Pet)
            .OrderByDescending(a => a.DataAlerta)
            .ToList();

    public IEnumerable<AlertaSaude> GetByClinica(long clinicaId) =>
        _context.AlertasSaude
            .AsNoTracking()
            .Where(a => a.Pet != null && a.Pet.ClinicaId == clinicaId)
            .Include(a => a.Pet)
            .OrderByDescending(a => a.DataAlerta)
            .ToList();

    public IEnumerable<AlertaSaude> GetUnresolved() =>
        _context.AlertasSaude
            .AsNoTracking()
            .Where(a => !a.Resolvido)
            .Include(a => a.Pet)
            .OrderByDescending(a => a.DataAlerta)
            .ToList();

    public IEnumerable<AlertaSaude> GetUnresolvedByPet(long petId) =>
        _context.AlertasSaude
            .AsNoTracking()
            .Where(a => a.PetId == petId && !a.Resolvido)
            .Include(a => a.Pet)
            .OrderByDescending(a => a.DataAlerta)
            .ToList();

    public IEnumerable<AlertaSaude> GetFiltered(long? petId, string? nivelAlerta, bool? resolvido)
    {
        var query = _context.AlertasSaude
            .AsNoTracking()
            .Include(a => a.Pet)
            .AsQueryable();

        if (petId.HasValue)
            query = query.Where(a => a.PetId == petId.Value);

        if (!string.IsNullOrWhiteSpace(nivelAlerta))
            query = query.Where(a => a.NivelAlerta.ToUpper() == nivelAlerta.ToUpper());

        if (resolvido.HasValue)
            query = query.Where(a => a.Resolvido == resolvido.Value);

        return query.OrderByDescending(a => a.DataAlerta).ToList();
    }

    public void ResolveAlert(long alertaId)
    {
        var alerta = _context.AlertasSaude.Find(alertaId);
        if (alerta is not null)
        {
            alerta.Resolvido = true;
            alerta.DataResolucao = DateTime.Now;
            _context.SaveChanges();
        }
    }
}