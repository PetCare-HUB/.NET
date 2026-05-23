using System.ComponentModel.DataAnnotations;

namespace PetCareHub.Application.DTOs;

public record ClinicaRequest(
    [property: Required(ErrorMessage = "O nome da clínica é obrigatório")]
    [property: StringLength(200, MinimumLength = 3, ErrorMessage = "Nome entre 3 e 200 caracteres")]
    string Nome,

    [property: Required(ErrorMessage = "O CNPJ é obrigatório")]
    [property: StringLength(14, MinimumLength = 14, ErrorMessage = "CNPJ deve ter 14 caracteres")]
    string Cnpj,

    [property: EmailAddress(ErrorMessage = "Email inválido")]
    string? Email,

    [property: Phone(ErrorMessage = "Telefone inválido")]
    string? Telefone,

    [property: StringLength(500)]
    string? Endereco,

    bool Ativo = true
);