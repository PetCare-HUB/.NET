using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.DTOs;

public record ConsultaResponse(
    long Id,
    long PetId,
    string NomePet,
    long ClinicaId,
    string NomeClinica,
    DateTime DataConsulta,
    string? TipoConsulta,
    string? Descricao,
    string? Diagnostico,
    decimal? Valor,
    bool RetornoRecomendado,
    DateTime? DataRetorno
)
{
    /// <summary>
    /// Converte a entidade Consulta para ConsultaResponse.
    /// </summary>
    public static ConsultaResponse FromDomain(Consulta consulta) =>
        new(
            consulta.Id,
            consulta.PetId,
            consulta.Pet?.Nome ?? string.Empty,
            consulta.ClinicaId,
            consulta.Clinica?.Nome ?? string.Empty,
            consulta.DataConsulta,
            consulta.TipoConsulta,
            consulta.Descricao,
            consulta.Diagnostico,
            consulta.Valor,
            consulta.RetornoRecomendado,
            consulta.DataRetorno
        );
}