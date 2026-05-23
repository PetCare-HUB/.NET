namespace PetCareHub.Domain.Entities;

public class Responsavel
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Cpf { get; set; }
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }
    
    public ICollection<Pet> Pets { get; set; } = new List<Pet>();
}