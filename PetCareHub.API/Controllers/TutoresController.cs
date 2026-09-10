using Microsoft.AspNetCore.Mvc;
using PetCareHub.Application.DTOs;
using PetCareHub.Application.Services.Interfaces;

namespace PetCareHub.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class TutoresController(ITutorService tutorService) : ControllerBase
{

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TutorResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var tutores = tutorService.GetAll();
        return Ok(tutores);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(TutorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(long id)
    {
        var tutor = tutorService.GetById(id);
        if (tutor is null)
            return NotFound();

        return Ok(tutor);
    }

    [HttpGet("clinica/{clinicaId:long}")]
    [ProducesResponseType(typeof(IReadOnlyList<TutorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetByClinica(long clinicaId)
    {
        try
        {
            var tutores = tutorService.GetByClinica(clinicaId);
            return Ok(tutores);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
