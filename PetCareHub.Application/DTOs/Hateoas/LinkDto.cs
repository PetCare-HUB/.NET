namespace PetCareHub.Application.DTOs.Hateoas;

/// <summary>
/// Um link HATEOAS: para onde ir, com que verbo, e que relação tem com o
/// recurso atual (<c>self</c>, <c>next</c>, <c>update</c>, <c>delete</c>...).
/// </summary>
/// <param name="Rel">Relação do link com o recurso atual.</param>
/// <param name="Href">URL absoluta do recurso.</param>
/// <param name="Method">Verbo HTTP a ser usado.</param>
public sealed record LinkDto(string Rel, string Href, string Method);
