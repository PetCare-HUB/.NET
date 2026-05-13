namespace PetCareHub.Domain.Entities;

public class Clinica
{
    public long Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Cnpj { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Telefone { get; set; }

    public string? Endereco { get; set; }

    public bool Ativo { get; set; } = true;

    public ICollection<Pet> Pets { get; set; } = new List<Pet>();

    public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
}