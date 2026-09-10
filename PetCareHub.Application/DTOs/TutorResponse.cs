using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.DTOs;

public record TutorResponse(
    long Id,
    string Nome,
    string? Email,
    string? Telefone,
    string? Cpf,
    string StatusAcesso
)
{
    public static TutorResponse FromDomain(Tutor tutor) =>
        new(
            tutor.Id,
            tutor.Nome,
            tutor.Email,
            tutor.Telefone,
            tutor.Cpf,
            tutor.StatusAcesso
        );
}
