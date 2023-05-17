using Microsoft.AspNetCore.Authentication.Cookies;

namespace AdminPanel;

public static class AuthChecker
{
    public static Task AdminOnlySingIn(CookieSigningInContext context)
    {
        var i = 1;
        return Task.CompletedTask;
    }
}