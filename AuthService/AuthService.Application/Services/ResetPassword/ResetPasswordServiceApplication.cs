using AuthService.Domain.Repositories;

namespace AuthService.Application.Services.ResetPassword;



public class ResetPasswordServiceApplication
{

    private readonly IResetPasswordTokenReadRepository _tokenReadRepository;
    private readonly IUserReadRepository _readRepository;
    private readonly IUserWriteRepository _writeRepository;

}