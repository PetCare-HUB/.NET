using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.DTOs;

public record PetResponse(
    long Id,
    long? TutorId,
    long? ClinicaId,
    string Nome,
    string Especie,
    string? Raca,
    DateTime? DataNascimento,
    decimal? PesoKg,
    string? Sexo,
    string? CondicoesCronicas,
    DateTime DataCadastro,
    bool Ativo
)
{
    public static PetResponse FromDomain(Pet pet) =>
        new(
            pet.Id,
            pet.TutorId,
            pet.ClinicaId,
            pet.Nome,
            pet.Especie,
            pet.Raca,
            pet.DataNascimento,
            pet.PesoKg,
            pet.Sexo,
            pet.CondicoesCronicas,
            pet.DataCadastro,
            pet.Ativo
        );
}
