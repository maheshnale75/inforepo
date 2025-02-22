using infoapi.DbData.Models;
using infoapi.Entities;

namespace infoapi.Interfaces
{
    public interface IAuthService
    {
        Task<User> RegisterUser(RegisterUser registerDto);
        Task<string> Login(LoginUser loginDto); // Return JWT Token
        Task Logout(Guid sessionId);
        Task<string> ForgotPassword(string input);
        Task<string> ResetPassword(ResetPassword reset);
    }
}
