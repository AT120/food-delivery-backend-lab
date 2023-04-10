namespace AuthApi;

public class DebugMiddleware
{
    private readonly RequestDelegate _next;
    public DebugMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);
        var returned = true;
    }
}