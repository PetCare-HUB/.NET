namespace PetCareHub.Domain.Entities;

public class Consulta
{
    public long Id { get; set; }

    public long PetId { get; set; }

    public long ClinicaId { get; set; }

    public DateTime DataConsulta { get; set; }

    public string TipoConsulta { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public string? Diagnostico { get; set; }

    public decimal? Valor { get; set; }

    public bool RetornoRecomendado { get; set; }

    public DateTime? DataRetorno { get; set; }

    public Pet? Pet { get; set; }

    public Clinica? Clinica { get; set; }
}