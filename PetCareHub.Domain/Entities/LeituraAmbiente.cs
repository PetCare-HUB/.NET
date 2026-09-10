namespace PetCareHub.Domain.Entities;

public class LeituraAmbiente
{
    public long Id { get; set; }

    public long PetId { get; set; }

    public decimal TemperaturaAmbiente { get; set; }

    public int UmidadePct { get; set; }

    public int QualidadeArPpm { get; set; }

    public bool PetPresente { get; set; }

    public DateTime TimestampLeitura { get; set; }

    public Pet? Pet { get; set; }
}
