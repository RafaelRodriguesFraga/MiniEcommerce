using CustomerService.Application;
using CustomerService.Application.DTOs;
using CustomerService.Application.Interfaces;
using CustomerService.Domain.Entities;
using CustomerService.Domain.Repositories;
using DotnetBaseKit.Components.Shared.Notifications;
using Moq;

namespace CustomerService.Tests.UnitTests.Application.Services;

public class CustomerServiceApplicationTests
{
    private readonly Mock<ICustomerReadRepository> _readRepositoryMock;
    private readonly Mock<ICustomerWriteRepository> _writeRepositoryMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly NotificationContext _notificationContext;
    private readonly CustomerServiceApplication _service;

    public CustomerServiceApplicationTests()
    {
        _readRepositoryMock = new Mock<ICustomerReadRepository>();
        _writeRepositoryMock = new Mock<ICustomerWriteRepository>();
        _userContextMock = new Mock<IUserContext>();
        _notificationContext = new NotificationContext();

        _service = new CustomerServiceApplication(
            _notificationContext,
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _userContextMock.Object
        );
    }

    [Fact(DisplayName = "GetOrCreate should create customer when not found")]
    public async Task GetOrCreate_Should_Create_Customer_When_NotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Rafael Teste";
        var email = "rafael@teste.com";

        _userContextMock.Setup(u => u.UserId).Returns(userId.ToString());
        _userContextMock.Setup(u => u.Name).Returns(name);
        _userContextMock.Setup(u => u.Email).Returns(email);

        _readRepositoryMock.Setup(r => r.GetByAuthServiceIdAsync(userId))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _service.GetOrCreateAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(email, result.Email);

        // Verifica se chamou o Insert no repositório
        _writeRepositoryMock.Verify(w => w.InsertAsync(It.IsAny<Customer>()), Times.Once);
    }

    [Fact(DisplayName = "GetOrCreate should return existing customer when found")]
    public async Task GetOrCreate_Should_Return_Existing_Customer()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingCustomer = new Customer(userId, "Nome Antigo", "email@antigo.com", "avatar.png");

        _userContextMock.Setup(u => u.UserId).Returns(userId.ToString());

        // Simulando que o banco ENCONTROU o cliente
        _readRepositoryMock.Setup(r => r.GetByAuthServiceIdAsync(userId))
            .ReturnsAsync(existingCustomer);

        // Act
        var result = await _service.GetOrCreateAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Nome Antigo", result.Name); // Deve manter o nome do banco, não do token

        // Garante que NÃO tentou criar de novo
        _writeRepositoryMock.Verify(w => w.InsertAsync(It.IsAny<Customer>()), Times.Never);
    }

    [Fact(DisplayName = "GetOrCreate should return error when UserContext is invalid")]
    public async Task GetOrCreate_Should_Return_Error_When_UserId_Invalid()
    {
        // Arrange
        _userContextMock.Setup(u => u.UserId).Returns(Guid.Empty.ToString()); // Token inválido

        // Act
        var result = await _service.GetOrCreateAsync();

        // Assert
        Assert.Null(result); // Ou default
        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains(_notificationContext.Notifications, n => n.Message == "UserId is invalid");
    }

    [Fact(DisplayName = "Update should update customer successfully")]
    public async Task Update_Should_Update_Customer_Successfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var customer = new Customer(userId, "Old Name", "old@email.com", null);

        var updateDto = new CustomerUpdateDto
        {
            Name = "New Name",
            Email = "new@email.com",
            AvatarUrl = "http://avatar.com/new.png"
        };

        _userContextMock.Setup(u => u.UserId).Returns(userId.ToString());

        _readRepositoryMock.Setup(r => r.GetByAuthServiceIdAsync(userId))
            .ReturnsAsync(customer);

        // Act
        var result = await _service.UpdateAsync(userId, updateDto);

        // Assert
        Assert.Equal("New Name", result.Name);
        Assert.Equal("new@email.com", result.Email);

        _writeRepositoryMock.Verify(w => w.UpdateAsync(It.Is<Customer>(c => c.Name == "New Name")), Times.Once);
    }
}