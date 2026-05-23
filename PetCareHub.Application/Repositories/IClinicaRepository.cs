using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.Repositories;

public interface IClinicaRepository : IRepository<Clinica>
{
    Clinica? GetByCnpj(string cnpj);
    
    bool ExistsByCnpj(string cnpj);
    
    bool HasPets(long clinicaId);
}