using System.ComponentModel.DataAnnotations;

namespace PetCareHub.Application.DTOs;

public record ConsultaRequest(
    [Required(ErrorMessage = "O ID do pet é obrigatório")]
    long PetId,

    [Required(ErrorMessage = "O ID da clínica é obrigatório")]
    long ClinicaId,

    [Required(ErrorMessage = "A data da consulta é obrigatória")]
    DateTime DataConsulta,

    [Required(ErrorMessage = "O tipo da consulta é obrigatório")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Tipo entre 3 e 30 caracteres")]
    [RegularExpression("^(CHECKUP|VACINA|EMERGENCIA|RETORNO|EXAME)$",
        ErrorMessage = "Tipo deve ser CHECKUP, VACINA, EMERGENCIA, RETORNO ou EXAME")]
    string TipoConsulta,

    [StringLength(500)]
    string? Descricao,

    [StringLength(500)]
    string? Diagnostico,

    [Range(0.01, 99999999.99, ErrorMessage = "Valor deve ser maior que zero")]
    decimal? Valor,

    bool RetornoRecomendado = false,

    DateTime? DataRetorno = null
);