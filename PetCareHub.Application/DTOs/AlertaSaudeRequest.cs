using System.ComponentModel.DataAnnotations;

namespace PetCareHub.Application.DTOs;

public record AlertaSaudeRequest(
    [property: Required(ErrorMessage = "O ID do pet é obrigatório")]
    long PetId,

    [property: Required(ErrorMessage = "O tipo de alerta é obrigatório")]
    [property: StringLength(100, MinimumLength = 3)]
    string TipoAlerta,

    [property: Required(ErrorMessage = "O nível de alerta é obrigatório")]
    [property: StringLength(50)]
    string NivelAlerta, // CRITICO, AVISO, INFO

    [property: StringLength(500)]
    string? Mensagem,

    [property: Range(0, double.MaxValue)]
    decimal? ValorDetectado,

    [property: Range(0, double.MaxValue)]
    decimal? LimiteReferencia,

    long? LeituraId = null
);