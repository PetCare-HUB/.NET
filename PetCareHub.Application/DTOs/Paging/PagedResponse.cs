using System.Text.Json.Serialization;
using PetCareHub.Application.DTOs.Hateoas;

namespace PetCareHub.Application.DTOs.Paging;

/// <summary>
/// Resposta paginada das listagens, com os links de navegação da coleção.
/// </summary>
/// <remarks>
/// O formato espelha o <c>PageResponse</c> da API Java de propósito: mesmos
/// nomes de campo e mesma base zero, para o app mobile tratar as duas APIs
/// com o mesmo código.
/// </remarks>
public sealed record PagedResponse<T>
{
    /// <summary>Itens desta página.</summary>
    public required IReadOnlyList<T> Content { get; init; }

    /// <summary>Número da página atual, começando em 0.</summary>
    public required int Page { get; init; }

    /// <summary>Itens por página solicitados.</summary>
    public required int Size { get; init; }

    /// <summary>Total de registros que atendem ao filtro, somando todas as páginas.</summary>
    public required long TotalElements { get; init; }

    /// <summary>Quantidade de páginas disponíveis.</summary>
    public int TotalPages => Size <= 0 ? 0 : (int)Math.Ceiling(TotalElements / (double)Size);

    /// <summary>True quando esta é a primeira página.</summary>
    public bool First => Page <= 0;

    /// <summary>True quando esta é a última página (ou não há resultado).</summary>
    public bool Last => TotalPages == 0 || Page >= TotalPages - 1;

    /// <summary>Links de navegação da coleção: self, first, prev, next, last.</summary>
    [JsonPropertyName("_links")]
    public IReadOnlyList<LinkDto> Links { get; init; } = [];

    /// <summary>Monta a resposta convertendo cada entidade da fatia para o DTO.</summary>
    public static PagedResponse<T> From<TEntity>(
        PagedResult<TEntity> resultado,
        PageQuery consulta,
        Func<TEntity, T> mapear) => new()
        {
            Content = resultado.Items.Select(mapear).ToList(),
            Page = consulta.Page,
            Size = consulta.Size,
            TotalElements = resultado.TotalElements
        };

    /// <summary>
    /// Pagina uma lista já materializada.
    /// </summary>
    /// <remarks>
    /// Usado pelos endpoints que combinam filtro com listagem (consultas,
    /// alertas e scores): o recorte acontece em memória, não no banco. É
    /// aceitável porque o query filter global do EF já limita o conjunto à
    /// clínica do token antes de chegar aqui. Se o volume por clínica crescer,
    /// o caminho é levar os filtros para dentro do <c>GetPaged</c>.
    /// </remarks>
    public static PagedResponse<T> FromList(IReadOnlyList<T> itens, PageQuery consulta) => new()
    {
        Content = itens.Skip(consulta.Skip).Take(consulta.Size).ToList(),
        Page = consulta.Page,
        Size = consulta.Size,
        TotalElements = itens.Count
    };

    /// <summary>Devolve a mesma página com os links preenchidos.</summary>
    public PagedResponse<T> ComLinks(IReadOnlyList<LinkDto> links) =>
        this with { Links = links };
}
