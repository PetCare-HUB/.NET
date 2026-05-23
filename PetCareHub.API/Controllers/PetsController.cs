using Microsoft.AspNetCore.Mvc;
using PetCareHub.Application.DTOs;
using PetCareHub.Application.Services.Interfaces;

namespace PetCareHub.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class PetsController(IPetService petService) : ControllerBase
{

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PetResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var pets = petService.GetAll();
        return Ok(pets);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(PetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(long id)
    {
        var pet = petService.GetById(id);
        if (pet is null)
            return NotFound();

        return Ok(pet);
    }

    [HttpGet("clinica/{clinicaId:long}")]
    [ProducesResponseType(typeof(IReadOnlyList<PetResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByClinica(long clinicaId)
    {
        var pets = petService.GetByClinica(clinicaId);
        return Ok(pets);
    }
}