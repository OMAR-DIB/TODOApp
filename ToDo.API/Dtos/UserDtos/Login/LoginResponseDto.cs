namespace ToDo.API.Dtos.UserDtos.Login
{
    public record LoginResponseDto(string AccessToken, DateTime ExpiresAt, int UserId, string UserName);
}
