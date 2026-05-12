namespace PetCareHub.Domain.Entities;

public class ScoreSaude
{
    public long Id { get; set; }

    public long PetId { get; set; }

    public int ScoreTotal { get; set; }

    public string Categoria { get; set; } = string.Empty;

    public DateTime DataCalculo { get; set; } = DateTime.UtcNow;

    public string? Observacao { get; set; }

    public Pet? Pet { get; set; }
}