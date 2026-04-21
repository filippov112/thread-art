using Application.DTO;

namespace Application.Interfaces;

public interface IIdentityService
{
    public Task<AuthResponseDto?> RegisterAsync(RegisterDto model);

    public Task<AuthResponseDto?> LoginAsync(LoginDto model);
}
