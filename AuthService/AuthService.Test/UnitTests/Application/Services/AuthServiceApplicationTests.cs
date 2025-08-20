using AuthService.Application.Services.Auth;
using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;
using DotnetBaseKit.Components.Shared.Notifications;
using Moq;

namespace AuthService.Test.UnitTests.Application.Services;

public class AuthServiceApplicationTests
{
    private readonly Mock<IUserReadRepository> _readRepositoryMock;
    private readonly Mock<IUserWriteRepository> _writeRepositoryMock;
    private readonly NotificationContext _notificationContext;
    private readonly AuthServiceApplication _authService;
    private readonly string _email;

    public AuthServiceApplicationTests()
    {
        _readRepositoryMock = new Mock<IUserReadRepository>();
        _writeRepositoryMock = new Mock<IUserWriteRepository>();
        _notificationContext = new NotificationContext();
        _authService = new AuthServiceApplication(_notificationContext, _writeRepositoryMock.Object,
            _readRepositoryMock.Object);

        _email = "notfound@example.com";

    }

    [Fact(DisplayName = "should add notification when user is not found")]
    public async Task Should_Add_Notification_When_User_Not_Found()
    {

        _readRepositoryMock.Setup(r => r.GetByEmailAsync(_email)).ReturnsAsync((User?)null);

        await _authService.ResetPasswordAsync(_email, "newPass123");

        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains(_notificationContext.Notifications, n => n.Key == "User" && n.Message == "User not found");
        _writeRepositoryMock.Verify(w => w.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact(DisplayName = "should update password when user is found")]
    public async Task Should_Update_Password_When_User_Is_Found()
    {
        var email = "found@example.com";
        var user = new User(It.IsAny<string>(), email, "oldPass");
        _readRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

        await _authService.ResetPasswordAsync(email, "newPass123");

        _writeRepositoryMock.Verify(w => w.UpdateAsync(It.Is<User>(u => u == user)), Times.Once);
        Assert.False(_notificationContext.HasNotifications);
    }
}