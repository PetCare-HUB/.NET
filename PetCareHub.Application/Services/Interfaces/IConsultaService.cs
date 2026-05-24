using PetCareHub.Application.DTOs;

namespace PetCareHub.Application.Services.Interfaces;

public interface IConsultaService
{
    IReadOnlyList<ConsultaResponse> GetAll();

    IReadOnlyList<ConsultaResponse> GetFiltered(
        long? clinicaId,
        long? petId,
        string? tipoConsulta,
        bool? retornoRecomendado);

    ConsultaResponse? GetById(long id);

    IReadOnlyList<ConsultaResponse> GetByClinica(long clinicaId);

    IReadOnlyList<ConsultaResponse> GetByPet(long petId);

    ConsultaResponse Create(ConsultaRequest request);

    ConsultaResponse? Update(long id, ConsultaRequest request);

    bool Delete(long id);
}