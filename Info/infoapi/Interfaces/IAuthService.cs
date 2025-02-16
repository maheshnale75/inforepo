using infoapi.Entities;

namespace infoapi.Interfaces
{
    public interface IAuthService
    {
        Task<User> RegisterUser(RegisterUser registerDto);
        Task<string> Login(LoginUser loginDto); // Return JWT Token
        Task Logout(Guid sessionId);
    }
}
