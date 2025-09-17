namespace ToDo.API.Services.TokenServices
{
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using ToDo.Data.Entities;

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly TimeSpan _tokenLifetime;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly byte[] _key;

        public TokenService(IConfiguration config)
        {
            _config = config;
            _issuer = _config["Jwt:Issuer"] ?? "ToDoApp";
            _audience = _config["Jwt:Audience"] ?? "ToDoAppClients";
            var minutes = int.TryParse(_config["Jwt:ExpiresMinutes"], out var m) ? m : 60;
            _tokenLifetime = TimeSpan.FromMinutes(minutes);
            var keyString = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured");
            _key = Encoding.UTF8.GetBytes(keyString);
        }

        public string CreateToken(User user, IEnumerable<string> roles)
        {
            var now = DateTime.UtcNow;

            // Claims: instructor expects "UserID" and "UserName" — we'll include them (and standard ones)
            var claims = new List<Claim>
        {
            new Claim("UserID", user.Id.ToString()),               // custom claim
            new Claim("UserName", user.Username ?? string.Empty),  // custom claim
            new Claim(JwtRegisteredClaimNames.Sub, user.Username ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, ((DateTimeOffset)now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

            // add role claims
            foreach (var role in roles ?? Enumerable.Empty<string>())
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var signingKey = new SymmetricSecurityKey(_key);
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(_tokenLifetime),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public DateTime GetExpiry() => DateTime.UtcNow.Add(_tokenLifetime);
    }

}
