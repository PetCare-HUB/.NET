using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCareHub.Infrastructure.Persistence;

namespace PetCareHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertasSaudeController : ControllerBase
{
    private readonly PetCareHubContext _context;

    public AlertasSaudeController(PetCareHubContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] long? petId,
        [FromQuery] string? nivelAlerta,
        [FromQuery] string? tipoAlerta,
        [FromQuery] bool? resolvido)
    {
        var query = _context.AlertasSaude
            .AsNoTracking()
            .AsQueryable();

        if (petId.HasValue)
        {
            query = query.Where(a => a.PetId == petId.Value);
        }

        if (!string.IsNullOrWhiteSpace(nivelAlerta))
        {
            query = query.Where(a => a.NivelAlerta.ToUpper() == nivelAlerta.ToUpper());
        }

        if (!string.IsNullOrWhiteSpace(tipoAlerta))
        {
            query = query.Where(a => a.TipoAlerta.ToUpper() == tipoAlerta.ToUpper());
        }

        if (resolvido.HasValue)
        {
            query = query.Where(a => a.Resolvido == resolvido.Value);
        }

        var alertas = await query
            .OrderByDescending(a => a.DataAlerta)
            .Select(a => new
            {
                a.Id,
                a.PetId,
                a.LeituraId,
                a.TipoAlerta,
                a.NivelAlerta,
                a.Mensagem,
                a.ValorDetectado,
                a.LimiteReferencia,
                a.Resolvido,
                a.DataAlerta,
                a.DataResolucao
            })
            .ToListAsync();

        return Ok(alertas);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var alerta = await _context.AlertasSaude
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new
            {
                a.Id,
                a.PetId,
                a.LeituraId,
                a.TipoAlerta,
                a.NivelAlerta,
                a.Mensagem,
                a.ValorDetectado,
                a.LimiteReferencia,
                a.Resolvido,
                a.DataAlerta,
                a.DataResolucao
            })
            .FirstOrDefaultAsync();

        if (alerta is null)
        {
            return NotFound(new
            {
                mensagem = $"Alerta de saúde com id {id} não encontrado."
            });
        }

        return Ok(alerta);
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

        var alertas = await _context.AlertasSaude
            .AsNoTracking()
            .Where(a => a.PetId == petId)
            .OrderByDescending(a => a.DataAlerta)
            .Select(a => new
            {
                a.Id,
                a.PetId,
                a.TipoAlerta,
                a.NivelAlerta,
                a.Mensagem,
                a.ValorDetectado,
                a.LimiteReferencia,
                a.Resolvido,
                a.DataAlerta,
                a.DataResolucao
            })
            .ToListAsync();

        return Ok(alertas);
    }

    [HttpGet("clinica/{clinicaId:long}")]
    public async Task<IActionResult> GetByClinica(long clinicaId, [FromQuery] bool? resolvido)
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

        var query = _context.AlertasSaude
            .AsNoTracking()
            .Where(a => a.Pet != null && a.Pet.ClinicaId == clinicaId);

        if (resolvido.HasValue)
        {
            query = query.Where(a => a.Resolvido == resolvido.Value);
        }

        var alertas = await query
            .OrderByDescending(a => a.DataAlerta)
            .Select(a => new
            {
                a.Id,
                a.PetId,
                NomePet = a.Pet != null ? a.Pet.Nome : null,
                a.TipoAlerta,
                a.NivelAlerta,
                a.Mensagem,
                a.ValorDetectado,
                a.LimiteReferencia,
                a.Resolvido,
                a.DataAlerta,
                a.DataResolucao
            })
            .ToListAsync();

        return Ok(alertas);
    }

    [HttpPut("{id:long}/resolver")]
    public async Task<IActionResult> Resolver(long id)
    {
        var alerta = await _context.AlertasSaude
            .FirstOrDefaultAsync(a => a.Id == id);

        if (alerta is null)
        {
            return NotFound(new
            {
                mensagem = $"Alerta de saúde com id {id} não encontrado."
            });
        }

        if (alerta.Resolvido)
        {
            return BadRequest(new
            {
                mensagem = "Este alerta já está resolvido."
            });
        }

        alerta.Resolvido = true;
        alerta.DataResolucao = DateTime.Now;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}