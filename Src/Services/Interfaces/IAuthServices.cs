using iCarus.Src.Dtos.Users;

namespace iCarus.Src.Services.interfaces;

public interface IAuthServices
{
    Task<LoginResponse> Login(LoginDto loginDto);
    Task<RegisterResponse> Register(RegisterDto registerDto);
    Task<LoginResponse> ChangePassword(int userId, ChangePasswordDto dto);
}