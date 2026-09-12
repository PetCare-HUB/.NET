using System.Diagnostics;

namespace PetCareHub.Application.Diagnostics;

/// <summary>
/// Fonte de spans manuais da camada de aplicação (regras de negócio dos Services).
/// Precisa ser registrada em <c>Program.cs</c> via <c>tracing.AddSource(AppTelemetry.SourceName)</c>
/// para que o OpenTelemetry exporte esses spans — sem isso o ActivitySource cria Activity
/// nula (custo zero, mas nada é exportado).
/// </summary>
public static class AppTelemetry
{
    public const string SourceName = "PetCareHub.Application";

    public static readonly ActivitySource Source = new(SourceName);
}
