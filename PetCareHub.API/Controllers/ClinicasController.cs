using Microsoft.AspNetCore.Mvc;
using PetCareHub.Application.DTOs;
using PetCareHub.Application.Services.Interfaces;

namespace PetCareHub.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class ClinicasController(IClinicaService clinicaService) : ControllerBase
{

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClinicaResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var clinicas = clinicaService.GetAll();
        return Ok(clinicas);
    }

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

    [HttpPost]
    [ProducesResponseType(typeof(ClinicaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] ClinicaRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var clinica = clinicaService.Create(request);
            return Ok(clinica);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ClinicaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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