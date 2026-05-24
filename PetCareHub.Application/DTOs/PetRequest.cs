using System.ComponentModel.DataAnnotations;

namespace PetCareHub.Application.DTOs;

public record PetRequest(
    [Required(ErrorMessage = "O ID do responsável é obrigatório")]
    long ResponsavelId,

    [Required(ErrorMessage = "O ID da clínica é obrigatório")]
    long ClinicaId,

    [Required(ErrorMessage = "O nome do pet é obrigatório")]
    [StringLength(80, MinimumLength = 1, ErrorMessage = "Nome entre 1 e 80 caracteres")]
    string Nome,

    [Required(ErrorMessage = "A espécie é obrigatória")]
    [StringLength(20, ErrorMessage = "Espécie com até 20 caracteres")]
    [RegularExpression("^(CAO|GATO|OUTRO)$",
        ErrorMessage = "Espécie deve ser CAO, GATO ou OUTRO")]
    string Especie,

    [StringLength(80)]
    string? Raca,

    DateTime? DataNascimento,

    [Range(0.01, 999.99, ErrorMessage = "Peso deve ser entre 0,01 e 999,99 kg")]
    decimal PesoKg,

    [StringLength(1)]
    [RegularExpression("^[MF]$", ErrorMessage = "Sexo deve ser M ou F")]
    string? Sexo,

    [StringLength(300)]
    string? CondicoesCronicas,

    bool Ativo = true
);