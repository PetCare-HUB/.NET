using Moq;
using PetCareHub.Application.Repositories;
using PetCareHub.Application.Services.Implementations;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Tests.Unit.Services;

public class TutorServiceTests
{
    private readonly Mock<ITutorRepository> _tutorRepository = new();
    private readonly Mock<IClinicaRepository> _clinicaRepository = new();
    private readonly TutorService _tutorService;

    public TutorServiceTests()
    {
        _tutorService = new TutorService(_tutorRepository.Object, _clinicaRepository.Object);
    }

    [Fact]
    public void GetById_TutorInexistente_RetornaNull()
    {
        // Arrange
        _tutorRepository.Setup(r => r.GetById(1)).Returns((Tutor?)null);

        // Act
        var resultado = _tutorService.GetById(1);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public void GetById_TutorExistente_RetornaTutorMapeado()
    {
        // Arrange
        var tutor = new Tutor { Id = 1, Nome = "Ana Souza", Email = "ana@petcare.com", StatusAcesso = "ATIVO" };
        _tutorRepository.Setup(r => r.GetById(1)).Returns(tutor);

        // Act
        var resultado = _tutorService.GetById(1);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(tutor.Nome, resultado!.Nome);
        Assert.Equal(tutor.StatusAcesso, resultado.StatusAcesso);
    }

    [Fact]
    public void GetByClinica_ClinicaInexistente_LancaExcecao()
    {
        // Arrange
        _clinicaRepository.Setup(r => r.Exists(1)).Returns(false);

        // Act
        var act = () => _tutorService.GetByClinica(1);

        // Assert
        Assert.Throws<KeyNotFoundException>(act);
        _tutorRepository.Verify(r => r.GetByClinica(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public void GetByClinica_ClinicaExistente_RetornaTutoresDaClinica()
    {
        // Arrange
        _clinicaRepository.Setup(r => r.Exists(1)).Returns(true);
        _tutorRepository.Setup(r => r.GetByClinica(1)).Returns(
        [
            new Tutor { Id = 1, Nome = "Ana Souza", StatusAcesso = "ATIVO" }
        ]);

        // Act
        var resultado = _tutorService.GetByClinica(1);

        // Assert
        Assert.Single(resultado);
        Assert.Equal("Ana Souza", resultado[0].Nome);
    }
}
