using ToDo.API.Dtos.UserDtos;
using ToDo.API.Dtos.UserDtos.Login;

namespace ToDo.API.Services.UserServices
{
    public interface IUserServices
    {
        Task<CreateUserResponseDto> CreateUser(CreateUserRequestDto dto, string? profileImagePath);
        Task<VerifyEmailResponseDto> VerifyEmailAsync(VerifyEmailRequestDto dto);
        Task<ResendVerificationResponseDto> ResendVerificationAsync(ResendVerificationRequestDto dto);

        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
    }
}
