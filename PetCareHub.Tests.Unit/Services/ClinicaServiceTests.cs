using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PetCareHub.Application.DTOs;
using PetCareHub.Application.Repositories;
using PetCareHub.Application.Services.Implementations;
using PetCareHub.Domain.Entities;

namespace PetCareHub.Tests.Unit.Services;

public class ClinicaServiceTests
{
    private readonly Mock<IClinicaRepository> _clinicaRepository = new();
    private readonly ClinicaService _clinicaService;

    public ClinicaServiceTests()
    {
        _clinicaService = new ClinicaService(_clinicaRepository.Object, NullLogger<ClinicaService>.Instance);
    }

    // Não há teste de Create: esta API não cria clínica. Quem entra na rede se
    // cadastra em POST /auth/cadastro-clinica na API Java, definindo a própria
    // senha, e depois é aprovado por um admin. Criar clínica por aqui produziria
    // um registro sem senha — e um registro sem senha pode ser reivindicado por
    // qualquer um que tenha o código de credenciamento, que é único para todas.

    [Fact]
    public void Update_CnpjDuplicadoDeOutraClinica_LancaExcecao()
    {
        // Arrange — clínica 1 tentando trocar o próprio CNPJ para um que já pertence a outra
        var clinicaExistente = new Clinica { Id = 1, Nome = "Clínica Original", Cnpj = "11111111000111" };
        var request = new ClinicaRequest("Clínica Original", "99999999000199", "teste@petcare.com", null, null, true);

        _clinicaRepository.Setup(r => r.GetById(1)).Returns(clinicaExistente);
        _clinicaRepository.Setup(r => r.ExistsByCnpj(request.Cnpj)).Returns(true);

        // Act
        void Act() => _clinicaService.Update(1, request);

        // Assert
        var ex = Assert.Throws<InvalidOperationException>(Act);
        Assert.Contains(request.Cnpj, ex.Message);
        _clinicaRepository.Verify(r => r.Update(It.IsAny<Clinica>()), Times.Never);
    }

    [Fact]
    public void Update_MesmoCnpjDaPropriaClinica_NaoLancaExcecao()
    {
        // Manter o próprio CNPJ ao atualizar outros campos não pode disparar a checagem de
        // duplicidade — senão nenhuma clínica conseguiria se atualizar sem trocar de CNPJ.
        var clinicaExistente = new Clinica { Id = 1, Nome = "Clínica Original", Cnpj = "99999999000199" };
        var request = new ClinicaRequest("Clínica Renomeada", "99999999000199", "novo@petcare.com", null, null, true);

        _clinicaRepository.Setup(r => r.GetById(1)).Returns(clinicaExistente);

        // Act
        var resultado = _clinicaService.Update(1, request);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Clínica Renomeada", resultado!.Nome);
        _clinicaRepository.Verify(r => r.ExistsByCnpj(It.IsAny<string>()), Times.Never);
        _clinicaRepository.Verify(r => r.Update(It.IsAny<Clinica>()), Times.Once);
    }

    [Fact]
    public void Delete_ClinicaComPetsVinculados_LancaExcecao()
    {
        // Arrange
        var clinica = new Clinica { Id = 1, Nome = "Clínica Teste", Cnpj = "99999999000199" };
        _clinicaRepository.Setup(r => r.GetById(1)).Returns(clinica);
        _clinicaRepository.Setup(r => r.HasPets(1)).Returns(true);

        // Act
        void Act() => _clinicaService.Delete(1);

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
        _clinicaRepository.Verify(r => r.Delete(It.IsAny<long>()), Times.Never);
    }
}
