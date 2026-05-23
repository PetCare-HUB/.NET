using Microsoft.AspNetCore.Mvc;
using PetCareHub.Application.DTOs;
using PetCareHub.Application.Services.Interfaces;

namespace PetCareHub.API.Controllers;

/// <summary>
/// Gerenciamento de alertas de saúde dos pets.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class AlertasSaudeController(IAlertaSaudeService alertaService) : ControllerBase
{
    /// <summary>
    /// Lista todos os alertas.
    /// </summary>
    /// <response code="200">Lista retornada com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AlertaSaudeResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var alertas = alertaService.GetAll();
        return Ok(alertas);
    }

    /// <summary>
    /// Obtém um alerta pelo ID.
    /// </summary>
    /// <param name="id">ID do alerta</param>
    /// <response code="200">Alerta encontrado.</response>
    /// <response code="404">Alerta não encontrado.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(AlertaSaudeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(long id)
    {
        var alerta = alertaService.GetById(id);
        if (alerta is null)
            return NotFound();

        return Ok(alerta);
    }

    [HttpGet("pet/{petId:long}")]
    [ProducesResponseType(typeof(IReadOnlyList<AlertaSaudeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetByPet(long petId)
    {
        try
        {
            var alertas = alertaService.GetByPet(petId);
            return Ok(alertas);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("clinica/{clinicaId:long}")]
    [ProducesResponseType(typeof(IReadOnlyList<AlertaSaudeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetByClinica(long clinicaId)
    {
        try
        {
            var alertas = alertaService.GetByClinica(clinicaId);
            return Ok(alertas);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(AlertaSaudeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] AlertaSaudeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var alerta = alertaService.Create(request);
            return Ok(alerta);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(AlertaSaudeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Update(long id, [FromBody] AlertaSaudeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var alerta = alertaService.Update(id, request);
            if (alerta is null)
                return NotFound();

            return Ok(alerta);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(long id)
    {
        var resultado = alertaService.Delete(id);
        if (!resultado)
            return NotFound();

        return NoContent();
    }
    
    [HttpPut("{id:long}/resolver")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Resolver(long id)
    {
        try
        {
            var resultado = alertaService.Resolve(id);
            if (!resultado)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}