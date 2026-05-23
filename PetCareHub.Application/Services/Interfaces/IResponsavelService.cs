using PetCareHub.Application.DTOs;

namespace PetCareHub.Application.Services.Interfaces;

public interface IResponsavelService
{
    IReadOnlyList<ResponsavelResponse> GetAll();

    ResponsavelResponse? GetById(long id);

    IReadOnlyList<ResponsavelResponse> GetByClinica(long clinicaId);
}