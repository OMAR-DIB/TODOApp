namespace ToDo.API.ClaimProvider
{
    internal class ClaimsProvider : IClaimsProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimsProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public long GetUserID()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("UserID");
            if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long userId))
            {
                return userId;
            }

            throw new Exception("User ID claim not found or invalid.");
        }

        public string GetUserName()
        {
            var userNameClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("UserName");
            if (userNameClaim != null && !string.IsNullOrEmpty(userNameClaim.Value))
            {
                return userNameClaim.Value;
            }
            throw new Exception("User Name claim not found.");
        }
    }
}
