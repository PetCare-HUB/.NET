using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PetCareHub.Application.DTOs;
using PetCareHub.Application.Repositories;
using PetCareHub.Application.Services.Implementations;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Tests.Unit.Services;

public class ConsultaServiceTests
{
    private readonly Mock<IConsultaRepository> _consultaRepository = new();
    private readonly Mock<IPetRepository> _petRepository = new();
    private readonly Mock<IClinicaRepository> _clinicaRepository = new();
    private readonly ConsultaService _consultaService;

    public ConsultaServiceTests()
    {
        _consultaService = new ConsultaService(
            _consultaRepository.Object,
            _petRepository.Object,
            _clinicaRepository.Object,
            NullLogger<ConsultaService>.Instance);
    }

    [Fact]
    public void Create_PetInexistente_LancaExcecao()
    {
        // Arrange
        var request = new ConsultaRequest(1, 1, DateTime.Now, "CHECKUP", null, null, 150m, false, null);
        _petRepository.Setup(r => r.Exists(request.PetId)).Returns(false);

        // Act
        var act = () => _consultaService.Create(request);

        // Assert
        Assert.Throws<KeyNotFoundException>(act);
        _consultaRepository.Verify(r => r.Add(It.IsAny<Consulta>()), Times.Never);
    }

    [Fact]
    public void Create_DadosValidos_RetornaConsultaCriada()
    {
        // Arrange
        var request = new ConsultaRequest(1, 1, DateTime.Now, "CHECKUP", "Consulta de rotina", null, 150m, false, null);
        _petRepository.Setup(r => r.Exists(request.PetId)).Returns(true);
        _clinicaRepository.Setup(r => r.Exists(request.ClinicaId)).Returns(true);

        // Act
        var resultado = _consultaService.Create(request);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("CHECKUP", resultado.TipoConsulta);
        _consultaRepository.Verify(r => r.Add(It.IsAny<Consulta>()), Times.Once);
    }

    [Fact]
    public void GetByClinica_ClinicaInexistente_LancaExcecao()
    {
        // Arrange
        _clinicaRepository.Setup(r => r.Exists(1)).Returns(false);

        // Act
        var act = () => _consultaService.GetByClinica(1);

        // Assert
        Assert.Throws<KeyNotFoundException>(act);
    }
}
