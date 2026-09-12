using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCareHub.Application.DTOs;
using PetCareHub.Application.Services.Interfaces;

namespace PetCareHub.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize(Roles = "CLINICA")]
public class AlertasSaudeController(IAlertaSaudeService alertaService) : ControllerBase
{

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AlertaSaudeResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll(
        [FromQuery] long? petId,
        [FromQuery] string? nivelAlerta,
        [FromQuery] bool? resolvido)
    {
        var temFiltro = petId.HasValue
            || !string.IsNullOrWhiteSpace(nivelAlerta) || resolvido.HasValue;

        var alertas = temFiltro
            ? alertaService.GetFiltered(petId, nivelAlerta, resolvido)
            : alertaService.GetAll();

        return Ok(alertas);
    }

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
    [ProducesResponseType(typeof(AlertaSaudeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] AlertaSaudeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var alerta = alertaService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = alerta.Id }, alerta);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(AlertaSaudeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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