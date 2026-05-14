using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCareHub.Infrastructure.Persistence;

namespace PetCareHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScoresSaudeController : ControllerBase
{
    private readonly PetCareHubContext _context;

    public ScoresSaudeController(PetCareHubContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] long? petId,
        [FromQuery] long? clinicaId,
        [FromQuery] string? categoria,
        [FromQuery] int? scoreMin,
        [FromQuery] int? scoreMax)
    {
        var query = _context.ScoresSaude
            .AsNoTracking()
            .AsQueryable();

        if (petId.HasValue)
        {
            query = query.Where(s => s.PetId == petId.Value);
        }

        if (clinicaId.HasValue)
        {
            query = query.Where(s => s.Pet != null && s.Pet.ClinicaId == clinicaId.Value);
        }

        if (!string.IsNullOrWhiteSpace(categoria))
        {
            query = query.Where(s => s.Categoria.ToUpper() == categoria.ToUpper());
        }

        if (scoreMin.HasValue)
        {
            query = query.Where(s => s.ScoreTotal >= scoreMin.Value);
        }

        if (scoreMax.HasValue)
        {
            query = query.Where(s => s.ScoreTotal <= scoreMax.Value);
        }

        var scores = await query
            .OrderByDescending(s => s.DataCalculo)
            .Select(s => new
            {
                s.Id,
                s.PetId,
                NomePet = s.Pet != null ? s.Pet.Nome : null,
                s.ScoreTotal,
                s.ScoreAtividade,
                s.ScoreAlimentacao,
                s.ScoreAmbiente,
                s.ScoreConsulta,
                s.ScorePreventivo,
                s.Categoria,
                s.DataCalculo
            })
            .ToListAsync();

        return Ok(scores);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var score = await _context.ScoresSaude
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new
            {
                s.Id,
                s.PetId,
                NomePet = s.Pet != null ? s.Pet.Nome : null,
                s.ScoreTotal,
                s.ScoreAtividade,
                s.ScoreAlimentacao,
                s.ScoreAmbiente,
                s.ScoreConsulta,
                s.ScorePreventivo,
                s.Categoria,
                s.DataCalculo
            })
            .FirstOrDefaultAsync();

        if (score is null)
        {
            return NotFound(new
            {
                mensagem = $"Score de saúde com id {id} não encontrado."
            });
        }

        return Ok(score);
    }

    [HttpGet("pet/{petId:long}")]
    public async Task<IActionResult> GetByPet(long petId)
    {
        var quantidadePets = await _context.Pets
            .AsNoTracking()
            .CountAsync(p => p.Id == petId);

        if (quantidadePets == 0)
        {
            return NotFound(new
            {
                mensagem = $"Pet com id {petId} não encontrado."
            });
        }

        var scores = await _context.ScoresSaude
            .AsNoTracking()
            .Where(s => s.PetId == petId)
            .OrderByDescending(s => s.DataCalculo)
            .Select(s => new
            {
                s.Id,
                s.PetId,
                s.ScoreTotal,
                s.ScoreAtividade,
                s.ScoreAlimentacao,
                s.ScoreAmbiente,
                s.ScoreConsulta,
                s.ScorePreventivo,
                s.Categoria,
                s.DataCalculo
            })
            .ToListAsync();

        return Ok(scores);
    }

    [HttpGet("pet/{petId:long}/atual")]
    public async Task<IActionResult> GetAtualByPet(long petId)
    {
        var quantidadePets = await _context.Pets
            .AsNoTracking()
            .CountAsync(p => p.Id == petId);

        if (quantidadePets == 0)
        {
            return NotFound(new
            {
                mensagem = $"Pet com id {petId} não encontrado."
            });
        }

        var scoreAtual = await _context.ScoresSaude
            .AsNoTracking()
            .Where(s => s.PetId == petId)
            .OrderByDescending(s => s.DataCalculo)
            .Select(s => new
            {
                s.Id,
                s.PetId,
                s.ScoreTotal,
                s.ScoreAtividade,
                s.ScoreAlimentacao,
                s.ScoreAmbiente,
                s.ScoreConsulta,
                s.ScorePreventivo,
                s.Categoria,
                s.DataCalculo
            })
            .FirstOrDefaultAsync();

        if (scoreAtual is null)
        {
            return NotFound(new
            {
                mensagem = $"Nenhum score encontrado para o pet com id {petId}."
            });
        }

        return Ok(scoreAtual);
    }

    [HttpGet("clinica/{clinicaId:long}")]
    public async Task<IActionResult> GetByClinica(long clinicaId)
    {
        var quantidadeClinicas = await _context.Clinicas
            .AsNoTracking()
            .CountAsync(c => c.Id == clinicaId);

        if (quantidadeClinicas == 0)
        {
            return NotFound(new
            {
                mensagem = $"Clínica com id {clinicaId} não encontrada."
            });
        }

        var scores = await _context.ScoresSaude
            .AsNoTracking()
            .Where(s => s.Pet != null && s.Pet.ClinicaId == clinicaId)
            .OrderByDescending(s => s.DataCalculo)
            .Select(s => new
            {
                s.Id,
                s.PetId,
                NomePet = s.Pet != null ? s.Pet.Nome : null,
                s.ScoreTotal,
                s.ScoreAtividade,
                s.ScoreAlimentacao,
                s.ScoreAmbiente,
                s.ScoreConsulta,
                s.ScorePreventivo,
                s.Categoria,
                s.DataCalculo
            })
            .ToListAsync();

        return Ok(scores);
    }
}