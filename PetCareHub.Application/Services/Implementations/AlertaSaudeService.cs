using PetCareHub.Application.DTOs;
using PetCareHub.Application.Repositories;
using PetCareHub.Application.Services.Interfaces;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.Services.Implementations;

public sealed class AlertaSaudeService(
    IAlertaSaudeRepository alertaRepository,
    IPetRepository petRepository,
    IClinicaRepository clinicaRepository) : IAlertaSaudeService
{
    public IReadOnlyList<AlertaSaudeResponse> GetAll()
    {
        return alertaRepository
            .GetAll()
            .Select(AlertaSaudeResponse.FromDomain)
            .ToList();
    }
    
    public AlertaSaudeResponse? GetById(long id)
    {
        var alerta = alertaRepository.GetById(id);
        return alerta is null ? null : AlertaSaudeResponse.FromDomain(alerta);
    }
    
    public IReadOnlyList<AlertaSaudeResponse> GetByPet(long petId)
    {
        if (!petRepository.Exists(petId))
            throw new KeyNotFoundException($"Pet com id {petId} não encontrado.");

        return alertaRepository
            .GetByPet(petId)
            .Select(AlertaSaudeResponse.FromDomain)
            .ToList();
    }
    
    public IReadOnlyList<AlertaSaudeResponse> GetByClinica(long clinicaId)
    {
        if (!clinicaRepository.Exists(clinicaId))
            throw new KeyNotFoundException($"Clínica com id {clinicaId} não encontrada.");

        return alertaRepository
            .GetByClinica(clinicaId)
            .Select(AlertaSaudeResponse.FromDomain)
            .ToList();
    }
    
    public AlertaSaudeResponse Create(AlertaSaudeRequest request)
    {
        if (!petRepository.Exists(request.PetId))
            throw new KeyNotFoundException($"Pet com id {request.PetId} não encontrado.");

        var alerta = new AlertaSaude
        {
            PetId = request.PetId,
            TipoAlerta = request.TipoAlerta,
            NivelAlerta = request.NivelAlerta,
            Mensagem = request.Mensagem,
            ValorDetectado = request.ValorDetectado,
            LimiteReferencia = request.LimiteReferencia,
            LeituraId = request.LeituraId,
            Resolvido = false,
            DataAlerta = DateTime.Now
        };

        alertaRepository.Add(alerta);

        return AlertaSaudeResponse.FromDomain(alerta);
    }
    
    public AlertaSaudeResponse? Update(long id, AlertaSaudeRequest request)
    {
        var alerta = alertaRepository.GetById(id);
        if (alerta is null)
            return null;

        if (request.PetId != alerta.PetId && !petRepository.Exists(request.PetId))
            throw new KeyNotFoundException($"Pet com id {request.PetId} não encontrado.");

        alerta.PetId = request.PetId;
        alerta.TipoAlerta = request.TipoAlerta;
        alerta.NivelAlerta = request.NivelAlerta;
        alerta.Mensagem = request.Mensagem;
        alerta.ValorDetectado = request.ValorDetectado;
        alerta.LimiteReferencia = request.LimiteReferencia;

        alertaRepository.Update(alerta);

        return AlertaSaudeResponse.FromDomain(alerta);
    }

    public bool Delete(long id)
    {
        return alertaRepository.Delete(id);
    }

    public bool Resolve(long id)
    {
        var alerta = alertaRepository.GetById(id);
        if (alerta is null)
            return false;

        if (alerta.Resolvido)
            throw new InvalidOperationException("Este alerta já está resolvido.");

        alertaRepository.ResolveAlert(id);
        return true;
    }
}