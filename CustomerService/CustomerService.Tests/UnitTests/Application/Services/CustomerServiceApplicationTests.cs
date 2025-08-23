using DotnetBaseKit.Components.Shared.Notifications;
using Microsoft.AspNetCore.Http;
using Moq;
using CustomerService.Application;
using CustomerService.Application.DTOs;
using CustomerService.Domain.Entities;
using CustomerService.Domain.Repositories;

namespace CustomerService.Tests.UnitTests.Application.Services
{
    public class CustomerServiceApplicationTests
    {
        private readonly Mock<ICustomerReadRepository> _readRepositoryMock;
        private readonly Mock<ICustomerWriteRepository> _writeRepositoryMock;
        private readonly NotificationContext _notificationContext;
        private readonly CustomerServiceApplication _service;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

        public CustomerServiceApplicationTests(IHttpContextAccessor httpContextAccessor)
        {
            _readRepositoryMock = new Mock<ICustomerReadRepository>();
            _writeRepositoryMock = new Mock<ICustomerWriteRepository>();
            _notificationContext = new NotificationContext();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _service = new CustomerServiceApplication(
                _notificationContext,
                _readRepositoryMock.Object,
                _writeRepositoryMock.Object,
                _httpContextAccessorMock.Object
            );
        }

        [Fact(DisplayName = "Should Return Null When User Profile Not Found")]
        public async Task Should_Return_Null_When_Customer_NotFound()
        {
            var userId = Guid.NewGuid();
            _readRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((Customer?)null);

            var result = await _service.GetByUserIdAsync(userId);

            Assert.Null(result);
        }

        [Fact(DisplayName = "Should Add Notification When User Not Found On Update")]
        public async Task Should_Add_Notification_When_User_NotFound_OnUpdate()
        {
            var userId = Guid.NewGuid();
            var dto = new CustomerUpdateDto { Name = "New Name" };

            _readRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((Customer?)null);

            var result = await _service.UpdateAsync(userId, dto);

            Assert.True(_notificationContext.HasNotifications);
            Assert.Contains(_notificationContext.Notifications,
                n => n.Key == "User" && n.Message == "User Not found");
            _writeRepositoryMock.Verify(w => w.UpdateAsync(It.IsAny<Customer>()), Times.Never);
            Assert.Null(result);
        }

        [Fact(DisplayName = "Should Create User Profile Successfully")]
        public async Task Should_Create_Customer_Successfully()
        {
            var dto = new CustomerRequestDto
            {
                AvatarUrl = "https://example.com/avatar.png"
            };

            var userId = Guid.NewGuid();
            var userName = "Test User";
            var userEmail = "test@example.com";

            _writeRepositoryMock.Setup(w => w.InsertAsync(It.IsAny<Customer>())).Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(dto, userId, userName, userEmail);

            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(userName, result.Name);
            Assert.Equal(userEmail, result.Email);
            Assert.Equal(dto.AvatarUrl, result.AvatarUrl);

            _writeRepositoryMock.Verify(w => w.InsertAsync(It.IsAny<Customer>()), Times.Once);
        }

        [Fact(DisplayName = "Should Add Notification When DTO Is Invalid On Create")]
        public async Task Should_Add_Notification_When_DTO_Is_Invalid_On_Create()
        {
            var dto = new CustomerRequestDto
            {
                AvatarUrl = "invalid-url"
            };

            var userId = Guid.NewGuid();
            var userName = "Test User";
            var userEmail = "test@example.com";

            var result = await _service.CreateAsync(dto, userId, userName, userEmail);

            Assert.Null(result);

            Assert.True(_notificationContext.HasNotifications);
            Assert.Contains(_notificationContext.Notifications, n => n.Key == "Name" || n.Key == "Email" || n.Key == "AvatarUrl");

            _writeRepositoryMock.Verify(w => w.InsertAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact(DisplayName = "Should Update User Profile Successfully")]
        public async Task Should_Update_Customer_Successfully()
        {
            var userId = Guid.NewGuid();
            var existingProfile = new Customer
            {
                AuthServiceId = userId,
                Name = "Old Name",
                Email = "old@example.com",
                AvatarUrl = "https://old.com/avatar.png"
            };

            var dto = new CustomerUpdateDto
            {
                Name = "New Name",
                Email = "new@example.com",
                AvatarUrl = "https://new.com/avatar.png"
            };

            _readRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(existingProfile);
            _writeRepositoryMock.Setup(w => w.UpdateAsync(existingProfile)).Returns(Task.CompletedTask);

            var result = await _service.UpdateAsync(userId, dto);

            Assert.NotNull(result);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.Email, result.Email);
            Assert.Equal(dto.AvatarUrl, result.AvatarUrl);

            _writeRepositoryMock.Verify(w => w.UpdateAsync(existingProfile), Times.Once);
        }
    }
}