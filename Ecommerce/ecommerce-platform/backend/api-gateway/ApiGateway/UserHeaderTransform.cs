using Yarp.ReverseProxy.Transforms;
using System.Security.Claims;

public class UserHeaderTransform : RequestTransform
{
    public override ValueTask ApplyAsync(RequestTransformContext context)
    {
        var user = context.HttpContext.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            var userId = user.FindFirst("sub")?.Value;
            var username = user.FindFirst("preferred_username")?.Value;

            context.ProxyRequest.Headers.TryAddWithoutValidation("X-User-Id", userId);
            context.ProxyRequest.Headers.TryAddWithoutValidation("X-Username", username);

            var roles = user.Claims
                .Where(c => c.Type.Contains("role"))
                .Select(c => c.Value);

            context.ProxyRequest.Headers.TryAddWithoutValidation(
                "X-User-Roles",
                string.Join(",", roles));
        }

        return ValueTask.CompletedTask;
    }
}