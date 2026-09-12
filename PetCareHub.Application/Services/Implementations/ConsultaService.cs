using Microsoft.Extensions.Logging;
using PetCareHub.Application.DTOs;
using PetCareHub.Application.Diagnostics;
using PetCareHub.Application.Repositories;
using PetCareHub.Application.Services.Interfaces;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.Services.Implementations;

public sealed class ConsultaService(
    IConsultaRepository consultaRepository,
    IPetRepository petRepository,
    IClinicaRepository clinicaRepository,
    ILogger<ConsultaService> logger) : IConsultaService
{
    public IReadOnlyList<ConsultaResponse> GetAll()
    {
        return consultaRepository
            .GetAll()
            .Select(ConsultaResponse.FromDomain)
            .ToList();
    }

    public IReadOnlyList<ConsultaResponse> GetFiltered(
        long? clinicaId,
        long? petId,
        string? tipoConsulta,
        bool? retornoRecomendado)
    {
        return consultaRepository
            .GetFiltered(clinicaId, petId, tipoConsulta, retornoRecomendado)
            .Select(ConsultaResponse.FromDomain)
            .ToList();
    }

    public ConsultaResponse? GetById(long id)
    {
        var consulta = consultaRepository.GetById(id);
        return consulta is null ? null : ConsultaResponse.FromDomain(consulta);
    }

    public IReadOnlyList<ConsultaResponse> GetByClinica(long clinicaId)
    {
        if (!clinicaRepository.Exists(clinicaId))
            throw new KeyNotFoundException($"Clínica com id {clinicaId} não encontrada.");

        return consultaRepository
            .GetByClinica(clinicaId)
            .Select(ConsultaResponse.FromDomain)
            .ToList();
    }

    public IReadOnlyList<ConsultaResponse> GetByPet(long petId)
    {
        if (!petRepository.Exists(petId))
            throw new KeyNotFoundException($"Pet com id {petId} não encontrado.");

        return consultaRepository
            .GetByPet(petId)
            .Select(ConsultaResponse.FromDomain)
            .ToList();
    }

    public ConsultaResponse Create(ConsultaRequest request)
    {
        using var activity = AppTelemetry.Source.StartActivity("ConsultaService.Create");
        activity?.SetTag("pet.id", request.PetId);
        activity?.SetTag("clinica.id", request.ClinicaId);

        if (!petRepository.Exists(request.PetId))
        {
            logger.LogWarning("Tentativa de criar consulta para pet {PetId} inexistente", request.PetId);
            throw new KeyNotFoundException($"Pet com id {request.PetId} não encontrado.");
        }

        if (!clinicaRepository.Exists(request.ClinicaId))
        {
            logger.LogWarning("Tentativa de criar consulta para clínica {ClinicaId} inexistente", request.ClinicaId);
            throw new KeyNotFoundException($"Clínica com id {request.ClinicaId} não encontrada.");
        }

        var consulta = new Consulta
        {
            PetId = request.PetId,
            ClinicaId = request.ClinicaId,
            DataConsulta = request.DataConsulta,
            TipoConsulta = request.TipoConsulta,
            Descricao = request.Descricao,
            Diagnostico = request.Diagnostico,
            Valor = request.Valor,
            RetornoRecomendado = request.RetornoRecomendado,
            DataRetorno = request.DataRetorno
        };

        consultaRepository.Add(consulta);

        logger.LogInformation("Consulta {ConsultaId} criada para o pet {PetId}", consulta.Id, consulta.PetId);

        return ConsultaResponse.FromDomain(consulta);
    }

    public ConsultaResponse? Update(long id, ConsultaRequest request)
    {
        using var activity = AppTelemetry.Source.StartActivity("ConsultaService.Update");
        activity?.SetTag("consulta.id", id);

        var consulta = consultaRepository.GetById(id);
        if (consulta is null)
            return null;

        if (request.PetId != consulta.PetId && !petRepository.Exists(request.PetId))
            throw new KeyNotFoundException($"Pet com id {request.PetId} não encontrado.");

        if (request.ClinicaId != consulta.ClinicaId && !clinicaRepository.Exists(request.ClinicaId))
            throw new KeyNotFoundException($"Clínica com id {request.ClinicaId} não encontrada.");

        consulta.PetId = request.PetId;
        consulta.ClinicaId = request.ClinicaId;
        consulta.DataConsulta = request.DataConsulta;
        consulta.TipoConsulta = request.TipoConsulta;
        consulta.Descricao = request.Descricao;
        consulta.Diagnostico = request.Diagnostico;
        consulta.Valor = request.Valor;
        consulta.RetornoRecomendado = request.RetornoRecomendado;
        consulta.DataRetorno = request.DataRetorno;

        consultaRepository.Update(consulta);

        return ConsultaResponse.FromDomain(consulta);
    }

    public bool Delete(long id)
    {
        return consultaRepository.Delete(id);
    }
}