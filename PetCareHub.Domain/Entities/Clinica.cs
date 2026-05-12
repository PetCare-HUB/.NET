namespace PetCareHub.Domain.Entities;

public class Clinica
{
    public long Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Cnpj { get; set; } = string.Empty;

    public string Endereco { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public bool Ativa { get; set; } = true;

    public ICollection<Pet> Pets { get; set; } = new List<Pet>();

    public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
}