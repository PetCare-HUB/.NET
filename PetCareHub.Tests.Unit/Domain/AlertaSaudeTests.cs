using PetCareHub.Domain.Entities;

namespace PetCareHub.Tests.Unit.Domain;

/// <summary>
/// Testa a regra de negócio direto na entidade, sem repositório nem mock nenhum — é a
/// própria <see cref="AlertaSaude"/> quem decide se pode ser resolvida.
/// </summary>
public class AlertaSaudeTests
{
    [Fact]
    public void Resolver_AlertaJaResolvido_LancaExcecao()
    {
        // Arrange
        var alerta = new AlertaSaude { Id = 1, Resolvido = true, DataResolucao = DateTime.Now.AddDays(-1) };

        // Act
        void Act() => alerta.Resolver();

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
    }

    [Fact]
    public void Resolver_AlertaPendente_MarcaComoResolvidoEDefineDataDeResolucao()
    {
        // Arrange
        var alerta = new AlertaSaude { Id = 1, Resolvido = false, DataResolucao = null };
        var antes = DateTime.Now;

        // Act
        alerta.Resolver();

        // Assert
        Assert.True(alerta.Resolvido);
        Assert.NotNull(alerta.DataResolucao);
        Assert.True(alerta.DataResolucao >= antes);
    }
}
