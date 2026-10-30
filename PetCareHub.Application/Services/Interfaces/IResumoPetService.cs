using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.Services.Interfaces;

public interface IResumoPetService
{
    Task<ResumoPetIa?> GenerateAsync(long clinicaId, long petId, DateTime inicio, DateTime fim);
    Task<ResumoPetIa?> GetAsync(long clinicaId, long petId, DateTime inicio, DateTime fim);
}
