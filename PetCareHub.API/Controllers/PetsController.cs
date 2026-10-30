using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCareHub.Application.DTOs;
using PetCareHub.Application.Services.Interfaces;
using PetCareHub.API.Hateoas;
using PetCareHub.Application.DTOs.Hateoas;
using PetCareHub.Application.DTOs.Paging;

namespace PetCareHub.API.Controllers;

/// <summary>Consulta pets vinculados à clínica (somente leitura — o cadastro é da API Java).</summary>
/// <param name="petService">Serviço de aplicação injetado.</param>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize(Roles = "CLINICA")]
public class PetsController(IPetService petService) : ControllerBase
{

    /// <summary>Lista todos os pets.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<PetResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll([FromQuery] PageQuery consulta)
    {
        return Ok(this.ComLinksDePagina(petService.GetAll(consulta)));
    }

    /// <summary>Busca um pet pelo id.</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(Resource<PetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(long id)
    {
        var pet = petService.GetById(id);
        if (pet is null)
            return NotFound();

        return Ok(this.ComoRecurso(pet, id, somenteLeitura: true));
    }

    /// <summary>Lista os pets vinculados a uma clínica.</summary>
    [HttpGet("clinica/{clinicaId:long}")]
    [ProducesResponseType(typeof(IReadOnlyList<PetResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByClinica(long clinicaId)
    {
        var pets = petService.GetByClinica(clinicaId);
        return Ok(pets);
    }
}