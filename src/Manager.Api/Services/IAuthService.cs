using Manager.Contracts.DTOs;

namespace Manager.Api.Services;

public interface IAuthService
{
    Task<AuthDTOs.AuthResponse> RegisterAsync(AuthDTOs.RegisterRequest request);
    Task<AuthDTOs.AuthResponse> LoginAsync(AuthDTOs.LoginRequest request);
    Task<AuthDTOs.AuthResponse> ForgotPasswordAsync(AuthDTOs.ForgotPasswordRequest request);
    Task<AuthDTOs.AuthResponse> ResetPasswordAsync(AuthDTOs.ResetPasswordRequest request);
    Task<AuthDTOs.AuthResponse> ChangePasswordAsync(Guid userId, AuthDTOs.ChangePasswordRequest request);
    Task<AuthDTOs.AuthResponse> VerifyEmailAsync(AuthDTOs.VerifyEmailRequest request);
    Task<AuthDTOs.UserDTO?> GetCurrentUserAsync(Guid userId);
}