using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCareHub.Infrastructure.Persistence;

namespace PetCareHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetsController : ControllerBase
{
    private readonly PetCareHubContext _context;

    public PetsController(PetCareHubContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] long? clinicaId,
        [FromQuery] string? especie,
        [FromQuery] bool? ativo)
    {
        var query = _context.Pets
            .AsNoTracking()
            .AsQueryable();

        if (clinicaId.HasValue)
        {
            query = query.Where(p => p.ClinicaId == clinicaId.Value);
        }

        if (!string.IsNullOrWhiteSpace(especie))
        {
            query = query.Where(p => p.Especie.ToUpper() == especie.ToUpper());
        }

        if (ativo.HasValue)
        {
            query = query.Where(p => p.Ativo == ativo.Value);
        }

        var pets = await query
            .OrderBy(p => p.Nome)
            .Select(p => new
            {
                p.Id,
                p.ResponsavelId,
                p.ClinicaId,
                p.Nome,
                p.Especie,
                p.Raca,
                p.DataNascimento,
                p.PesoKg,
                p.Sexo,
                p.CondicoesCronicas,
                p.DataCadastro,
                p.Ativo
            })
            .ToListAsync();

        return Ok(pets);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var pet = await _context.Pets
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new
            {
                p.Id,
                p.ResponsavelId,
                p.ClinicaId,
                p.Nome,
                p.Especie,
                p.Raca,
                p.DataNascimento,
                p.PesoKg,
                p.Sexo,
                p.CondicoesCronicas,
                p.DataCadastro,
                p.Ativo
            })
            .FirstOrDefaultAsync();

        if (pet is null)
        {
            return NotFound(new
            {
                mensagem = $"Pet com id {id} não encontrado."
            });
        }

        return Ok(pet);
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

        var pets = await _context.Pets
            .AsNoTracking()
            .Where(p => p.ClinicaId == clinicaId)
            .OrderBy(p => p.Nome)
            .Select(p => new
            {
                p.Id,
                p.Nome,
                p.Especie,
                p.Raca,
                p.PesoKg,
                p.Ativo
            })
            .ToListAsync();

        return Ok(pets);
    }
}