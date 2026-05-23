using PetCareHub.Application.DTOs;

namespace PetCareHub.Application.Services.Interfaces;

public interface IClinicaService
{
    IReadOnlyList<ClinicaResponse> GetAll();

    ClinicaResponse? GetById(long id);

    ClinicaResponse Create(ClinicaRequest request);

    ClinicaResponse? Update(long id, ClinicaRequest request);
    
    bool Delete(long id);
}