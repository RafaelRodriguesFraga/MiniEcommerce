using DotnetBaseKit.Components.Api.Responses;

namespace Commons.Swagger.Docs;

public static class CommonResponsesDoc
{
    public static readonly (int, string, Type?, object?) Success =
        (200, "Success", typeof(Response), new { success = true, errors = new List<string>() });

    public static readonly (int, string, Type?, object?) BadRequest =
        (400, "Bad Request", typeof(Response), new { success = false, errors = new[] { "error key: error message" } });

    public static readonly (int, string, Type?, object?) Unauthorized =
        (401, "Unauthorized", null, null);

    public static readonly (int, string, Type?, object?) Forbidden =
        (403, "Forbidden", null, null);
}