using Microsoft.EntityFrameworkCore;
using PetCareHub.Application.Repositories;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Repositories;

public sealed class ScoreSaudeRepository(PetCareHubContext context)
    : Repository<ScoreSaude>(context), IScoreSaudeRepository
{

    public IEnumerable<ScoreSaude> GetByPet(long petId) =>
        _context.ScoresSaude
            .AsNoTracking()
            .Where(s => s.PetId == petId)
            .Include(s => s.Pet)
            .OrderByDescending(s => s.DataCalculo)
            .ToList();

    public IEnumerable<ScoreSaude> GetByClinica(long clinicaId) =>
        _context.ScoresSaude
            .AsNoTracking()
            .Where(s => s.Pet != null && s.Pet.ClinicaId == clinicaId)
            .Include(s => s.Pet)
            .OrderByDescending(s => s.DataCalculo)
            .ToList();

    public IEnumerable<ScoreSaude> GetFiltered(
        long? petId,
        long? clinicaId,
        string? categoria,
        int? scoreMin,
        int? scoreMax)
    {
        var query = _context.ScoresSaude
            .AsNoTracking()
            .Include(s => s.Pet)
            .AsQueryable();

        if (petId.HasValue)
            query = query.Where(s => s.PetId == petId.Value);

        if (clinicaId.HasValue)
            query = query.Where(s => s.Pet != null && s.Pet.ClinicaId == clinicaId.Value);

        if (!string.IsNullOrWhiteSpace(categoria))
            query = query.Where(s => s.Categoria.ToUpper() == categoria.ToUpper());

        if (scoreMin.HasValue)
            query = query.Where(s => s.ScoreTotal >= scoreMin.Value);

        if (scoreMax.HasValue)
            query = query.Where(s => s.ScoreTotal <= scoreMax.Value);

        return query.OrderByDescending(s => s.DataCalculo).ToList();
    }

    public ScoreSaude? GetLatestByPet(long petId) =>
        _context.ScoresSaude
            .AsNoTracking()
            .Where(s => s.PetId == petId)
            .OrderByDescending(s => s.DataCalculo)
            .FirstOrDefault();
}