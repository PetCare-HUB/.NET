using MongoDB.Driver;
using PetCareHub.Application.Repositories;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Infrastructure.Persistence.Repositories;

/// <summary>Salva e consulta documentos de contexto da IA na coleção do MongoDB.</summary>
public sealed class ResumoPetMongoRepository(IMongoDatabase database) : IResumoPetRepository
{
    private readonly IMongoCollection<ResumoPetIa> _resumos = database.GetCollection<ResumoPetIa>("resumos_pet_ia");

    public async Task SaveAsync(ResumoPetIa resumo)
    {
        var filtro = Builders<ResumoPetIa>.Filter.Eq(item => item.ClinicaId, resumo.ClinicaId) &
                     Builders<ResumoPetIa>.Filter.Eq(item => item.PetId, resumo.PetId) &
                     Builders<ResumoPetIa>.Filter.Eq(item => item.PeriodoInicio, resumo.PeriodoInicio) &
                     Builders<ResumoPetIa>.Filter.Eq(item => item.PeriodoFim, resumo.PeriodoFim);

        await _resumos.ReplaceOneAsync(filtro, resumo, new ReplaceOptions { IsUpsert = true });
    }

    public async Task<ResumoPetIa?> GetAsync(long clinicaId, long petId, DateTime inicio, DateTime fim)
    {
        return await _resumos.Find(item => item.ClinicaId == clinicaId &&
                                           item.PetId == petId &&
                                           item.PeriodoInicio == inicio &&
                                           item.PeriodoFim == fim)
            .FirstOrDefaultAsync();
    }
}
