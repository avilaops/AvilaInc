using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Manager.Contracts.DTOs;
using Manager.Core.Entities.Identity;
using Manager.Infrastructure.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace Manager.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository userRepository,
        ISessionRepository sessionRepository,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<AuthDTOs.AuthResponse> RegisterAsync(AuthDTOs.RegisterRequest request)
    {
        // Verificar se email já existe
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            return new AuthDTOs.AuthResponse
            {
                Success = false,
                Error = "Usuário já existe"
            };
        }

        // Hash da senha
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Criar usuário
        var user = new User
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            Name = request.Name,
            EmailVerified = false
        };

        // Gerar token de verificação
        var verifyToken = GenerateToken(new[]
        {
            new Claim("userId", user.Id.ToString()),
            new Claim("type", "verify")
        }, TimeSpan.FromHours(24));

        user.VerifyToken = verifyToken;
        user.VerifyTokenExpires = DateTime.UtcNow.AddHours(24);

        await _userRepository.AddAsync(user);

        // Enviar email de confirmação
        try
        {
            await _emailService.SendWelcomeEmailAsync(user.Email, user.Name, verifyToken);
        }
        catch (Exception ex)
        {
            // Log error but don't fail registration
            Console.WriteLine($"Failed to send welcome email: {ex.Message}");
        }

        return new AuthDTOs.AuthResponse
        {
            Success = true,
            Message = "Usuário criado com sucesso. Verifique seu email para confirmar a conta."
        };
    }

    public async Task<AuthDTOs.AuthResponse> LoginAsync(AuthDTOs.LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new AuthDTOs.AuthResponse
            {
                Success = false,
                Error = "Credenciais inválidas"
            };
        }

        if (!user.IsActive)
        {
            return new AuthDTOs.AuthResponse
            {
                Success = false,
                Error = "Conta desativada"
            };
        }

        // Gerar token JWT
        var token = GenerateToken(new[]
        {
            new Claim("userId", user.Id.ToString()),
            new Claim("email", user.Email)
        });

        // Criar sessão
        var session = new Session
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        await _sessionRepository.AddAsync(session);

        // Atualizar último login
        await _userRepository.UpdateLastLoginAsync(user.Id);

        return new AuthDTOs.AuthResponse
        {
            Success = true,
            Token = token,
            User = new AuthDTOs.UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt
            }
        };
    }

    public async Task<AuthDTOs.AuthResponse> ForgotPasswordAsync(AuthDTOs.ForgotPasswordRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
        {
            return new AuthDTOs.AuthResponse
            {
                Success = false,
                Error = "Usuário não encontrado"
            };
        }

        // Gerar token de reset
        var resetToken = GenerateToken(new[]
        {
            new Claim("userId", user.Id.ToString()),
            new Claim("type", "reset")
        }, TimeSpan.FromHours(1));

        user.ResetToken = resetToken;
        user.ResetTokenExpires = DateTime.UtcNow.AddHours(1);

        await _userRepository.UpdateAsync(user);

        // Enviar email
        try
        {
            await _emailService.SendPasswordResetEmailAsync(user.Email, user.Name, resetToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send reset email: {ex.Message}");
            return new AuthDTOs.AuthResponse
            {
                Success = false,
                Error = "Erro ao enviar email de redefinição"
            };
        }

        return new AuthDTOs.AuthResponse
        {
            Success = true,
            Message = "Email de redefinição enviado"
        };
    }

    public async Task<AuthDTOs.AuthResponse> ResetPasswordAsync(AuthDTOs.ResetPasswordRequest request)
    {
        var user = await _userRepository.GetByResetTokenAsync(request.Token);

        if (user == null)
        {
            return new AuthDTOs.AuthResponse
            {
                Success = false,
                Error = "Token inválido ou expirado"
            };
        }

        // Hash da nova senha
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.ResetToken = null;
        user.ResetTokenExpires = null;

        await _userRepository.UpdateAsync(user);

        // Revogar todas as sessões do usuário
        await _sessionRepository.RevokeUserSessionsAsync(user.Id);

        return new AuthDTOs.AuthResponse
        {
            Success = true,
            Message = "Senha redefinida com sucesso"
        };
    }

    public async Task<AuthDTOs.AuthResponse> ChangePasswordAsync(Guid userId, AuthDTOs.ChangePasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            return new AuthDTOs.AuthResponse
            {
                Success = false,
                Error = "Usuário não encontrado"
            };
        }

        // Verificar senha atual
        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return new AuthDTOs.AuthResponse
            {
                Success = false,
                Error = "Senha atual incorreta"
            };
        }

        // Hash da nova senha
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        await _userRepository.UpdateAsync(user);

        // Revogar todas as sessões exceto a atual (opcional)
        // await _sessionRepository.RevokeUserSessionsAsync(user.Id);

        return new AuthDTOs.AuthResponse
        {
            Success = true,
            Message = "Senha alterada com sucesso"
        };
    }

    public async Task<AuthDTOs.AuthResponse> VerifyEmailAsync(AuthDTOs.VerifyEmailRequest request)
    {
        var user = await _userRepository.GetByVerifyTokenAsync(request.Token);

        if (user == null)
        {
            return new AuthDTOs.AuthResponse
            {
                Success = false,
                Error = "Token inválido ou expirado"
            };
        }

        user.EmailVerified = true;
        user.VerifyToken = null;
        user.VerifyTokenExpires = null;

        await _userRepository.UpdateAsync(user);

        return new AuthDTOs.AuthResponse
        {
            Success = true,
            Message = "Email verificado com sucesso"
        };
    }

    public async Task<AuthDTOs.UserDTO?> GetCurrentUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            return null;
        }

        return new AuthDTOs.UserDTO
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt
        };
    }

    private string GenerateToken(Claim[] claims, TimeSpan? expiry = null)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "your-super-secret-jwt-key-here";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "AvilaDashboard",
            audience: _configuration["Jwt:Audience"] ?? "AvilaUsers",
            claims: claims,
            expires: DateTime.UtcNow.Add(expiry ?? TimeSpan.FromHours(24)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}