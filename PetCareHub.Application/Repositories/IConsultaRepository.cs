using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.Repositories;

public interface IConsultaRepository : IRepository<Consulta>
{
    IEnumerable<Consulta> GetByClinica(long clinicaId);

    IEnumerable<Consulta> GetByPet(long petId);
    
    IEnumerable<Consulta> GetByPetAndClinica(long petId, long clinicaId);
}