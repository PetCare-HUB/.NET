using System.Text.Json.Serialization;

namespace PetCareHub.Application.DTOs.Hateoas;

/// <summary>
/// Envelope HATEOAS de um recurso único: os dados mais o que dá para fazer
/// com ele a partir daqui.
/// </summary>
public sealed record Resource<T>(
    T Data,
    [property: JsonPropertyName("_links")] IReadOnlyList<LinkDto> Links);
