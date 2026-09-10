namespace PetCareHub.Domain.Entities;

public class ProtocoloPreventivo
{
    public long Id { get; set; }

    public string Especie { get; set; } = string.Empty;

    public string? Raca { get; set; }

    public string TipoEvento { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public int? IdadeMesesRecomendada { get; set; }

    public int? IntervaloDias { get; set; }

    public bool Ativo { get; set; } = true;

    public ICollection<EventoPreventivo> EventosPreventivos { get; set; } = new List<EventoPreventivo>();
}
