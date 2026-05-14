namespace PetCareHub.Domain.Entities;

public class ScoreSaude
{
    public long Id { get; set; }

    public long PetId { get; set; }

    public int ScoreTotal { get; set; }

    public int ScoreAtividade { get; set; }

    public int ScoreAlimentacao { get; set; }

    public int ScoreAmbiente { get; set; }

    public int ScoreConsulta { get; set; }

    public int ScorePreventivo { get; set; }

    public string Categoria { get; set; } = string.Empty;

    public DateTime DataCalculo { get; set; }

    public Pet? Pet { get; set; }
}