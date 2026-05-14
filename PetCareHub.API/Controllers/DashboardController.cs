using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCareHub.Infrastructure.Persistence;

namespace PetCareHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly PetCareHubContext _context;

    public DashboardController(PetCareHubContext context)
    {
        _context = context;
    }

    [HttpGet("clinicas/{clinicaId:long}")]
    public async Task<IActionResult> GetResumoClinica(long clinicaId)
    {
        var clinica = await _context.Clinicas
            .AsNoTracking()
            .Where(c => c.Id == clinicaId)
            .Select(c => new
            {
                c.Id,
                c.Nome,
                c.Cnpj,
                c.Email,
                c.Telefone,
                c.Endereco,
                c.Ativo
            })
            .FirstOrDefaultAsync();

        if (clinica is null)
        {
            return NotFound(new
            {
                mensagem = $"Clínica com id {clinicaId} não encontrada."
            });
        }

        var totalPets = await _context.Pets
            .AsNoTracking()
            .CountAsync(p => p.ClinicaId == clinicaId);

        var petsAtivos = await _context.Pets
            .AsNoTracking()
            .CountAsync(p => p.ClinicaId == clinicaId && p.Ativo);

        var alertasAbertos = await _context.AlertasSaude
            .AsNoTracking()
            .CountAsync(a => a.Pet != null && a.Pet.ClinicaId == clinicaId && !a.Resolvido);

        var consultasRealizadas = await _context.Consultas
            .AsNoTracking()
            .CountAsync(c => c.ClinicaId == clinicaId);

        var eventosPendentes = await _context.EventosPreventivos
            .AsNoTracking()
            .CountAsync(e => e.Pet != null && e.Pet.ClinicaId == clinicaId && e.Status == "PENDENTE");

        var scoreMedio = await _context.ScoresSaude
            .AsNoTracking()
            .Where(s => s.Pet != null && s.Pet.ClinicaId == clinicaId)
            .Select(s => (double?)s.ScoreTotal)
            .AverageAsync();

        var petsEmRisco = await _context.ScoresSaude
            .AsNoTracking()
            .Where(s => s.Pet != null && s.Pet.ClinicaId == clinicaId && s.Categoria == "VERMELHO")
            .Select(s => s.PetId)
            .Distinct()
            .CountAsync();

        return Ok(new
        {
            clinica,
            indicadores = new
            {
                totalPets,
                petsAtivos,
                alertasAbertos,
                consultasRealizadas,
                eventosPendentes,
                scoreMedio = scoreMedio is null ? 0 : Math.Round(scoreMedio.Value, 2),
                petsEmRisco
            }
        });
    }

    [HttpGet("clinicas/{clinicaId:long}/pets-em-risco")]
    public async Task<IActionResult> GetPetsEmRisco(long clinicaId)
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

        var petsEmRisco = await _context.ScoresSaude
            .AsNoTracking()
            .Where(s => s.Pet != null && s.Pet.ClinicaId == clinicaId && s.Categoria == "VERMELHO")
            .OrderBy(s => s.ScoreTotal)
            .Select(s => new
            {
                s.PetId,
                NomePet = s.Pet != null ? s.Pet.Nome : null,
                Especie = s.Pet != null ? s.Pet.Especie : null,
                Raca = s.Pet != null ? s.Pet.Raca : null,
                s.ScoreTotal,
                s.Categoria,
                s.DataCalculo
            })
            .ToListAsync();

        return Ok(petsEmRisco);
    }

    [HttpGet("clinicas/{clinicaId:long}/alertas-abertos")]
    public async Task<IActionResult> GetAlertasAbertos(long clinicaId)
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

        var alertas = await _context.AlertasSaude
            .AsNoTracking()
            .Where(a => a.Pet != null && a.Pet.ClinicaId == clinicaId && !a.Resolvido)
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
                a.DataAlerta
            })
            .ToListAsync();

        return Ok(alertas);
    }

    [HttpGet("clinicas/{clinicaId:long}/consultas-recentes")]
    public async Task<IActionResult> GetConsultasRecentes(long clinicaId)
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
            .Take(10)
            .Select(c => new
            {
                c.Id,
                c.PetId,
                NomePet = c.Pet != null ? c.Pet.Nome : null,
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

    [HttpGet("clinicas/{clinicaId:long}/eventos-pendentes")]
    public async Task<IActionResult> GetEventosPendentes(long clinicaId)
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

        var eventos = await _context.EventosPreventivos
            .AsNoTracking()
            .Where(e => e.Pet != null && e.Pet.ClinicaId == clinicaId && e.Status == "PENDENTE")
            .OrderBy(e => e.DataPrevista)
            .Select(e => new
            {
                e.Id,
                e.PetId,
                NomePet = e.Pet != null ? e.Pet.Nome : null,
                e.TipoEvento,
                e.Descricao,
                e.DataPrevista,
                e.Status
            })
            .ToListAsync();

        return Ok(eventos);
    }
}