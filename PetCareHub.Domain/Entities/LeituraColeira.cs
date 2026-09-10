namespace PetCareHub.Domain.Entities;

public class LeituraColeira
{
    public long Id { get; set; }

    public long PetId { get; set; }

    public string StatusAtividade { get; set; } = string.Empty;

    public int NivelBateria { get; set; }

    public DateTime TimestampLeitura { get; set; }

    public Pet? Pet { get; set; }
}
