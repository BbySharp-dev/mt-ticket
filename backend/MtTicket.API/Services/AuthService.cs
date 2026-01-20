using Microsoft.IdentityModel.Tokens;
using MtTicket.API.DTOs.User;
using MtTicket.API.Models;
using MtTicket.API.Repositories.UnitOfWork;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MtTicket.API.Services;

/// <summary>
/// Service implementation cho authentication
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<AuthResult> RegisterAsync(RegisterDto registerDto)
    {
        // Kiểm tra username đã tồn tại chưa
        if (await _unitOfWork.Users.UsernameExistsAsync(registerDto.Username))
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "Username đã tồn tại"
            };
        }

        // Kiểm tra email đã tồn tại chưa
        if (await _unitOfWork.Users.EmailExistsAsync(registerDto.Email))
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "Email đã tồn tại"
            };
        }

        // Tạo user mới
        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            FullName = registerDto.FullName,
            PhoneNumber = registerDto.PhoneNumber,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(); // lưu user

        // Tạo token
        var token = GenerateJwtToken(user);

        return new AuthResult
        {
            Success = true,
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt
            }
        };
    }

    public async Task<AuthResult> LoginAsync(LoginDto loginDto)
    {
        // Tìm user theo username hoặc email
        var user = await _unitOfWork.Users.GetByUsernameOrEmailAsync(loginDto.UsernameOrEmail);
        
        if (user == null)
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "Username/Email hoặc password không đúng"
            };
        }

        // Kiểm tra password
        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "Username/Email hoặc password không đúng"
            };
        }

        // Tạo token
        var token = GenerateJwtToken(user);

        return new AuthResult
        {
            Success = true,
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt
            }
        };
    }

    public Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// Tạo JWT token
    /// </summary>
    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!);
        var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}