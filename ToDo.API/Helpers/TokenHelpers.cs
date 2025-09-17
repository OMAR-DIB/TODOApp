using System.Security.Cryptography;
using System.Text;

public static class TokenHelpers
{
    public static string CreateNumericCode(int digits = 6)
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        var value = BitConverter.ToUInt32(bytes, 0) % (uint)Math.Pow(10, digits);
        return value.ToString($"D{digits}");
    }

    public static string Hash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha.ComputeHash(bytes);
        var sb = new StringBuilder();
        foreach (var b in hash) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    public static bool VerifyHash(string input, string? storedHash)
    {
        if (string.IsNullOrEmpty(storedHash)) return false;
        var computed = Hash(input);
        // constant time compare
        var a = Encoding.UTF8.GetBytes(computed);
        var b = Encoding.UTF8.GetBytes(storedHash);
        if (a.Length != b.Length) return false;
        return CryptographicOperations.FixedTimeEquals(a, b);
    }
}
