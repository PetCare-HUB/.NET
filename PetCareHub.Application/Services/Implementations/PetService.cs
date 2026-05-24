using PetCareHub.Application.DTOs;
using PetCareHub.Application.Repositories;
using PetCareHub.Application.Services.Interfaces;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Application.Services.Implementations;

public sealed class PetService(
    IPetRepository petRepository,
    IClinicaRepository clinicaRepository,
    IResponsavelRepository responsavelRepository) : IPetService
{
    public IReadOnlyList<PetResponse> GetAll()
    {
        return petRepository
            .GetAll()
            .Select(PetResponse.FromDomain)
            .ToList();
    }

    public IReadOnlyList<PetResponse> GetFiltered(long? clinicaId, string? especie, bool? ativo)
    {
        return petRepository
            .GetFiltered(clinicaId, especie, ativo)
            .Select(PetResponse.FromDomain)
            .ToList();
    }

    public PetResponse? GetById(long id)
    {
        var pet = petRepository.GetByIdWithRelations(id);
        return pet is null ? null : PetResponse.FromDomain(pet);
    }

    public IReadOnlyList<PetResponse> GetByClinica(long clinicaId)
    {
        if (!clinicaRepository.Exists(clinicaId))
            throw new KeyNotFoundException($"Clínica com id {clinicaId} não encontrada.");

        return petRepository
            .GetByClinica(clinicaId)
            .Select(PetResponse.FromDomain)
            .ToList();
    }

    public PetResponse Create(PetRequest request)
    {
        if (!clinicaRepository.Exists(request.ClinicaId))
            throw new KeyNotFoundException($"Clínica com id {request.ClinicaId} não encontrada.");

        if (!responsavelRepository.Exists(request.ResponsavelId))
            throw new KeyNotFoundException($"Responsável com id {request.ResponsavelId} não encontrado.");

        var pet = new Pet
        {
            ResponsavelId = request.ResponsavelId,
            ClinicaId = request.ClinicaId,
            Nome = request.Nome,
            Especie = request.Especie,
            Raca = request.Raca,
            DataNascimento = request.DataNascimento,
            PesoKg = request.PesoKg,
            Sexo = request.Sexo,
            CondicoesCronicas = request.CondicoesCronicas,
            DataCadastro = DateTime.Now,
            Ativo = request.Ativo
        };

        petRepository.Add(pet);

        return PetResponse.FromDomain(pet);
    }

    public PetResponse? Update(long id, PetRequest request)
    {
        var pet = petRepository.GetById(id);
        if (pet is null)
            return null;

        if (request.ClinicaId != pet.ClinicaId && !clinicaRepository.Exists(request.ClinicaId))
            throw new KeyNotFoundException($"Clínica com id {request.ClinicaId} não encontrada.");

        if (request.ResponsavelId != pet.ResponsavelId && !responsavelRepository.Exists(request.ResponsavelId))
            throw new KeyNotFoundException($"Responsável com id {request.ResponsavelId} não encontrado.");

        pet.ResponsavelId = request.ResponsavelId;
        pet.ClinicaId = request.ClinicaId;
        pet.Nome = request.Nome;
        pet.Especie = request.Especie;
        pet.Raca = request.Raca;
        pet.DataNascimento = request.DataNascimento;
        pet.PesoKg = request.PesoKg;
        pet.Sexo = request.Sexo;
        pet.CondicoesCronicas = request.CondicoesCronicas;
        pet.Ativo = request.Ativo;

        petRepository.Update(pet);

        return PetResponse.FromDomain(pet);
    }

    public bool Delete(long id)
    {
        return petRepository.Delete(id);
    }
}