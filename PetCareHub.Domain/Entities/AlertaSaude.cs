namespace PetCareHub.Domain.Entities;

public class AlertaSaude
{
    public long Id { get; set; }

    public long PetId { get; set; }

    public long? LeituraSensorId { get; set; }

    public string TipoAlerta { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public string Nivel { get; set; } = string.Empty;

    public bool Resolvido { get; set; }

    public DateTime DataAlerta { get; set; } = DateTime.UtcNow;

    public DateTime? DataResolucao { get; set; }

    public Pet? Pet { get; set; }

    public LeituraSensor? LeituraSensor { get; set; }
}