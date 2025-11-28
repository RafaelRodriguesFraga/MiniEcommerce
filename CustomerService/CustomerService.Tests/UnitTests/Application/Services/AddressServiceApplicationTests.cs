using CustomerService.Application.DTOs.Address;
using CustomerService.Application.Services.Address;
using CustomerService.Domain.Entities;
using CustomerService.Domain.Repositories;
using CustomerService.Domain.Repositories.Address;
using DotnetBaseKit.Components.Shared.Notifications;
using Moq;


namespace CustomerService.Tests.UnitTests.Application.Services;

public class AddressServiceApplicationTests
{
    private readonly Mock<IAddressReadRepository> _addressReadRepositoryMock;
    private readonly Mock<IAddressWriteRepository> _addressWriteRepositoryMock;
    private readonly Mock<ICustomerReadRepository> _customerReadRepositoryMock;
    private readonly NotificationContext _notificationContext;
    private readonly AddressServiceApplication _service;

    public AddressServiceApplicationTests()
    {
        _addressReadRepositoryMock = new Mock<IAddressReadRepository>();
        _addressWriteRepositoryMock = new Mock<IAddressWriteRepository>();
        _customerReadRepositoryMock = new Mock<ICustomerReadRepository>();
        _notificationContext = new NotificationContext();

        _service = new AddressServiceApplication(
            _notificationContext,
            _addressReadRepositoryMock.Object,
            _addressWriteRepositoryMock.Object,
            _customerReadRepositoryMock.Object
        );
    }

    [Fact(DisplayName = "CreateAsync should add notification when Customer is not found")]
    public async Task CreateAsync_Should_Notify_When_Customer_NotFound()
    {
        // Arrange
        var authUserId = Guid.NewGuid();
        _customerReadRepositoryMock.Setup(r => r.GetByAuthServiceIdAsync(authUserId))
            .ReturnsAsync((Customer?)null);

        var dto = new AddressRequestDto();

        await _service.CreateAsync(dto, authUserId);

        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains(_notificationContext.Notifications, n => n.Key == "Customer" && n.Message == "Customer not found");
        _addressWriteRepositoryMock.Verify(w => w.InsertAsync(It.IsAny<Address>()), Times.Never);
    }

    [Fact(DisplayName = "CreateAsync should create address linked to CustomerId (PK) not AuthId")]
    public async Task CreateAsync_Should_Link_To_CustomerPK()
    {
        var authUserId = Guid.NewGuid();
        var customerPk = Guid.NewGuid();
        var customer = new Customer(authUserId, "Test", "test@email.com", null);


        _customerReadRepositoryMock.Setup(r => r.GetByAuthServiceIdAsync(authUserId))
            .ReturnsAsync(customer);

        var dto = new AddressRequestDto
        {
            Street = "Rua Teste",
            Number = "123",
            City = "São Paulo",
            State = "SP",
            PostalCode = "00000000",
            Neighborhood = "Bairro"
        };

        await _service.CreateAsync(dto, authUserId);

        _addressWriteRepositoryMock.Verify(w => w.InsertAsync(It.Is<Address>(a =>
            a.CustomerId == customer.Id &&
            a.Street == dto.Street
        )), Times.Once);
    }

    [Fact(DisplayName = "GetByIdAsync should return Forbidden when address belongs to another customer")]
    public async Task GetByIdAsync_Should_Return_Forbidden_For_Other_User()
    {
        var authUserId = Guid.NewGuid();
        var customerMe = new Customer(authUserId, "Me", "me@test.com", null); // ID X

        var otherCustomerId = Guid.NewGuid(); // ID Y
        var addressOfOther = new Address(otherCustomerId, "Rua Outro", "1", "", "B", "C", "S", "00");

        _customerReadRepositoryMock.Setup(r => r.GetByAuthServiceIdAsync(authUserId))
            .ReturnsAsync(customerMe);

        _addressReadRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(addressOfOther);

        var result = await _service.GetByIdAsync(Guid.NewGuid(), authUserId);

        Assert.Null(result);
        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains(_notificationContext.Notifications, n => n.Key == "Address.Forbidden");
    }

