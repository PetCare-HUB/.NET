using System.Globalization;
using PetCareHub.Application.Repositories;
using PetCareHub.Application.Services.Interfaces;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.Services.Implementations;

public sealed class ResumoPetService(
    IPetIotSummarySource summarySource,
    IResumoPetRepository resumoRepository) : IResumoPetService
{
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    public async Task<ResumoPetIa?> GenerateAsync(
        long clinicaId,
        long petId,
        DateTime inicio,
        DateTime fim)
    {
        if (inicio >= fim)
            throw new ArgumentException("A data inicial precisa ser anterior à data final.");

        if (fim - inicio > TimeSpan.FromDays(31))
            throw new ArgumentException("O período máximo para gerar um resumo é de 31 dias.");

        var dados = await summarySource.GetPeriodSummaryAsync(petId, inicio, fim);
        if (dados is null)
            return null;

        var resumo = new ResumoPetIa
        {
            PetId = petId,
            ClinicaId = clinicaId,
            PeriodoInicio = inicio,
            PeriodoFim = fim,
            GeradoEmUtc = DateTime.UtcNow,
            TotalLeiturasColeira = dados.TotalLeiturasColeira,
            LeiturasSedentarias = dados.LeiturasSedentarias,
            LeiturasModeradas = dados.LeiturasModeradas,
            LeiturasAtivas = dados.LeiturasAtivas,
            TotalLeiturasComedouro = dados.TotalLeiturasComedouro,
            ConsumoTotalGramas = dados.ConsumoTotalGramas,
            NivelMedioRacaoPct = dados.NivelMedioRacaoPct,
            TotalLeiturasAmbiente = dados.TotalLeiturasAmbiente,
            TemperaturaMedia = dados.TemperaturaMedia,
            UmidadeMediaPct = dados.UmidadeMediaPct,
            Conteudo = BuildContent(petId, inicio, fim, dados)
        };

        await resumoRepository.SaveAsync(resumo);
        return resumo;
    }

    public Task<ResumoPetIa?> GetAsync(long clinicaId, long petId, DateTime inicio, DateTime fim)
    {
        if (inicio >= fim)
            throw new ArgumentException("A data inicial precisa ser anterior à data final.");

        return resumoRepository.GetAsync(clinicaId, petId, inicio, fim);
    }

    private static string BuildContent(long petId, DateTime inicio, DateTime fim, PetCareHub.Application.DTOs.PetIotSummaryData dados)
    {
        var atividade = dados.TotalLeiturasColeira == 0
            ? "Sem leituras de atividade no período."
            : $"Atividade registrada em {dados.TotalLeiturasColeira} leituras: " +
              $"{dados.LeiturasSedentarias} sedentárias, {dados.LeiturasModeradas} moderadas e {dados.LeiturasAtivas} ativas.";

        var alimentacao = dados.TotalLeiturasComedouro == 0
            ? "Sem leituras do comedouro no período."
            : $"Comedouro: consumo acumulado de {Format(dados.ConsumoTotalGramas)} g; " +
              $"nível médio de ração {Format(dados.NivelMedioRacaoPct)}%.";

        var ambiente = dados.TotalLeiturasAmbiente == 0
            ? "Sem leituras ambientais no período."
            : $"Ambiente: temperatura média de {Format(dados.TemperaturaMedia)} °C " +
              $"e umidade média de {Format(dados.UmidadeMediaPct)}%.";

        return $"Resumo factual de dados do pet {petId}, de {inicio:O} até {fim:O}. " +
               $"{atividade} {alimentacao} {ambiente} " +
               "Este documento resume leituras registradas e não constitui diagnóstico veterinário.";
    }

    private static string Format(decimal? value) =>
        value?.ToString("0.##", PtBr) ?? "indisponível";
}
