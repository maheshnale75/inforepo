using infoapi.DbData.Models;
using infoapi.Entities;
using infoapi.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace infoapi.Services
{
    public class AuthService : IAuthService
    {
        private readonly infoContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(infoContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<User> RegisterUser(RegisterUser registerDto)
        {
            if (_context.Users.Any(u => u.Username == registerDto.Username))
            {
                throw new Exception("A user with this user name is already exist");
            }

            var salt = GenerateSalt();
            var hashedPassword = HashPassword(registerDto.Password, salt);

            var user = new User
            {
                Username = registerDto.Username,
                HashedPassword = hashedPassword,
                Salt = salt,
                EmailId = registerDto.EmailId,
                RoleId = registerDto.RoleId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }
        public async Task<string> Login(LoginUser loginDto)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == loginDto.Username);
            if (user == null)
                throw new Exception("User not found");

            var hashedPassword = HashPassword(loginDto.Password, user.Salt);
            if (user.HashedPassword != hashedPassword)
                throw new Exception("Invalid credentials");

            var session = new Session
            {
                UserId = user.Id,
                RoleId = user.RoleId, // Assuming default role
                SessionId = Guid.NewGuid(),
                IsActive = true,
                LoginTime = DateTime.UtcNow
            };

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            // Generate JWT Token
            var token = GenerateJwtToken(session.SessionId.ToString(), user.Role.Role);

            return token;
        }
        public async Task Logout(Guid sessionId)
        {
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.SessionId == sessionId);
            if (session == null)
                throw new Exception("Session not found");

            session.IsActive = false;
            session.LogoutTime = DateTime.UtcNow;

            _context.Sessions.Update(session);
            await _context.SaveChangesAsync();
        }
        private string GenerateJwtToken(string sessionId, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, sessionId),
                new Claim(ClaimTypes.Role, role)
            }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + salt;
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(bytes);
            }
        }
        public string GenerateSalt()
        {
            var rng = new RNGCryptoServiceProvider();
            byte[] saltBytes = new byte[16];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }
        public async Task<string> ForgotPassword(string input)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailId == input || u.Username == input);
            if (user == null)
                throw new Exception("User not found");

            var otp = new Random().Next(100000, 999999).ToString(); // Generate 6-digit OTP
            var otpExpiration = DateTime.UtcNow.AddMinutes(10); // Set OTP expiry

            var otpEntry = new OTPTable
            {
                UserId = user.Id,
                Otp = otp,
                ExpirationTime = otpExpiration
            };

            _context.OTPTable.Add(otpEntry);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            // Send OTP via email
            var emailService = new EmailService(_configuration);
            await emailService.SendOtpEmail(user.EmailId, otp);

            return "OTP sent to your email.";
        }
        public async Task<string> ResetPassword(ResetPassword reset)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailId == reset.EmailId);
            if (user == null)
                throw new Exception("User not found");

            //var otpEntry = await _context.OTPs
            //    .Where(o => o.UserId == user.Id &&  o.ExpirationTime > DateTime.UtcNow)
            //    .FirstOrDefaultAsync();

            
            var otpEntry = await VerifyOtp(user.Id);
            if (otpEntry == null)
                throw new Exception("Invalid or expired OTP");
            var salt = GenerateSalt();
            var hashedPassword = HashPassword(reset.NewPassword, salt);

            user.HashedPassword = hashedPassword;
            user.Salt = salt;

            _context.Users.Update(user);
            _context.OTPTable.Remove(otpEntry); // Remove OTP after use
            await _context.SaveChangesAsync();

            return "Password reset successful.";
        }

        public async Task<OTPTable> VerifyOtp(int usierid)
        {
            var otpEntry = await _context.OTPTable
               .Where(o => o.UserId == usierid && o.ExpirationTime > DateTime.UtcNow)
               .FirstOrDefaultAsync();

            if (otpEntry != null)
            {
                return otpEntry;
            }
            return null;

        }
    }
}
