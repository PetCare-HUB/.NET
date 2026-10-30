namespace PetCareHub.Application.DTOs.Paging;

/// <summary>
/// Uma fatia de entidades vinda do repositório, com o total de registros que
/// existem no filtro. Fica propositalmente na fronteira Application/Infra:
/// o repositório devolve entidades, não DTO de resposta.
/// </summary>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, long TotalElements);
