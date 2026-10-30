using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCareHub.Application.DTOs;
using PetCareHub.Application.Services.Interfaces;
using PetCareHub.API.Hateoas;
using PetCareHub.Application.DTOs.Hateoas;
using PetCareHub.Application.DTOs.Paging;

namespace PetCareHub.API.Controllers;

/// <summary>Consulta scores de saúde (somente leitura — calculados pela API Java).</summary>
/// <param name="scoreSaudeService">Serviço de aplicação injetado.</param>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize(Roles = "CLINICA")]
public class ScoresSaudeController(IScoreSaudeService scoreSaudeService) : ControllerBase
{

    /// <summary>Lista scores de saúde, com filtros opcionais por pet, clínica, categoria e faixa.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ScoreSaudeResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll(
        [FromQuery] long? petId,
        [FromQuery] long? clinicaId,
        [FromQuery] string? categoria,
        [FromQuery] int? scoreMin,
        [FromQuery] int? scoreMax,
        [FromQuery] PageQuery consulta)
    {
        var temFiltro = petId.HasValue || clinicaId.HasValue
            || !string.IsNullOrWhiteSpace(categoria)
            || scoreMin.HasValue || scoreMax.HasValue;

        // Sem filtro a paginação acontece no banco; com filtro, sobre a lista
        // já restrita pelo query filter da clínica (ver PagedResponse.FromList).
        var pagina = temFiltro
            ? PagedResponse<ScoreSaudeResponse>.FromList(
                scoreSaudeService.GetFiltered(petId, clinicaId, categoria, scoreMin, scoreMax),
                consulta)
            : scoreSaudeService.GetAll(consulta);

        return Ok(this.ComLinksDePagina(pagina));
    }

    /// <summary>Busca um score pelo id.</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(Resource<ScoreSaudeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(long id)
    {
        var score = scoreSaudeService.GetById(id);
        if (score is null)
            return NotFound();

        return Ok(this.ComoRecurso(score, id, somenteLeitura: true));
    }

    /// <summary>Histórico de scores de saúde de um pet.</summary>
    [HttpGet("pet/{petId:long}")]
    [ProducesResponseType(typeof(IReadOnlyList<ScoreSaudeResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByPet(long petId)
    {
        var scores = scoreSaudeService.GetByPet(petId);
        return Ok(scores);
    }

    /// <summary>Score de saúde mais recente de um pet.</summary>
    [HttpGet("pet/{petId:long}/atual")]
    [ProducesResponseType(typeof(ScoreSaudeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetLatestByPet(long petId)
    {
        var score = scoreSaudeService.GetLatestByPet(petId);
        if (score is null)
            return NotFound();

        return Ok(score);
    }

    /// <summary>Scores de saúde de todos os pets de uma clínica.</summary>
    [HttpGet("clinica/{clinicaId:long}")]
    [ProducesResponseType(typeof(IReadOnlyList<ScoreSaudeResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByClinica(long clinicaId)
    {
        var scores = scoreSaudeService.GetByClinica(clinicaId);
        return Ok(scores);
    }
}