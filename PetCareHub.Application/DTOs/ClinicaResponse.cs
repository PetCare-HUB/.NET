using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.DTOs;

public record ClinicaResponse(
    long Id,
    string Nome,
    string Cnpj,
    string? Email,
    string? Telefone,
    string? Endereco,
    bool Ativo
)
{
    public static ClinicaResponse FromDomain(Clinica clinica) =>
        new(
            clinica.Id,
            clinica.Nome,
            clinica.Cnpj,
            clinica.Email,
            clinica.Telefone,
            clinica.Endereco,
            clinica.Ativo
        );
}