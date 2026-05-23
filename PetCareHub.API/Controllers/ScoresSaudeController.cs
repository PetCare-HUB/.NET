using Microsoft.AspNetCore.Mvc;
using PetCareHub.Application.DTOs;
using PetCareHub.Application.Services.Interfaces;

namespace PetCareHub.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class ScoresSaudeController(IScoreSaudeService scoreSaudeService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ScoreSaudeResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var scores = scoreSaudeService.GetAll();
        return Ok(scores);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ScoreSaudeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(long id)
    {
        var score = scoreSaudeService.GetById(id);
        if (score is null)
            return NotFound();

        return Ok(score);
    }

    [HttpGet("pet/{petId:long}")]
    [ProducesResponseType(typeof(IReadOnlyList<ScoreSaudeResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByPet(long petId)
    {
        var scores = scoreSaudeService.GetByPet(petId);
        return Ok(scores);
    }

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

    [HttpGet("clinica/{clinicaId:long}")]
    [ProducesResponseType(typeof(IReadOnlyList<ScoreSaudeResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByClinica(long clinicaId)
    {
        var scores = scoreSaudeService.GetByClinica(clinicaId);
        return Ok(scores);
    }
}