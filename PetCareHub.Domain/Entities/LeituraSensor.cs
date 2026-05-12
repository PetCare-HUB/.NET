namespace PetCareHub.Domain.Entities;

public class LeituraSensor
{
    public long Id { get; set; }

    public long PetId { get; set; }

    public string TipoSensor { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public string UnidadeMedida { get; set; } = string.Empty;

    public string StatusLeitura { get; set; } = "NORMAL";

    public DateTime DataLeitura { get; set; } = DateTime.UtcNow;

    public Pet? Pet { get; set; }

    public ICollection<AlertaSaude> AlertasSaude { get; set; } = new List<AlertaSaude>();
}