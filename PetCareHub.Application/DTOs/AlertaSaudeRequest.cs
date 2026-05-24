using System.ComponentModel.DataAnnotations;

namespace PetCareHub.Application.DTOs;

public record AlertaSaudeRequest(
    [property: Required(ErrorMessage = "O ID do pet é obrigatório")]
    long PetId,

    [property: Required(ErrorMessage = "O tipo de alerta é obrigatório")]
    [property: StringLength(40, MinimumLength = 3, ErrorMessage = "Tipo entre 3 e 40 caracteres")]
    string TipoAlerta,

    [property: Required(ErrorMessage = "O nível de alerta é obrigatório")]
    [property: StringLength(20, ErrorMessage = "Nível com até 20 caracteres")]
    [property: RegularExpression("^(BAIXO|MEDIO|ALTO|CRITICO)$",
        ErrorMessage = "Nível deve ser BAIXO, MEDIO, ALTO ou CRITICO")]
    string NivelAlerta,

    [property: Required(ErrorMessage = "A mensagem é obrigatória")]
    [property: StringLength(300, MinimumLength = 1)]
    string Mensagem,

    [property: Range(0, double.MaxValue)]
    decimal? ValorDetectado,

    [property: Range(0, double.MaxValue)]
    decimal? LimiteReferencia,

    long? LeituraId = null
);