    [Fact(DisplayName = "GetByIdAsync should return Success when address belongs to user")]
    public async Task GetByIdAsync_Should_Return_Address_When_Owner_Is_Correct()
    {
        var authUserId = Guid.NewGuid();
        var customer = new Customer(authUserId, "Me", "me@test.com", null);

        var address = new Address(customer.Id, "Rua Minha", "1", "", "B", "C", "S", "00");

        _customerReadRepositoryMock.Setup(r => r.GetByAuthServiceIdAsync(authUserId))
            .ReturnsAsync(customer);

        _addressReadRepositoryMock.Setup(r => r.GetByIdAsync(address.Id))
            .ReturnsAsync(address);

        var result = await _service.GetByIdAsync(address.Id, authUserId);

        Assert.NotNull(result);
        Assert.Equal(address.Street, result.Street);
    }

    [Fact(DisplayName = "UpdateAsync should update when user is owner")]
    public async Task UpdateAsync_Should_Update_When_Owner()
    {
        var authUserId = Guid.NewGuid();
        var customer = new Customer(authUserId, "Me", "me@test.com", null);
        var address = new Address(customer.Id, "Old St", "1", "", "B", "C", "S", "00");

        var updateDto = new AddressUpdateDto { Street = "New St" };

        _customerReadRepositoryMock.Setup(r => r.GetByAuthServiceIdAsync(authUserId))
            .ReturnsAsync(customer);
        _addressReadRepositoryMock.Setup(r => r.GetByIdAsync(address.Id))
            .ReturnsAsync(address);

        await _service.UpdateAsync(address.Id, updateDto, authUserId);

        Assert.Equal("New St", address.Street);
        _addressWriteRepositoryMock.Verify(w => w.UpdateAsync(address), Times.Once);
        Assert.False(_notificationContext.HasNotifications);
    }

    [Fact(DisplayName = "UpdateAsync should fail when address not found")]
    public async Task UpdateAsync_Should_Fail_When_Address_NotFound()
    {
        var authUserId = Guid.NewGuid();
        var customer = new Customer(authUserId, "Me", "me@test.com", null);

        _customerReadRepositoryMock.Setup(r => r.GetByAuthServiceIdAsync(authUserId))
            .ReturnsAsync(customer);

        _addressReadRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Address?)null);

        // Act
        await _service.UpdateAsync(Guid.NewGuid(), new AddressUpdateDto(), authUserId);

        // Assert
        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains(_notificationContext.Notifications, n => n.Key == "Address.NotFound");
        _addressWriteRepositoryMock.Verify(w => w.UpdateAsync(It.IsAny<Address>()), Times.Never);
    }


    [Fact(DisplayName = "DeleteAsync should delete when user is owner")]
    public async Task DeleteAsync_Should_Delete_When_Owner()
    {
        var authUserId = Guid.NewGuid();
        var customer = new Customer(authUserId, "Me", "me@test.com", null);
        var address = new Address(customer.Id, "St", "1", "", "B", "C", "S", "00");

        _customerReadRepositoryMock.Setup(r => r.GetByAuthServiceIdAsync(authUserId))
            .ReturnsAsync(customer);
        _addressReadRepositoryMock.Setup(r => r.GetByIdAsync(address.Id))
            .ReturnsAsync(address);

        await _service.DeleteAsync(address.Id, authUserId);

        _addressWriteRepositoryMock.Verify(w => w.DeleteAsync(address), Times.Once);
    }

    [Fact(DisplayName = "DeleteAsync should return Forbidden for another user")]
    public async Task DeleteAsync_Should_Return_Forbidden()
    {
        var authUserId = Guid.NewGuid();
        var customerMe = new Customer(authUserId, "Me", "me@test.com", null);

        var otherCustomerId = Guid.NewGuid();
        var addressOfOther = new Address(otherCustomerId, "St", "1", "", "B", "C", "S", "00");

        _customerReadRepositoryMock.Setup(r => r.GetByAuthServiceIdAsync(authUserId))
            .ReturnsAsync(customerMe);
        _addressReadRepositoryMock.Setup(r => r.GetByIdAsync(addressOfOther.Id))
            .ReturnsAsync(addressOfOther);

        await _service.DeleteAsync(addressOfOther.Id, authUserId);

        _addressWriteRepositoryMock.Verify(w => w.DeleteAsync(It.IsAny<Address>()), Times.Never);
        Assert.Contains(_notificationContext.Notifications, n => n.Key == "Address.Forbidden");
    }
}