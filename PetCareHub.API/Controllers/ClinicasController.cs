using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCareHub.Application.DTOs;
using PetCareHub.Application.Services.Interfaces;

namespace PetCareHub.API.Controllers;

/// <summary>Gerencia clínicas parceiras — CRUD completo.</summary>
/// <param name="clinicaService">Serviço de aplicação injetado.</param>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize(Roles = "CLINICA")]
public class ClinicasController(IClinicaService clinicaService) : ControllerBase
{

    /// <summary>Lista todas as clínicas cadastradas.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClinicaResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var clinicas = clinicaService.GetAll();
        return Ok(clinicas);
    }

    /// <summary>Busca uma clínica pelo id.</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ClinicaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(long id)
    {
        var clinica = clinicaService.GetById(id);
        if (clinica is null)
            return NotFound();

        return Ok(clinica);
    }

    /// <summary>Cria uma nova clínica. Falha com 400 se o CNPJ já estiver cadastrado.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClinicaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] ClinicaRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var clinica = clinicaService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = clinica.Id }, clinica);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
    
    /// <summary>Atualiza os dados de uma clínica. Falha com 400 se o novo CNPJ já pertencer a outra.</summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ClinicaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(long id, [FromBody] ClinicaRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var clinica = clinicaService.Update(id, request);
            if (clinica is null)
                return NotFound();

            return Ok(clinica);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
    
    /// <summary>Remove uma clínica. Falha com 400 se ela ainda tiver pets vinculados.</summary>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(long id)
    {
        try
        {
            var resultado = clinicaService.Delete(id);
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