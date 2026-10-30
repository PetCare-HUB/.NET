namespace PetCareHub.Application.Services.Interfaces;

/// <summary>Fornece à aplicação a clínica autenticada na requisição atual.</summary>
public interface ICurrentClinicContext
{
    long? ClinicaId { get; }

    /// <summary>
    /// True quando quem chamou é um admin da PetCare Hub.
    /// </summary>
    /// <remarks>
    /// O admin está acima da hierarquia de clínicas: não tem clinicaId no
    /// token, então sem este sinal o filtro global o deixaria sem enxergar
    /// absolutamente nada.
    /// </remarks>
    bool EhAdmin { get; }
}
