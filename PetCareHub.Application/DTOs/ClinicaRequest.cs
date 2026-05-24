using System.ComponentModel.DataAnnotations;

namespace PetCareHub.Application.DTOs;

public record ClinicaRequest(
    [Required(ErrorMessage = "O nome da clínica é obrigatório")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "Nome entre 3 e 120 caracteres")]
    string Nome,

    [Required(ErrorMessage = "O CNPJ é obrigatório")]
    [StringLength(14, MinimumLength = 14, ErrorMessage = "CNPJ deve ter 14 caracteres")]
    string Cnpj,

    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(120)]
    string? Email,

    [Phone(ErrorMessage = "Telefone inválido")]
    [StringLength(20)]
    string? Telefone,

    [StringLength(200)]
    string? Endereco,

    bool Ativo = true
);