using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.DTOs;

public record ScoreSaudeResponse(
    long Id,
    long PetId,
    string? NomePet,
    decimal ScoreTotal,
    decimal? ScoreAtividade,
    decimal? ScoreAlimentacao,
    decimal? ScoreAmbiente,
    decimal? ScoreConsulta,
    decimal? ScorePreventivo,
    string Categoria,
    DateTime DataCalculo
)
{
    public static ScoreSaudeResponse FromDomain(ScoreSaude score) =>
        new(
            score.Id,
            score.PetId,
            score.Pet?.Nome,
            score.ScoreTotal,
            score.ScoreAtividade,
            score.ScoreAlimentacao,
            score.ScoreAmbiente,
            score.ScoreConsulta,
            score.ScorePreventivo,
            score.Categoria,
            score.DataCalculo
        );
}