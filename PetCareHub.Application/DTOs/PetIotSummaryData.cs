namespace PetCareHub.Application.DTOs;

/// <summary>Agregados lidos do Oracle antes de serem preparados para consulta pela IA.</summary>
public sealed record PetIotSummaryData(
    int TotalLeiturasColeira,
    int LeiturasSedentarias,
    int LeiturasModeradas,
    int LeiturasAtivas,
    int TotalLeiturasComedouro,
    decimal? ConsumoTotalGramas,
    decimal? NivelMedioRacaoPct,
    int TotalLeiturasAmbiente,
    decimal? TemperaturaMedia,
    decimal? UmidadeMediaPct);
