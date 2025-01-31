namespace TagAuctions.Infrastructure;

public class HttpContextCookieService(IHttpContextAccessor httpContextAccessor) 
    : IHttpContextCookieService
{
    public string? GetCookieValue(string key)
    {
        return httpContextAccessor.HttpContext?.Request.Cookies[key];
    }
}
