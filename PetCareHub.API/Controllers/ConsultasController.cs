using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCareHub.Application.DTOs;
using PetCareHub.Application.Services.Interfaces;

namespace PetCareHub.API.Controllers;

/// <summary>Gerencia consultas veterinárias — CRUD completo.</summary>
/// <param name="consultaService">Serviço de aplicação injetado.</param>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize(Roles = "CLINICA")]
public class ConsultasController(IConsultaService consultaService) : ControllerBase
{

    /// <summary>Lista consultas, com filtros opcionais por clínica, pet, tipo e retorno.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ConsultaResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll(
        [FromQuery] long? clinicaId,
        [FromQuery] long? petId,
        [FromQuery] string? tipoConsulta,
        [FromQuery] bool? retornoRecomendado)
    {
        var temFiltro = clinicaId.HasValue || petId.HasValue
            || !string.IsNullOrWhiteSpace(tipoConsulta) || retornoRecomendado.HasValue;

        var consultas = temFiltro
            ? consultaService.GetFiltered(clinicaId, petId, tipoConsulta, retornoRecomendado)
            : consultaService.GetAll();

        return Ok(consultas);
    }

    /// <summary>Busca uma consulta pelo id.</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ConsultaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(long id)
    {
        var consulta = consultaService.GetById(id);
        if (consulta is null)
            return NotFound();

        return Ok(consulta);
    }

    /// <summary>Lista as consultas de uma clínica.</summary>
    [HttpGet("clinica/{clinicaId:long}")]
    [ProducesResponseType(typeof(IReadOnlyList<ConsultaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetByClinica(long clinicaId)
    {
        try
        {
            var consultas = consultaService.GetByClinica(clinicaId);
            return Ok(consultas);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>Lista as consultas de um pet.</summary>
    [HttpGet("pet/{petId:long}")]
    [ProducesResponseType(typeof(IReadOnlyList<ConsultaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetByPet(long petId)
    {
        try
        {
            var consultas = consultaService.GetByPet(petId);
            return Ok(consultas);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>Cria uma nova consulta para um pet.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ConsultaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] ConsultaRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var consulta = consultaService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = consulta.Id }, consulta);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>Atualiza os dados de uma consulta existente.</summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ConsultaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(long id, [FromBody] ConsultaRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var consulta = consultaService.Update(id, request);
            if (consulta is null)
                return NotFound();

            return Ok(consulta);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>Remove uma consulta.</summary>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(long id)
    {
        var resultado = consultaService.Delete(id);
        if (!resultado)
            return NotFound();

        return NoContent();
    }
}