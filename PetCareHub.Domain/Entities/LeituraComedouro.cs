namespace PetCareHub.Domain.Entities;

public class LeituraComedouro
{
    public long Id { get; set; }

    public long PetId { get; set; }

    public int NivelRacaoPct { get; set; }

    public decimal PesoConsumidoG { get; set; }

    public DateTime TimestampLeitura { get; set; }

    public Pet? Pet { get; set; }
}
