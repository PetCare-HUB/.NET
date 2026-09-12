using System.Diagnostics;

namespace PetCareHub.Infrastructure.Diagnostics;

/// <summary>
/// Fonte de spans manuais da camada de persistência (operações do <c>Repository&lt;T&gt;</c>
/// contra o Oracle). Precisa ser registrada em <c>Program.cs</c> via
/// <c>tracing.AddSource(InfraTelemetry.SourceName)</c> para o OpenTelemetry exportar os spans.
/// </summary>
public static class InfraTelemetry
{
    public const string SourceName = "PetCareHub.Infrastructure";

    public static readonly ActivitySource Source = new(SourceName);
}
