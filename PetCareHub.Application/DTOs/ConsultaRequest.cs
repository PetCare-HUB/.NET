using System.ComponentModel.DataAnnotations;

namespace PetCareHub.Application.DTOs;

public record ConsultaRequest(
    [property: Required(ErrorMessage = "O ID do pet é obrigatório")]
    long PetId,

    [property: Required(ErrorMessage = "O ID da clínica é obrigatório")]
    long ClinicaId,

    [property: Required(ErrorMessage = "A data da consulta é obrigatória")]
    DateTime DataConsulta,

    [property: Required(ErrorMessage = "O tipo da consulta é obrigatório")]
    [property: StringLength(30, MinimumLength = 3, ErrorMessage = "Tipo entre 3 e 30 caracteres")]
    [property: RegularExpression("^(CHECKUP|VACINA|EMERGENCIA|RETORNO|EXAME)$",
        ErrorMessage = "Tipo deve ser CHECKUP, VACINA, EMERGENCIA, RETORNO ou EXAME")]
    string TipoConsulta,

    [property: StringLength(500)]
    string? Descricao,

    [property: StringLength(500)]
    string? Diagnostico,

    [property: Range(0.01, 99999999.99, ErrorMessage = "Valor deve ser maior que zero")]
    decimal? Valor,

    bool RetornoRecomendado = false,

    DateTime? DataRetorno = null
);