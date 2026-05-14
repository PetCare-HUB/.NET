namespace PetCareHub.Domain.Entities;

public class AlertaSaude
{
    public long Id { get; set; }

    public long PetId { get; set; }

    public long? LeituraId { get; set; }

    public string TipoAlerta { get; set; } = string.Empty;

    public string NivelAlerta { get; set; } = string.Empty;

    public string Mensagem { get; set; } = string.Empty;

    public decimal? ValorDetectado { get; set; }

    public decimal? LimiteReferencia { get; set; }

    public bool Resolvido { get; set; }

    public DateTime DataAlerta { get; set; }

    public DateTime? DataResolucao { get; set; }

    public Pet? Pet { get; set; }

    public LeituraSensor? LeituraSensor { get; set; }
}