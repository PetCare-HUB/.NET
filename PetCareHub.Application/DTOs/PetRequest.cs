using System.ComponentModel.DataAnnotations;

namespace PetCareHub.Application.DTOs;

public record PetRequest(
    [property: Required(ErrorMessage = "O ID do responsável é obrigatório")]
    long ResponsavelId,

    [property: Required(ErrorMessage = "O ID da clínica é obrigatório")]
    long ClinicaId,

    [property: Required(ErrorMessage = "O nome do pet é obrigatório")]
    [property: StringLength(80, MinimumLength = 1, ErrorMessage = "Nome entre 1 e 80 caracteres")]
    string Nome,

    [property: Required(ErrorMessage = "A espécie é obrigatória")]
    [property: StringLength(20, ErrorMessage = "Espécie com até 20 caracteres")]
    [property: RegularExpression("^(CAO|GATO|OUTRO)$",
        ErrorMessage = "Espécie deve ser CAO, GATO ou OUTRO")]
    string Especie,

    [property: StringLength(80)]
    string? Raca,

    DateTime? DataNascimento,

    [property: Range(0.01, 999.99, ErrorMessage = "Peso deve ser entre 0,01 e 999,99 kg")]
    decimal PesoKg,

    [property: StringLength(1)]
    [property: RegularExpression("^[MF]$", ErrorMessage = "Sexo deve ser M ou F")]
    string? Sexo,

    [property: StringLength(300)]
    string? CondicoesCronicas,

    bool Ativo = true
);