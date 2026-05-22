using EffettoMandria.Api.Models;
using EffettoMandria.Api.Services;

namespace EffettoMandria.Api;

public static class HttpContextExtensions
{
    public const string AuthHeader = "X-Auth-Token";

    public static User? GetCurrentUser(this HttpContext ctx, AuthService auth)
    {
        if (!ctx.Request.Headers.TryGetValue(AuthHeader, out var values))
            return null;
        if (!Guid.TryParse(values.FirstOrDefault(), out var token))
            return null;
        return auth.GetUserFromToken(token);
    }
}
