namespace PetCareHub.Domain.Entities;

public class LeituraSensor
{
    public long Id { get; set; }

    public long PetId { get; set; }

    public long DispositivoId { get; set; }

    public string TipoLeitura { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public string Unidade { get; set; } = string.Empty;

    public DateTime DataLeitura { get; set; }

    public string StatusLeitura { get; set; } = "NORMAL";

    public Pet? Pet { get; set; }

    public ICollection<AlertaSaude> AlertasSaude { get; set; } = new List<AlertaSaude>();
}