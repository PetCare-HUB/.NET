using System.ComponentModel.DataAnnotations;

namespace PetCareHub.Application.DTOs;


public record ConsultaRequest(
    [property: Required(ErrorMessage = "O ID do pet é obrigatório")]
    long PetId,

    [property: Required(ErrorMessage = "O ID da clínica é obrigatório")]
    long ClinicaId,

    [property: Required(ErrorMessage = "A data da consulta é obrigatória")]
    DateTime DataConsulta,

    [property: StringLength(100, MinimumLength = 3, ErrorMessage = "Tipo de consulta entre 3 e 100 caracteres")]
    string? TipoConsulta,

    [property: StringLength(500)]
    string? Descricao,

    [property: StringLength(500)]
    string? Diagnostico,

    [property: Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
    decimal? Valor,

    bool RetornoRecomendado = false,

    DateTime? DataRetorno = null
);