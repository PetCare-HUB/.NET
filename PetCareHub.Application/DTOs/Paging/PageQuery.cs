using System.ComponentModel;

namespace PetCareHub.Application.DTOs.Paging;

/// <summary>
/// Parâmetros de paginação e ordenação das listagens.
/// </summary>
/// <remarks>
/// Os nomes (<c>page</c>, <c>size</c>, <c>sort</c>) e a base zero são os mesmos
/// que a API Java expõe via <c>Pageable</c> do Spring, para que o app mobile e o
/// dashboard não precisem de dois contratos de paginação diferentes.
/// </remarks>
public sealed record PageQuery
{
    /// <summary>Teto de itens por página — impede que um cliente peça a tabela inteira.</summary>
    public const int TamanhoMaximo = 100;

    private readonly int _page;
    private readonly int _size = 10;

    /// <summary>Página desejada, começando em 0.</summary>
    [DefaultValue(0)]
    public int Page
    {
        get => _page;
        init => _page = value < 0 ? 0 : value;
    }

    /// <summary>Itens por página (1 a 100).</summary>
    [DefaultValue(10)]
    public int Size
    {
        get => _size;
        init => _size = value switch
        {
            < 1 => 1,
            > TamanhoMaximo => TamanhoMaximo,
            _ => value
        };
    }

    /// <summary>Propriedade usada na ordenação. Desconhecida ou vazia cai para <c>Id</c>.</summary>
    [DefaultValue("Id")]
    public string Sort { get; init; } = "Id";

    /// <summary>Direção da ordenação: <c>asc</c> (padrão) ou <c>desc</c>.</summary>
    [DefaultValue("asc")]
    public string Dir { get; init; } = "asc";

    /// <summary>True quando <see cref="Dir"/> pede ordem decrescente.</summary>
    public bool Descending =>
        string.Equals(Dir, "desc", StringComparison.OrdinalIgnoreCase);

    /// <summary>Quantos registros pular para chegar na página pedida.</summary>
    public int Skip => Page * Size;
}
