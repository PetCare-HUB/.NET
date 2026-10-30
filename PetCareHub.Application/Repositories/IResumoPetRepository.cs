using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.Repositories;

/// <summary>Persiste resumos preparados para recuperação contextual da IA no MongoDB.</summary>
public interface IResumoPetRepository
{
    Task SaveAsync(ResumoPetIa resumo);
    Task<ResumoPetIa?> GetAsync(long clinicaId, long petId, DateTime inicio, DateTime fim);
}
