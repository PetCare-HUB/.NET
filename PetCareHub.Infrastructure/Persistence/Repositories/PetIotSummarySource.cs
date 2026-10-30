using Microsoft.EntityFrameworkCore;
using PetCareHub.Application.DTOs;
using PetCareHub.Application.Repositories;

namespace PetCareHub.Infrastructure.Persistence.Repositories;

/// <summary>Agrega no Oracle as leituras de um pet no período selecionado.</summary>
public sealed class PetIotSummarySource(PetCareHubContext context) : IPetIotSummarySource
{
    public async Task<PetIotSummaryData?> GetPeriodSummaryAsync(long petId, DateTime inicio, DateTime fim)
    {
        var petExiste = await context.Pets
            .AsNoTracking()
            .AnyAsync(pet => pet.Id == petId);

        if (!petExiste)
            return null;

        var atividades = await context.LeiturasColeira
            .AsNoTracking()
            .Where(leitura => leitura.PetId == petId &&
                              leitura.TimestampLeitura >= inicio && leitura.TimestampLeitura <= fim)
            .Select(leitura => leitura.StatusAtividade)
            .ToListAsync();

        var alimentacoes = await context.LeiturasComedouro
            .AsNoTracking()
            .Where(leitura => leitura.PetId == petId &&
                              leitura.TimestampLeitura >= inicio && leitura.TimestampLeitura <= fim)
            .Select(leitura => new { leitura.PesoConsumidoG, leitura.NivelRacaoPct })
            .ToListAsync();

        var ambientes = await context.LeiturasAmbiente
            .AsNoTracking()
            .Where(leitura => leitura.PetId == petId &&
                              leitura.TimestampLeitura >= inicio && leitura.TimestampLeitura <= fim)
            .Select(leitura => new { leitura.TemperaturaAmbiente, leitura.UmidadePct })
            .ToListAsync();

        return new PetIotSummaryData(
            TotalLeiturasColeira: atividades.Count,
            LeiturasSedentarias: atividades.Count(status => string.Equals(status, "SEDENTARIO", StringComparison.OrdinalIgnoreCase)),
            LeiturasModeradas: atividades.Count(status => string.Equals(status, "MODERADO", StringComparison.OrdinalIgnoreCase)),
            LeiturasAtivas: atividades.Count(status => string.Equals(status, "ATIVO", StringComparison.OrdinalIgnoreCase)),
            TotalLeiturasComedouro: alimentacoes.Count,
            ConsumoTotalGramas: alimentacoes.Count == 0 ? null : alimentacoes.Sum(leitura => leitura.PesoConsumidoG),
            NivelMedioRacaoPct: alimentacoes.Count == 0 ? null : alimentacoes.Average(leitura => (decimal)leitura.NivelRacaoPct),
            TotalLeiturasAmbiente: ambientes.Count,
            TemperaturaMedia: ambientes.Count == 0 ? null : ambientes.Average(leitura => leitura.TemperaturaAmbiente),
            UmidadeMediaPct: ambientes.Count == 0 ? null : ambientes.Average(leitura => (decimal)leitura.UmidadePct));
    }
}
