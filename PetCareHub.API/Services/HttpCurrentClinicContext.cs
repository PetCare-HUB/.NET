using PetCareHub.Application.Services.Interfaces;

namespace PetCareHub.API.Services;

/// <summary>Lê o identificador da clínica da claim emitida pelo login Java.</summary>
public sealed class HttpCurrentClinicContext(IHttpContextAccessor httpContextAccessor) : ICurrentClinicContext
{
    /// <summary>Identificador da clínica autenticada, ou nulo se a claim estiver ausente/inválida.</summary>
    public long? ClinicaId => long.TryParse(
        httpContextAccessor.HttpContext?.User.FindFirst("clinicaId")?.Value,
        out var clinicaId) && clinicaId > 0
            ? clinicaId
            : null;

    /// <summary>
    /// Admin da PetCare Hub, pela claim <c>role</c> do mesmo token que o Java emite.
    /// </summary>
    /// <remarks>
    /// Lido da claim bruta e não de <c>User.IsInRole</c> de propósito: a API
    /// configura <c>MapInboundClaims = false</c>, então o nome da claim
    /// permanece exatamente "role" como veio do Java.
    /// </remarks>
    public bool EhAdmin => string.Equals(
        httpContextAccessor.HttpContext?.User.FindFirst("role")?.Value,
        "ADMIN",
        StringComparison.OrdinalIgnoreCase);
}
