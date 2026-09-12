namespace PetCareHub.Domain.Entities;

public class AlertaSaude
{
    public long Id { get; set; }

    public long PetId { get; set; }

    public string TipoAlerta { get; set; } = string.Empty;

    public string NivelAlerta { get; set; } = string.Empty;

    public string Mensagem { get; set; } = string.Empty;

    public decimal? ValorDetectado { get; set; }

    public decimal? LimiteReferencia { get; set; }

    public bool Resolvido { get; set; }

    public DateTime DataAlerta { get; set; }

    public DateTime? DataResolucao { get; set; }

    public Pet? Pet { get; set; }

    /// <summary>
    /// Marca o alerta como resolvido. Um alerta já resolvido não pode ser resolvido de
    /// novo — quem chama decide o que fazer com a exceção (a API converte em 400).
    /// </summary>
    public void Resolver()
    {
        if (Resolvido)
            throw new InvalidOperationException("Este alerta já está resolvido.");

        Resolvido = true;
        DataResolucao = DateTime.Now;
    }
}
