using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.DTOs;

public record AlertaSaudeResponse(
    long Id,
    long PetId,
    string NomePet,
    string TipoAlerta,
    string NivelAlerta,
    string? Mensagem,
    decimal? ValorDetectado,
    decimal? LimiteReferencia,
    bool Resolvido,
    DateTime DataAlerta,
    DateTime? DataResolucao
)
{

    public static AlertaSaudeResponse FromDomain(AlertaSaude alerta) =>
        new(
            alerta.Id,
            alerta.PetId,
            alerta.Pet?.Nome ?? string.Empty,
            alerta.TipoAlerta,
            alerta.NivelAlerta,
            alerta.Mensagem,
            alerta.ValorDetectado,
            alerta.LimiteReferencia,
            alerta.Resolvido,
            alerta.DataAlerta,
            alerta.DataResolucao
        );
}