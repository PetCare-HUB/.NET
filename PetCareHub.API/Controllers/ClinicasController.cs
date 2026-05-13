using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCareHub.Infrastructure.Persistence;

namespace PetCareHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClinicasController : ControllerBase
{
    private readonly PetCareHubContext _context;

    public ClinicasController(PetCareHubContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clinicas = await _context.Clinicas
            .AsNoTracking()
            .ToListAsync();

        return Ok(clinicas);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var clinica = await _context.Clinicas
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (clinica is null)
        {
            return NotFound(new
            {
                mensagem = $"Clínica com id {id} não encontrada."
            });
        }

        return Ok(clinica);
    }
}