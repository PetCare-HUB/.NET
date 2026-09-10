using Moq;
using PetCareHub.Application.DTOs;
using PetCareHub.Application.Repositories;
using PetCareHub.Application.Services.Implementations;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Tests.Unit.Services;

public class PetServiceTests
{
    private readonly Mock<IPetRepository> _petRepository = new();
    private readonly Mock<IClinicaRepository> _clinicaRepository = new();
    private readonly Mock<ITutorRepository> _tutorRepository = new();
    private readonly PetService _petService;

    public PetServiceTests()
    {
        _petService = new PetService(
            _petRepository.Object,
            _clinicaRepository.Object,
            _tutorRepository.Object);
    }

    [Fact]
    public void Create_ClinicaInexistente_LancaExcecao()
    {
        // Arrange
        var request = new PetRequest(1, 1, "Rex", "CAO", null, null, 12.5m, "M", null, true);
        _clinicaRepository.Setup(r => r.Exists(request.ClinicaId)).Returns(false);

        // Act
        var act = () => _petService.Create(request);

        // Assert
        Assert.Throws<KeyNotFoundException>(act);
        _petRepository.Verify(r => r.Add(It.IsAny<Pet>()), Times.Never);
    }

    [Fact]
    public void GetByClinica_ClinicaInexistente_LancaExcecao()
    {
        // Arrange
        _clinicaRepository.Setup(r => r.Exists(1)).Returns(false);

        // Act
        var act = () => _petService.GetByClinica(1);

        // Assert
        Assert.Throws<KeyNotFoundException>(act);
    }

    [Fact]
    public void GetById_PetInexistente_RetornaNull()
    {
        // Arrange
        _petRepository.Setup(r => r.GetByIdWithRelations(1)).Returns((Pet?)null);

        // Act
        var resultado = _petService.GetById(1);

        // Assert
        Assert.Null(resultado);
    }
}
