using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.DTOs;

public record ResponsavelResponse(
    long Id,
    string Nome,
    string? Email,
    string? Telefone,
    string? Cpf
)
{
    public static ResponsavelResponse FromDomain(Responsavel responsavel) =>
        new(
            responsavel.Id,
            responsavel.Nome,
            responsavel.Email,
            responsavel.Telefone,
            responsavel.Cpf
        );
}