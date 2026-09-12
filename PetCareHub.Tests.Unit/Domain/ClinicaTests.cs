using PetCareHub.Domain.Entities;

namespace PetCareHub.Tests.Unit.Domain;

/// <summary>
/// Testa a regra de negócio direto na entidade, sem repositório nem mock nenhum. O fato de
/// "ter pets vinculados" vem de fora (só o repositório sabe isso de verdade), mas quem decide
/// a regra em cima desse fato é a própria <see cref="Clinica"/>.
/// </summary>
public class ClinicaTests
{
    [Fact]
    public void GarantirQuePodeSerExcluida_ComPetsVinculados_LancaExcecao()
    {
        // Arrange
        var clinica = new Clinica { Id = 1, Nome = "Clínica Teste", Cnpj = "99999999000199" };

        // Act
        void Act() => clinica.GarantirQuePodeSerExcluida(possuiPetsVinculados: true);

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
    }

    [Fact]
    public void GarantirQuePodeSerExcluida_SemPetsVinculados_NaoLancaExcecao()
    {
        // Arrange
        var clinica = new Clinica { Id = 1, Nome = "Clínica Teste", Cnpj = "99999999000199" };

        // Act
        var exception = Record.Exception(() => clinica.GarantirQuePodeSerExcluida(possuiPetsVinculados: false));

        // Assert
        Assert.Null(exception);
    }
}
