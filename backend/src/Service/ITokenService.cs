namespace ProjectX.API.Service;

public interface ITokenService
{
    string GenerateToken(int userId, string email, string role);
}
