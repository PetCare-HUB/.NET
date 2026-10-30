using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCareHub.Application.Services.Interfaces;
using PetCareHub.Domain.Entities;

namespace PetCareHub.API.Controllers;

/// <summary>Gera e consulta resumos de leituras do pet armazenados no MongoDB para uso pelo RAG.</summary>
[ApiController]
[Route("api/contexto-ia/pets/{petId:long}/resumos")]
[Produces("application/json")]
[Authorize(Roles = "CLINICA")]
public sealed class ContextoIaController(IResumoPetService resumoService) : ControllerBase
{
    /// <summary>Agrega leituras Oracle do período e salva o documento preparado para recuperação pela IA.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ResumoPetIa), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Gerar(
        long petId,
        [FromQuery] DateTime inicio,
        [FromQuery] DateTime fim)
    {
        if (!TryGetClinicaId(out var clinicaId))
            return Forbid();

        if (!PeriodoValido(inicio, fim))
            return BadRequest(new { mensagem = "Informe um período válido de até 31 dias." });

        var resumo = await resumoService.GenerateAsync(clinicaId, petId, inicio, fim);
        if (resumo is null)
            return NotFound(new { mensagem = "Pet não encontrado ou não vinculado a esta clínica." });

        return CreatedAtAction(
            nameof(Consultar),
            new { petId, inicio, fim },
            resumo);
    }

    /// <summary>Busca no MongoDB o resumo já gerado para o pet e período.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ResumoPetIa), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Consultar(
        long petId,
        [FromQuery] DateTime inicio,
        [FromQuery] DateTime fim)
    {
        if (!TryGetClinicaId(out var clinicaId))
            return Forbid();

        if (!PeriodoValido(inicio, fim))
            return BadRequest(new { mensagem = "Informe um período válido de até 31 dias." });

        var resumo = await resumoService.GetAsync(clinicaId, petId, inicio, fim);
        return resumo is null ? NotFound() : Ok(resumo);
    }

    private bool TryGetClinicaId(out long clinicaId) =>
        long.TryParse(User.FindFirst("clinicaId")?.Value, out clinicaId) && clinicaId > 0;

    private static bool PeriodoValido(DateTime inicio, DateTime fim) =>
        inicio < fim && fim - inicio <= TimeSpan.FromDays(31);
}
