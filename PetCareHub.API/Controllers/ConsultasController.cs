using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCareHub.Infrastructure.Persistence;

namespace PetCareHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultasController : ControllerBase
{
    private readonly PetCareHubContext _context;

    public ConsultasController(PetCareHubContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] long? clinicaId,
        [FromQuery] long? petId,
        [FromQuery] string? tipoConsulta,
        [FromQuery] bool? retornoRecomendado)
    {
        var query = _context.Consultas
            .AsNoTracking()
            .AsQueryable();

        if (clinicaId.HasValue)
        {
            query = query.Where(c => c.ClinicaId == clinicaId.Value);
        }

        if (petId.HasValue)
        {
            query = query.Where(c => c.PetId == petId.Value);
        }

        if (!string.IsNullOrWhiteSpace(tipoConsulta))
        {
            query = query.Where(c => c.TipoConsulta.ToUpper() == tipoConsulta.ToUpper());
        }

        if (retornoRecomendado.HasValue)
        {
            query = query.Where(c => c.RetornoRecomendado == retornoRecomendado.Value);
        }

        var consultas = await query
            .OrderByDescending(c => c.DataConsulta)
            .Select(c => new
            {
                c.Id,
                c.PetId,
                c.ClinicaId,
                c.DataConsulta,
                c.TipoConsulta,
                c.Descricao,
                c.Diagnostico,
                c.Valor,
                c.RetornoRecomendado,
                c.DataRetorno
            })
            .ToListAsync();

        return Ok(consultas);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var consulta = await _context.Consultas
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new
            {
                c.Id,
                c.PetId,
                c.ClinicaId,
                c.DataConsulta,
                c.TipoConsulta,
                c.Descricao,
                c.Diagnostico,
                c.Valor,
                c.RetornoRecomendado,
                c.DataRetorno
            })
            .FirstOrDefaultAsync();

        if (consulta is null)
        {
            return NotFound(new
            {
                mensagem = $"Consulta com id {id} não encontrada."
            });
        }

        return Ok(consulta);
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

        var consultas = await _context.Consultas
            .AsNoTracking()
            .Where(c => c.ClinicaId == clinicaId)
            .OrderByDescending(c => c.DataConsulta)
            .Select(c => new
            {
                c.Id,
                c.PetId,
                c.ClinicaId,
                c.DataConsulta,
                c.TipoConsulta,
                c.Descricao,
                c.Diagnostico,
                c.Valor,
                c.RetornoRecomendado,
                c.DataRetorno
            })
            .ToListAsync();

        return Ok(consultas);
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

        var consultas = await _context.Consultas
            .AsNoTracking()
            .Where(c => c.PetId == petId)
            .OrderByDescending(c => c.DataConsulta)
            .Select(c => new
            {
                c.Id,
                c.PetId,
                c.ClinicaId,
                c.DataConsulta,
                c.TipoConsulta,
                c.Descricao,
                c.Diagnostico,
                c.Valor,
                c.RetornoRecomendado,
                c.DataRetorno
            })
            .ToListAsync();

        return Ok(consultas);
    }
}