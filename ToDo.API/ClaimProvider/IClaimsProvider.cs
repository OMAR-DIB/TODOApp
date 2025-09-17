namespace ToDo.API.ClaimProvider
{
    public interface IClaimsProvider
    {
        long GetUserID();
        string GetUserName();
    }

}
