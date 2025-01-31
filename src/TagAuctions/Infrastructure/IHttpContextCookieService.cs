namespace TagAuctions.Infrastructure;

public interface IHttpContextCookieService
{
    string? GetCookieValue(string key);
}