using ToDo.Data.Entities;

namespace ToDo.API.Services.TokenServices
{
    public interface ITokenService
    {
        string CreateToken(User user, IEnumerable<string> roles);
        DateTime GetExpiry();
    }
}
