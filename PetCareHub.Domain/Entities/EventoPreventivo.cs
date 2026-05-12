namespace PetCareHub.Domain.Entities;

public class EventoPreventivo
{
    public long Id { get; set; }

    public long PetId { get; set; }

    public string Tipo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public DateTime DataPrevista { get; set; }

    public DateTime? DataRealizacao { get; set; }

    public string Status { get; set; } = "PENDENTE";

    public Pet? Pet { get; set; }
}