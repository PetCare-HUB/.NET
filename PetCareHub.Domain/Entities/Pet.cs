namespace PetCareHub.Domain.Entities;

public class Pet
{
    public long Id { get; set; }

    public long TutorId { get; set; }

    public long ClinicaId { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Especie { get; set; } = string.Empty;

    public string? Raca { get; set; }

    public DateTime? DataNascimento { get; set; }

    public decimal PesoKg { get; set; }

    public string? Sexo { get; set; }

    public string? CondicoesCronicas { get; set; }

    public DateTime DataCadastro { get; set; }

    public bool Ativo { get; set; }

    public Clinica? Clinica { get; set; }

    public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();

    public ICollection<EventoPreventivo> EventosPreventivos { get; set; } = new List<EventoPreventivo>();

    public ICollection<LeituraSensor> LeiturasSensor { get; set; } = new List<LeituraSensor>();

    public ICollection<AlertaSaude> AlertasSaude { get; set; } = new List<AlertaSaude>();

    public ICollection<ScoreSaude> ScoresSaude { get; set; } = new List<ScoreSaude>();
}