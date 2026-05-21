using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCareHub.Domain.Entities;
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
            .OrderBy(c => c.Nome)
            .Select(c => new
            {
                c.Id,
                c.Nome,
                c.Cnpj,
                c.Email,
                c.Telefone,
                c.Endereco,
                c.Ativo
            })
            .ToListAsync();

        return Ok(clinicas);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var clinica = await _context.Clinicas
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new
            {
                c.Id,
                c.Nome,
                c.Cnpj,
                c.Email,
                c.Telefone,
                c.Endereco,
                c.Ativo
            })
            .FirstOrDefaultAsync();

        if (clinica is null)
        {
            return NotFound(new
            {
                mensagem = $"Clínica com id {id} não encontrada."
            });
        }

        return Ok(clinica);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ClinicaCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            return BadRequest(new { mensagem = "O nome da clínica é obrigatório." });
        }

        if (string.IsNullOrWhiteSpace(request.Cnpj))
        {
            return BadRequest(new { mensagem = "O CNPJ da clínica é obrigatório." });
        }

        var cnpjNormalizado = SomenteNumeros(request.Cnpj);

        if (cnpjNormalizado.Length != 14)
        {
            return BadRequest(new { mensagem = "O CNPJ deve conter exatamente 14 números." });
        }

        var cnpjJaExiste = await _context.Clinicas
            .AsNoTracking()
            .CountAsync(c => c.Cnpj == cnpjNormalizado);

        if (cnpjJaExiste > 0)
        {
            return BadRequest(new { mensagem = "Já existe uma clínica cadastrada com este CNPJ." });
        }

        var proximoId = await _context.Clinicas
            .Select(c => (long?)c.Id)
            .MaxAsync() ?? 0;

        var clinica = new Clinica
        {
            Id = proximoId + 1,
            Nome = request.Nome.Trim(),
            Cnpj = cnpjNormalizado,
            Email = request.Email?.Trim(),
            Telefone = request.Telefone?.Trim(),
            Endereco = request.Endereco?.Trim(),
            Ativo = true
        };

        _context.Clinicas.Add(clinica);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = clinica.Id },
            new
            {
                clinica.Id,
                clinica.Nome,
                clinica.Cnpj,
                clinica.Email,
                clinica.Telefone,
                clinica.Endereco,
                clinica.Ativo
            }
        );
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] ClinicaUpdateRequest request)
    {
        var clinica = await _context.Clinicas
            .FirstOrDefaultAsync(c => c.Id == id);

        if (clinica is null)
        {
            return NotFound(new
            {
                mensagem = $"Clínica com id {id} não encontrada."
            });
        }

        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            return BadRequest(new { mensagem = "O nome da clínica é obrigatório." });
        }

        if (string.IsNullOrWhiteSpace(request.Cnpj))
        {
            return BadRequest(new { mensagem = "O CNPJ da clínica é obrigatório." });
        }

        var cnpjNormalizado = SomenteNumeros(request.Cnpj);

        if (cnpjNormalizado.Length != 14)
        {
            return BadRequest(new { mensagem = "O CNPJ deve conter exatamente 14 números." });
        }

        var cnpjJaExisteEmOutraClinica = await _context.Clinicas
            .AsNoTracking()
            .CountAsync(c => c.Cnpj == cnpjNormalizado && c.Id != id);

        if (cnpjJaExisteEmOutraClinica > 0)
        {
            return BadRequest(new { mensagem = "Já existe outra clínica cadastrada com este CNPJ." });
        }

        clinica.Nome = request.Nome.Trim();
        clinica.Cnpj = cnpjNormalizado;
        clinica.Email = request.Email?.Trim();
        clinica.Telefone = request.Telefone?.Trim();
        clinica.Endereco = request.Endereco?.Trim();
        clinica.Ativo = request.Ativo;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            clinica.Id,
            clinica.Nome,
            clinica.Cnpj,
            clinica.Email,
            clinica.Telefone,
            clinica.Endereco,
            clinica.Ativo
        });
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var clinica = await _context.Clinicas
            .FirstOrDefaultAsync(c => c.Id == id);

        if (clinica is null)
        {
            return NotFound(new
            {
                mensagem = $"Clínica com id {id} não encontrada."
            });
        }

        var quantidadePets = await _context.Pets
            .AsNoTracking()
            .CountAsync(p => p.ClinicaId == id);

        if (quantidadePets > 0)
        {
            return BadRequest(new
            {
                mensagem = "Não é possível remover a clínica porque existem pets vinculados a ela."
            });
        }

        _context.Clinicas.Remove(clinica);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static string SomenteNumeros(string valor)
    {
        return new string(valor.Where(char.IsDigit).ToArray());
    }
}

public record ClinicaCreateRequest(
    string Nome,
    string Cnpj,
    string? Email,
    string? Telefone,
    string? Endereco
);

public record ClinicaUpdateRequest(
    string Nome,
    string Cnpj,
    string? Email,
    string? Telefone,
    string? Endereco,
    bool Ativo
);