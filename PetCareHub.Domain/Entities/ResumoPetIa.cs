namespace PetCareHub.Domain.Entities;

/// <summary>Resumo factual de leituras IoT de um pet para recuperação contextual pela IA.</summary>
public sealed class ResumoPetIa
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public long PetId { get; set; }
    public long ClinicaId { get; set; }
    public DateTime PeriodoInicio { get; set; }
    public DateTime PeriodoFim { get; set; }
    public DateTime GeradoEmUtc { get; set; }

    public int TotalLeiturasColeira { get; set; }
    public int LeiturasSedentarias { get; set; }
    public int LeiturasModeradas { get; set; }
    public int LeiturasAtivas { get; set; }
    public int TotalLeiturasComedouro { get; set; }
    public decimal? ConsumoTotalGramas { get; set; }
    public decimal? NivelMedioRacaoPct { get; set; }
    public int TotalLeiturasAmbiente { get; set; }
    public decimal? TemperaturaMedia { get; set; }
    public decimal? UmidadeMediaPct { get; set; }

    public string Conteudo { get; set; } = string.Empty;
}
