using PetCareHub.Application.DTOs;
using PetCareHub.Application.DTOs.Paging;

namespace PetCareHub.Application.Services.Interfaces;

public interface IClinicaService
{
    PagedResponse<ClinicaResponse> GetAll(PageQuery consulta);

    ClinicaResponse? GetById(long id);

    // Sem Create: o cadastro de clínica é da API Java (POST /auth/cadastro-clinica,
    // onde a própria clínica define a senha). Criar por aqui geraria clínica sem
    // senha, que é um estado reivindicável por quem tiver o código de credenciamento.

    ClinicaResponse? Update(long id, ClinicaRequest request);
    
    bool Delete(long id);
}