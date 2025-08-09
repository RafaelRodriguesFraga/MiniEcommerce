using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AuthService.Infra.Middlewares
{
    public class HttpLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HttpLoggingMiddleware> _logger;
        private static readonly string[] SensitiveFields = ["password", "token", "refreshToken"];

        public HttpLoggingMiddleware(RequestDelegate next, ILogger<HttpLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            var request = context.Request;
            var requestBody = request.Body;
            string requestBodyText = "";

            if (request.ContentLength > 0 && request.Body.CanRead)
            {
                request.EnableBuffering();
                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                requestBodyText = await reader.ReadToEndAsync();
                request.Body.Position = 0;
            }

            var responseBody = context.Response.Body;
            using var newResponseBody = new MemoryStream();
            context.Response.Body = newResponseBody;

            await _next(context);

            stopwatch.Stop();

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            var log = new
            {
                method = request.Method,
                path = request.Path.Value,
                status = context.Response.StatusCode,
                duration = stopwatch.ElapsedMilliseconds + "ms",
                request = new
                {
                    headers = request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                    parameters = request.Query.ToDictionary(q => q.Key, q => q.Value.ToString()),
                    body = TryParseJson(requestBodyText)
                },
                response = new
                {
                    headers = context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                    body = TryParseJson(responseBodyText)
                }
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var formattedLog = JsonSerializer.Serialize(log, options);
            _logger.LogInformation("{Log}", formattedLog);

            await newResponseBody.CopyToAsync(responseBody);
        }

        private object TryParseJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                using var doc = JsonDocument.Parse(json);

                return ConvertJsonToObject(doc.RootElement);
            }
            catch
            {
                return json;
            }
        }

        // TODO: IMPROVE LOG CODE
        private object ConvertJsonToObject(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    var dict = new Dictionary<string, object>();

                    foreach (var prop in element.EnumerateObject())
                    {
                        dict[prop.Name] = SensitiveFields.Contains(prop.Name, StringComparer.OrdinalIgnoreCase) ? "*****" : ConvertJsonToObject(prop.Value);
                    }
                    return dict;

                case JsonValueKind.Array:
                    return element.EnumerateArray()
                                  .Select(ConvertJsonToObject)
                                  .ToList();

                case JsonValueKind.String:
                    return element.GetString();

                case JsonValueKind.Number:
                    if (element.TryGetInt32(out int intValue))
                        return intValue;
                    if (element.TryGetInt64(out long longValue))
                        return longValue;
                    return element.GetDecimal();

                case JsonValueKind.True:
                    return true;

                case JsonValueKind.False:
                    return false;

                case JsonValueKind.Null:
                    return null;

                default:
                    return null;
            }
        }

    }
}