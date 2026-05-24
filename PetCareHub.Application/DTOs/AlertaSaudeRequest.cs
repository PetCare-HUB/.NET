using System.ComponentModel.DataAnnotations;

namespace PetCareHub.Application.DTOs;

public record AlertaSaudeRequest(
    [Required(ErrorMessage = "O ID do pet é obrigatório")]
    long PetId,

    [Required(ErrorMessage = "O tipo de alerta é obrigatório")]
    [StringLength(40, MinimumLength = 3, ErrorMessage = "Tipo entre 3 e 40 caracteres")]
    string TipoAlerta,

    [Required(ErrorMessage = "O nível de alerta é obrigatório")]
    [StringLength(20, ErrorMessage = "Nível com até 20 caracteres")]
    [RegularExpression("^(BAIXO|MEDIO|ALTO|CRITICO)$",
        ErrorMessage = "Nível deve ser BAIXO, MEDIO, ALTO ou CRITICO")]
    string NivelAlerta,

    [Required(ErrorMessage = "A mensagem é obrigatória")]
    [StringLength(300, MinimumLength = 1)]
    string Mensagem,

    [Range(0, double.MaxValue)]
    decimal? ValorDetectado,

    [Range(0, double.MaxValue)]
    decimal? LimiteReferencia,

    long? LeituraId = null
);