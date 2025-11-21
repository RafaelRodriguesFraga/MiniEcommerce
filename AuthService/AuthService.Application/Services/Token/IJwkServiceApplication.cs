namespace AuthService.Application.Services.Token;

public interface IJwkServiceApplication
{
    object GetJsonWebKeySet();
}