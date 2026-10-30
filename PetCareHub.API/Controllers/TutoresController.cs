using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCareHub.Application.DTOs;
using PetCareHub.Application.Services.Interfaces;
using PetCareHub.API.Hateoas;
using PetCareHub.Application.DTOs.Hateoas;
using PetCareHub.Application.DTOs.Paging;

namespace PetCareHub.API.Controllers;

/// <summary>Consulta tutores (somente leitura — cadastrados/ativados pela API Java).</summary>
/// <param name="tutorService">Serviço de aplicação injetado.</param>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize(Roles = "CLINICA")]
public class TutoresController(ITutorService tutorService) : ControllerBase
{

    /// <summary>Lista todos os tutores.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<TutorResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll([FromQuery] PageQuery consulta)
    {
        return Ok(this.ComLinksDePagina(tutorService.GetAll(consulta)));
    }

    /// <summary>Busca um tutor pelo id.</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(Resource<TutorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(long id)
    {
        var tutor = tutorService.GetById(id);
        if (tutor is null)
            return NotFound();

        return Ok(this.ComoRecurso(tutor, id, somenteLeitura: true));
    }

    /// <summary>Lista os tutores que têm pelo menos um pet vinculado à clínica.</summary>
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
