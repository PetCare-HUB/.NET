using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PetCareHub.Application.DTOs;
using PetCareHub.Application.Repositories;
using PetCareHub.Application.Services.Implementations;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Tests.Unit.Services;

public class AlertaSaudeServiceTests
{
    private readonly Mock<IAlertaSaudeRepository> _alertaRepository = new();
    private readonly Mock<IPetRepository> _petRepository = new();
    private readonly Mock<IClinicaRepository> _clinicaRepository = new();
    private readonly AlertaSaudeService _alertaSaudeService;

    public AlertaSaudeServiceTests()
    {
        _alertaSaudeService = new AlertaSaudeService(
            _alertaRepository.Object,
            _petRepository.Object,
            _clinicaRepository.Object,
            NullLogger<AlertaSaudeService>.Instance);
    }

    [Fact]
    public void Create_PetInexistente_LancaExcecao()
    {
        // Arrange
        var request = new AlertaSaudeRequest(1, "TEMPERATURA", "ALTO", "Temperatura acima do recomendado", 32.5m, 28.0m);
        _petRepository.Setup(r => r.Exists(request.PetId)).Returns(false);

        // Act
        var act = () => _alertaSaudeService.Create(request);

        // Assert
        Assert.Throws<KeyNotFoundException>(act);
        _alertaRepository.Verify(r => r.Add(It.IsAny<AlertaSaude>()), Times.Never);
    }

    [Fact]
    public void Resolve_AlertaJaResolvido_LancaExcecao()
    {
        // Arrange
        var alerta = new AlertaSaude { Id = 1, PetId = 1, Resolvido = true };
        _alertaRepository.Setup(r => r.GetById(1)).Returns(alerta);

        // Act
        void Act() => _alertaSaudeService.Resolve(1);

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
        _alertaRepository.Verify(r => r.Update(It.IsAny<AlertaSaude>()), Times.Never);
    }

    [Fact]
    public void Resolve_AlertaValido_RetornaTrue()
    {
        // Arrange
        var alerta = new AlertaSaude { Id = 1, PetId = 1, Resolvido = false };
        _alertaRepository.Setup(r => r.GetById(1)).Returns(alerta);

        // Act
        var resultado = _alertaSaudeService.Resolve(1);

        // Assert
        Assert.True(resultado);
        Assert.True(alerta.Resolvido);
        _alertaRepository.Verify(r => r.Update(alerta), Times.Once);
    }
}
