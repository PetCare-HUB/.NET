using Microsoft.AspNetCore.Mvc;
using PetCareHub.Application.DTOs;
using PetCareHub.Application.Services.Interfaces;

namespace PetCareHub.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class ResponsaveisController(IResponsavelService responsavelService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ResponsavelResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var responsaveis = responsavelService.GetAll();
        return Ok(responsaveis);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ResponsavelResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(long id)
    {
        var responsavel = responsavelService.GetById(id);
        if (responsavel is null)
            return NotFound();

        return Ok(responsavel);
    }
    
    [HttpGet("clinica/{clinicaId:long}")]
    [ProducesResponseType(typeof(IReadOnlyList<ResponsavelResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByClinica(long clinicaId)
    {
        var responsaveis = responsavelService.GetByClinica(clinicaId);
        return Ok(responsaveis);
    }
}