using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ToDo.API.middelware
{
    public class AuthLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthLoggingMiddleware> _logger;
        private readonly string[] _authPaths = new[] {
            "/api/user",                 // login, refresh, logout etc
            "/api/user/register",       // registration
            "/api/user/verify-email",   // email verification
            "/api/user/resend-verification"
        };
        private const int MaxBodyReadBytes = 64 * 1024; // 64 KB

        public AuthLoggingMiddleware(RequestDelegate next, ILogger<AuthLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var path = context.Request.Path.Value ?? string.Empty;
                if (IsAuthPath(path))
                {
                    // Read & sanitize request body (don't log passwords/tokens)
                    var requestBody = await ReadRequestBodyAsync(context);
                    var sanitized = SanitizeJson(requestBody);

                    // Capture response status by swapping the response body
                    var originalBodyStream = context.Response.Body;
                    await using var responseBody = new MemoryStream();
                    context.Response.Body = responseBody;

                    // Log attempt (incoming)
                    _logger.LogInformation("Auth attempt: {Method} {Path} from {IP} - Request: {Request}",
                        context.Request.Method, context.Request.Path, context.Connection.RemoteIpAddress, sanitized);

                    // proceed
                    await _next(context);

                    // read response body (optional) — but do not log tokens or big payloads
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    var respText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                    context.Response.Body.Seek(0, SeekOrigin.Begin);

                    // Log result (outgoing)
                    _logger.LogInformation("Auth result: {Method} {Path} from {IP} - Status: {StatusCode} - ResponsePreview: {RespPreview}",
                        context.Request.Method, context.Request.Path, context.Connection.RemoteIpAddress, context.Response.StatusCode, PreviewResponse(respText));

                    // copy the contents of the new memory stream (which contains the response) to the original stream.
                    await responseBody.CopyToAsync(originalBodyStream);
                }
                else
                {
                    await _next(context);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AuthLoggingMiddleware for path {Path}", context.Request.Path);
                // let exception bubble (or handle as you prefer)
                throw;
            }
        }

        private bool IsAuthPath(string path)
        {
            path = path.ToLowerInvariant();
            foreach (var p in _authPaths)
            {
                if (path.StartsWith(p, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private async Task<string> ReadRequestBodyAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            if (context.Request.ContentLength == null || context.Request.ContentLength == 0)
                return string.Empty;

            // Limit read size for safety
            var length = (int)Math.Min(context.Request.ContentLength.Value, MaxBodyReadBytes);
            var buffer = new char[length];

            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var read = await reader.ReadBlockAsync(buffer, 0, length);
            var body = new string(buffer, 0, read);

            // Reset position for next consumer
            context.Request.Body.Position = 0;
            return body;
        }

        // Remove sensitive properties from JSON payloads (passwords, tokens)
        private string SanitizeJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return string.Empty;

            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.ValueKind != JsonValueKind.Object) return Truncate(json);

                using var stream = new MemoryStream();
                using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false });

                writer.WriteStartObject();
                foreach (var prop in root.EnumerateObject())
                {
                    var name = prop.Name;
                    if (IsSensitiveField(name))
                    {
                        writer.WriteString(name, "[REDACTED]");
                        continue;
                    }

                    // write property as-is
                    prop.WriteTo(writer);
                }
                writer.WriteEndObject();
                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
            catch
            {
                // if parsing fails, return a truncated preview (never full body)
                return Truncate(json);
            }
        }

        private static bool IsSensitiveField(string name)
        {
            var lower = name.ToLowerInvariant();
            return lower.Contains("password") || lower.Contains("token") || lower.Contains("secret") || lower.Contains("pwd");
        }

        private static string Truncate(string s, int max = 512)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return s.Length <= max ? s : s.Substring(0, max) + "...";
        }

        private static string PreviewResponse(string respText)
        {
            // don't log long responses or tokens in responses; return a short preview
            if (string.IsNullOrEmpty(respText)) return string.Empty;
            var preview = respText.Length <= 256 ? respText : respText.Substring(0, 256) + "...";
            // remove known token fields heuristically
            return preview.Replace("\"accessToken\"", "\"accessToken\":\"[REDACTED]\"");
        }
    }
}
