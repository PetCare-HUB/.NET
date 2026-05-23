using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.Repositories;

public interface IResponsavelRepository : IRepository<Responsavel>
{
    IEnumerable<Responsavel> GetByClinica(long clinicaId);
}