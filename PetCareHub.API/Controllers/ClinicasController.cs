using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCareHub.Application.DTOs;
using PetCareHub.Application.Services.Interfaces;
using PetCareHub.API.Hateoas;
using PetCareHub.Application.DTOs.Hateoas;
using PetCareHub.Application.DTOs.Paging;

namespace PetCareHub.API.Controllers;

/// <summary>
/// Clínicas parceiras. A clínica lê e edita o próprio cadastro; credenciar
/// e descredenciar quem entra na rede é do admin da PetCare Hub.
/// </summary>
/// <remarks>
/// Uma clínica não cria outra clínica: quem entra na rede se cadastra sozinho
/// em <c>POST /auth/cadastro-clinica</c> na API Java e depois é aprovado por
/// um admin. O POST daqui é o pré-cadastro feito pelo próprio admin.
/// </remarks>
/// <param name="clinicaService">Serviço de aplicação injetado.</param>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize(Roles = "CLINICA,ADMIN")]
public class ClinicasController(IClinicaService clinicaService) : ControllerBase
{

    /// <summary>Lista todas as clínicas cadastradas.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ClinicaResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll([FromQuery] PageQuery consulta)
    {
        return Ok(this.ComLinksDePagina(clinicaService.GetAll(consulta)));
    }

    /// <summary>Busca uma clínica pelo id.</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(Resource<ClinicaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(long id)
    {
        var clinica = clinicaService.GetById(id);
        if (clinica is null)
            return NotFound();

        return Ok(this.ComoRecurso(clinica, id));
    }

    // Sem POST: esta API não cadastra clínica.
    //
    // O .NET não mapeia SENHA_HASH de propósito (autenticação é só do Java),
    // então uma clínica criada aqui nasceria sem senha — e clínica sem senha
    // pode ser reivindicada por quem souber o e-mail, o CNPJ (que é público)
    // e o código de credenciamento, que é um só para toda a rede.
    //
    // Quem entra na rede se cadastra em POST /auth/cadastro-clinica na API
    // Java, definindo a própria senha, e é aprovado por um admin em
    // PUT /admin/clinicas/{id}/ativar.
    
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
    
    /// <summary>
    /// Descredencia uma clínica. Falha com 400 se ela ainda tiver pets vinculados.
    /// </summary>
    /// <remarks>
    /// Restrito ao admin, e é a opção mais agressiva: apagar leva junto o
    /// vínculo dos pets. Para suspender o acesso preservando o histórico
    /// longitudinal, use <c>PUT /admin/clinicas/{id}/bloquear</c> na API Java.
    /// </remarks>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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