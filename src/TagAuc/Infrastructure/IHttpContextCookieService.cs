namespace TagAuc.Infrastructure;

public interface IHttpContextCookieService
{
    string? GetCookieValue(string key);
}