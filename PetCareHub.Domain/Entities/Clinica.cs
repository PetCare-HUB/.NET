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

    /// <summary>
    /// Verifica se a clínica pode ser excluída. Quem tem os pets vinculados de verdade é o
    /// repositório (não dá pra saber isso só olhando essa instância em memória), então quem
    /// chama informa o fato (<paramref name="possuiPetsVinculados"/>) e a entidade decide a
    /// regra: uma clínica com pet vinculado não pode ser excluída.
    /// </summary>
    public void GarantirQuePodeSerExcluida(bool possuiPetsVinculados)
    {
        if (possuiPetsVinculados)
            throw new InvalidOperationException("Não é possível deletar uma clínica que possui pets vinculados.");
    }
}