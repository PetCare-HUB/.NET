using PetCareHub.Application.DTOs;

namespace PetCareHub.Application.Services.Interfaces;

public interface IClinicaService
{
    IReadOnlyList<ClinicaResponse> GetAll();

    ClinicaResponse? GetById(long id);
}