using Microsoft.EntityFrameworkCore;
using PetCareHub.Application.Repositories;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Repositories;

public sealed class ScoreSaudeRepository(PetCareHubContext context) 
    : Repository<ScoreSaude>(context), IScoreSaudeRepository
{
    private readonly PetCareHubContext _context = context;

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

    public ScoreSaude? GetLatestByPet(long petId) =>
        _context.ScoresSaude
            .AsNoTracking()
            .Where(s => s.PetId == petId)
            .OrderByDescending(s => s.DataCalculo)
            .FirstOrDefault();
}