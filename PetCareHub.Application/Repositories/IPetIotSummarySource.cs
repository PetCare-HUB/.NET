using PetCareHub.Application.DTOs;

namespace PetCareHub.Application.Repositories;

/// <summary>Consulta e agrega leituras IoT existentes no Oracle.</summary>
public interface IPetIotSummarySource
{
    Task<PetIotSummaryData?> GetPeriodSummaryAsync(long petId, DateTime inicio, DateTime fim);
}
