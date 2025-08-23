using DotnetBaseKit.Components.Shared.Notifications;
using Moq;
using UserService.Application;
using UserService.Application.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;

namespace UserService.Tests.UnitTests.Application.Services
{
    public class UserProfileServiceApplicationTests
    {
        private readonly Mock<IUserProfileReadRepository> _readRepositoryMock;
        private readonly Mock<IUserProfileWriteRepository> _writeRepositoryMock;
        private readonly NotificationContext _notificationContext;
        private readonly UserProfileServiceApplication _service;

        public UserProfileServiceApplicationTests()
        {
            _readRepositoryMock = new Mock<IUserProfileReadRepository>();
            _writeRepositoryMock = new Mock<IUserProfileWriteRepository>();
            _notificationContext = new NotificationContext();

            _service = new UserProfileServiceApplication(
                _notificationContext,
                _readRepositoryMock.Object,
                _writeRepositoryMock.Object
            );
        }

        [Fact(DisplayName = "Should Return Null When User Profile Not Found")]
        public async Task Should_Return_Null_When_UserProfile_NotFound()
        {
            var userId = Guid.NewGuid();
            _readRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((UserProfile?)null);

            var result = await _service.GetByUserIdAsync(userId);

            Assert.Null(result);
        }

        [Fact(DisplayName = "Should Add Notification When User Not Found On Update")]
        public async Task Should_Add_Notification_When_User_NotFound_OnUpdate()
        {
            var userId = Guid.NewGuid();
            var dto = new UserProfileUpdateDto { Name = "New Name" };

            _readRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((UserProfile?)null);

            var result = await _service.UpdateAsync(userId, dto);

            Assert.True(_notificationContext.HasNotifications);
            Assert.Contains(_notificationContext.Notifications,
                n => n.Key == "User" && n.Message == "User Not found");
            _writeRepositoryMock.Verify(w => w.UpdateAsync(It.IsAny<UserProfile>()), Times.Never);
            Assert.Null(result);
        }

        [Fact(DisplayName = "Should Create User Profile Successfully")]
        public async Task Should_Create_UserProfile_Successfully()
        {
            var dto = new UserProfileRequestDto
            {
                AvatarUrl = "https://example.com/avatar.png"
            };

            var userId = Guid.NewGuid();
            var userName = "Test User";
            var userEmail = "test@example.com";

            _writeRepositoryMock.Setup(w => w.InsertAsync(It.IsAny<UserProfile>())).Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(dto, userId, userName, userEmail);

            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(userName, result.Name);
            Assert.Equal(userEmail, result.Email);
            Assert.Equal(dto.AvatarUrl, result.AvatarUrl);

            _writeRepositoryMock.Verify(w => w.InsertAsync(It.IsAny<UserProfile>()), Times.Once);
        }

        [Fact(DisplayName = "Should Add Notification When DTO Is Invalid On Create")]
        public async Task Should_Add_Notification_When_DTO_Is_Invalid_On_Create()
        {
            var dto = new UserProfileRequestDto
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

            _writeRepositoryMock.Verify(w => w.InsertAsync(It.IsAny<UserProfile>()), Times.Never);
        }

        [Fact(DisplayName = "Should Update User Profile Successfully")]
        public async Task Should_Update_UserProfile_Successfully()
        {
            var userId = Guid.NewGuid();
            var existingProfile = new UserProfile
            {
                UserId = userId,
                Name = "Old Name",
                Email = "old@example.com",
                AvatarUrl = "https://old.com/avatar.png"
            };

            var dto = new UserProfileUpdateDto
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